using System;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Network
{
    public class ServerController : IInitializable, IDisposable, IRakServer
    {
        private readonly CompositeDisposable _disposable = new();

        public readonly ReactiveCollection<ClientData> Clients = new();

        private Thread _loopThread;
        private bool _loopThreadActive;
        private bool _stopThread;

        public void Initialize()
        {
            RakServer.RegisterInterface(this);
            RakServer.Init();
            StartUpdate();
        }

        public void Dispose()
        {
            _stopThread = true;
            _disposable.Dispose();
            RakServer.Destroy();
        }

        private void Update()
        {
            while (!_stopThread)
            {
                Thread.Sleep(100);
                RakServer.Update();
            }
            
            _loopThreadActive = _stopThread = false;
        }

        public void Start()
        {
            StartUpdate();
            RakServer.Start("192.168.31.45", 30502);
        }

        public void Stop()
        {
            _stopThread = true;
            RakServer.Stop();
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

        public void OnConnected(ushort connectionIndex, ulong guid)
        {
            using var bitStream = PooledBitStream.GetBitStream();
            bitStream.Write((byte)GamePacketID.CLIENT_DATA_REQUEST);
            RakServer.SendToClient(bitStream, guid, PacketPriority.IMMEDIATE_PRIORITY, PacketReliability.RELIABLE, 0);
        }

        public void OnDisconnected(ushort connectionIndex, ulong guid, DisconnectReason reason, string message)
        {
            if (Clients[connectionIndex] != null && Clients[connectionIndex].Guid == guid)
            {
                Debug.Log("[Server] Client " + Clients[connectionIndex].NickName + " disconnected! (" + reason + ")");
                Clients.RemoveAt(connectionIndex);
            }
            else
            {
                Debug.Log("[Server] Client " + RakServer.GetAddress(guid,true) + " disconnected! (" + reason + ")");
            }
        }

        public void OnReceived(GamePacketID packet_id, ushort connectionIndex, ulong guid, BitStream bitStream, ulong local_time)
        {
            switch (packet_id)
            {
                case GamePacketID.CLIENT_DATA_REPLY:
                    var playerName = bitStream.ReadString();

                    Clients.Add(new ClientData(guid, playerName));

                    using(var bsOut = PooledBitStream.GetBitStream())
                    {
                        bsOut.Write((byte)GamePacketID.CLIENT_DATA_ACCEPTED);
                        bsOut.Write("edited_"+playerName);
                        RakServer.SendToClient(bsOut, guid, PacketPriority.LOW_PRIORITY, PacketReliability.RELIABLE, 0);
                    }
                    break;
                case GamePacketID.CLIENT_CHAT_MESSAGE:
                    var text = bitStream.ReadString();
                    using(var bsOut = PooledBitStream.GetBitStream())
                    {
                        bsOut.Write((byte)GamePacketID.SERVER_CHAT_MESSAGE);
                        bsOut.Write(Clients.First(x => x.Guid == guid).NickName);
                        bsOut.Write(text);
                        RakServer.SendToAllIgnore(bsOut, guid, PacketPriority.LOW_PRIORITY, PacketReliability.RELIABLE, 0);
                    }
                    break;
            }
        }
        
        public class ClientData
        {
            public ulong Guid;
            public string NickName;

            public ClientData(ulong guid, string username)
            {
                Guid = guid;
                NickName = username;
            }
        }
    }
}