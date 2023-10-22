using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Common;
using Game.Services;
using ModestTree;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using Zenject;

namespace Game.Network
{
    public class ClientController : IInitializable, IDisposable, IRakClient
    {
        public int Ping => RakClient.Ping;
        
        private CancellationTokenSource _loopToken;

        private readonly ConnectInfo ConnectionInfo = new();

        public event Action<ConnectInfo> ConnectionUpdate;
        public event Action<NetworkPackets.Packet> OnPacketReceived;
        
        private Thread _loopThread;
        private bool _loopThreadActive;

        public string NickName { get; private set; }
        
        public void Initialize()
        {
            OnPacketReceived += async x =>
                {
                    switch (x)
                    {
                        case NetworkPackets.ClientDataAccepted accepted:
                            Debug.Log(accepted.NickName);
                            break;
                        case NetworkPackets.ClientDataRequest request:
                            Debug.Log("request");
                            using (var bsOut = PooledBitStream.GetBitStream())
                            {
                                bsOut.Write((byte)GamePacketID.CLIENT_DATA_REPLY);
                                bsOut.Write(NickName);
                                var ticket = await SteamService.GetAuthTicket();
                                bsOut.Write(BitConverter.ToString(ticket.Data).Replace("-", string.Empty));
                                RakClient.Send(bsOut, PacketPriority.IMMEDIATE_PRIORITY, PacketReliability.RELIABLE);
                            }
                            break;
                    }
                };

            // ConnectedInfo.Subscribe(x =>
            // {
            //     Debug.Log($"{x.Address}:{x.Port}, {x.Password}, {x.State}, {x.DisconnectMessage}, {x.DisconnectReason}");
            // });
            
            _loopToken = new CancellationTokenSource();
            RakClient.RegisterInterface(this);
            RakClient.Init();
            StartUpdate();
        }

        public void SendPacket(NetworkPackets.Packet packet)
        {
            using var bsOut = PooledBitStream.GetBitStream();
            switch (packet)
            {
                case NetworkPackets.ClientChatMessage clientChatMessage:
                    bsOut.Write((byte)GamePacketID.CLIENT_CHAT_MESSAGE);
                    bsOut.Write(clientChatMessage.Text);
                    RakClient.Send(bsOut, PacketPriority.IMMEDIATE_PRIORITY, PacketReliability.RELIABLE, 0);
                    break;
            }
        }

        public void Dispose()
        {
            _loopToken.Cancel();
            _loopToken.Dispose();
            _loopThreadActive = false;
            RakClient.Destroy();
        }

        public void SetNickName(string nickName)
        {
            NickName = nickName;
        }

        private async void Update()
        {
            while (!_loopToken.IsCancellationRequested)
            {
                RakClient.Update();
                await UniTask.Delay(100);
            }
        }

        private void StartUpdate()
        {
            if (_loopThreadActive)
                return;
            
            _loopThreadActive = true;
            _ = UniTask.RunOnThreadPool(Update, false, _loopToken.Token);
        }

        public void Disconnect()
        {
            _loopToken.Cancel();
            _loopToken.Dispose();
            _loopThreadActive = false;
            RakClient.Disconnect();
        }

        public ClientConnectResult Connect(string address, ushort port, string password = "", int attemps = 10)
        {
            StartUpdate();
            return RakClient.Connect(address, port, password, attemps);
        }

        public void OnConnecting(string address, ushort port, string password)
        {
            ConnectionInfo.Address = address;
            ConnectionInfo.Port = port;
            ConnectionInfo.Password = password;
            ConnectionInfo.State = ClientState.IS_CONNECTING;
            ConnectionUpdate?.Invoke(ConnectionInfo);
        }

        public void OnConnected(string address, ushort port, string password)
        {
            ConnectionInfo.Address = address;
            ConnectionInfo.Port = port;
            ConnectionInfo.Password = password;
            ConnectionInfo.State = ClientState.IS_CONNECTED;
            ConnectionUpdate?.Invoke(ConnectionInfo);
        }

        public void OnDisconnected(DisconnectReason reason, string message = "")
        {
            ConnectionInfo.DisconnectMessage = message;
            ConnectionInfo.DisconnectReason = reason;
            ConnectionInfo.State = ClientState.IS_DISCONNECTED;
            ConnectionUpdate?.Invoke(ConnectionInfo);
        }

        public void OnReceived(GamePacketID packet_id, uint packet_size, BitStream bitStream, ulong local_time)
        {
            Debug.Log($"OnReceived: {packet_id} {packet_size} {local_time}");
            switch (packet_id)
            {
                case GamePacketID.CLIENT_DATA_REQUEST:
                    OnPacketReceived?.Invoke(new NetworkPackets.ClientDataRequest()
                    {
                        GamePacketID = packet_id,
                        PacketSize = packet_size,
                        PacketStream = bitStream,
                        LocalTime = local_time
                    });
                    break;
                case GamePacketID.CLIENT_DATA_ACCEPTED:
                    OnPacketReceived?.Invoke(new NetworkPackets.ClientDataAccepted()
                    {
                        GamePacketID = packet_id,
                        PacketSize = packet_size,
                        PacketStream = bitStream,
                        LocalTime = local_time,
                        NickName = bitStream.ReadString()
                    });
                    break;
                case GamePacketID.SERVER_CHAT_MESSAGE:
                    OnPacketReceived?.Invoke(new NetworkPackets.ServerChatMessage()
                    {
                        GamePacketID = packet_id,
                        PacketSize = packet_size,
                        PacketStream = bitStream,
                        LocalTime = local_time,
                        NickName = bitStream.ReadString(),
                        Text = bitStream.ReadString(),
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(packet_id), packet_id, null);
            }
        }

        public class ConnectInfo
        {
            public string Address;
            public ushort Port;
            public string Password;
            public ClientState State;
            public DisconnectReason DisconnectReason;
            public string DisconnectMessage;
        }
    }
}