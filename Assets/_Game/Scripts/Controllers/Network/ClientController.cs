using System;
using System.Threading;
using Game.Common;
using ModestTree;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Controllers.Network
{
    public class ClientController : IInitializable, IDisposable, IRakClient
    {
        private readonly CompositeDisposable _disposable = new();

        public readonly ReactiveCommand<ConnectInfo> ConnectedInfo = new();
        public readonly ReactiveCommand<NetworkPackets.Packet> Packet = new();
        
        private Thread _loopThread;
        private bool _loopThreadActive;
        private bool _stopThread;

        public void Initialize()
        {
            Packet.Subscribe(x =>
                {
                    switch (x)
                    {
                        case NetworkPackets.ClientDataAccepted accepted:
                            Debug.Log(accepted.PayLoad);
                            break;
                        case NetworkPackets.ClientDataRequest request:
                            using (var bsOut = PooledBitStream.GetBitStream())
                            {
                                bsOut.Write((byte)GamePacketID.CLIENT_DATA_REPLY);
                                bsOut.Write("Wtf");
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

        public void Dispose()
        {
            _stopThread = true;
            _disposable.Dispose();
            RakClient.Destroy();
        }

        private void Update()
        {
            while (!_stopThread)
            {
                RakClient.Update();
                Thread.Sleep(1);
            }

            _loopThreadActive = false;
        }

        private void StartUpdate()
        {
            if (_loopThreadActive)
            {
                _stopThread = true;
                return;
            }
            _loopThread = new Thread(Update);
            _loopThread.Start();
            _loopThreadActive = true;
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
                        PayLoad = bitStream.ReadString()
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