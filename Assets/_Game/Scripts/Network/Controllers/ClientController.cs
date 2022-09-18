using System;
using System.Threading;
using Game.Common;
using ModestTree;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Network
{
    public class ClientController : IInitializable, IDisposable, IRakClient
    {
        private readonly SignalBus _signalBus;
        
        private readonly CompositeDisposable _disposable = new();

        public readonly ReactiveCommand<ConnectInfo> ConnectedInfo = new();
        public readonly ReactiveCommand<NetworkPackets.Packet> Packet = new();
        
        private Thread _loopThread;
        private bool _loopThreadActive;
        private bool _stopThread;

        public string NickName { get; private set; }

        private ClientController(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }
        
        public void Initialize()
        {
            Packet.Subscribe(x =>
                {
                    _signalBus.Fire(new NetworkSignals.ClientPacketReceived()
                    {
                        Packet = x
                    });
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
                                RakClient.Send(bsOut, PacketPriority.IMMEDIATE_PRIORITY, PacketReliability.RELIABLE, 0);
                            }
                            break;
                    }
                });

            // ConnectedInfo.Subscribe(x =>
            // {
            //     Debug.Log($"{x.Address}:{x.Port}, {x.Password}, {x.State}, {x.DisconnectMessage}, {x.DisconnectReason}");
            // });
            
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
            _stopThread = true;
            _disposable.Dispose();
            RakClient.Destroy();
        }

        public void SetNickName(string nickName)
        {
            NickName = nickName;
        }

        private void Update()
        {
            while (!_stopThread)
            {
                Thread.Sleep(10);
                RakClient.Update();
            }

            _loopThreadActive = _stopThread = false;
        }

        private void StartUpdate()
        {
            if (_loopThreadActive)
                return;
            _loopThread = new Thread(Update);
            _loopThread.Start();
            _loopThreadActive = true;
        }

        public void Disconnect()
        {
            _stopThread = true;
            RakClient.Disconnect();
        }

        public ClientConnectResult Connect(string address, ushort port, string password = "", int attemps = 10)
        {
            StartUpdate();
            return RakClient.Connect(address, port, password, attemps);
        }

        public void OnConnecting(string address, ushort port, string password)
        {
            ConnectedInfo.Execute(new ConnectInfo()
            {
                Address = address,
                Port = port,
                Password = password,
                State = ClientState.IS_CONNECTING
            });
        }

        public void OnConnected(string address, ushort port, string password)
        {
            ConnectedInfo.Execute(new ConnectInfo()
            {
                Address = address,
                Port = port,
                Password = password,
                State = ClientState.IS_CONNECTED
            });
        }

        public void OnDisconnected(DisconnectReason reason, string message = "")
        {
            ConnectedInfo.Execute(new ConnectInfo()
            {
                State = ClientState.IS_DISCONNECTED,
                DisconnectMessage = message,
                DisconnectReason = reason
            });
        }

        public void OnReceived(GamePacketID packet_id, uint packet_size, BitStream bitStream, ulong local_time)
        {
            Debug.Log($"OnReceived: {packet_id} {packet_size} {local_time}");
            switch (packet_id)
            {
                case GamePacketID.CLIENT_DATA_REQUEST:
                    Packet.Execute(new NetworkPackets.ClientDataRequest()
                    {
                        GamePacketID = packet_id,
                        PacketSize = packet_size,
                        PacketStream = bitStream,
                        LocalTime = local_time
                    });
                    break;
                case GamePacketID.CLIENT_DATA_ACCEPTED:
                    Packet.Execute(new NetworkPackets.ClientDataAccepted()
                    {
                        GamePacketID = packet_id,
                        PacketSize = packet_size,
                        PacketStream = bitStream,
                        LocalTime = local_time,
                        NickName = bitStream.ReadString()
                    });
                    break;
                case GamePacketID.SERVER_CHAT_MESSAGE:
                    Packet.Execute(new NetworkPackets.ServerChatMessage()
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