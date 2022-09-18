using System;
using System.Threading;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Controllers.Network
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
                RakServer.Update();
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
                Debug.Log("[Server] Client " + Clients[connectionIndex].PlayerName + " disconnected! (" + reason + ")");
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

                    using(PooledBitStream bsOut = PooledBitStream.GetBitStream())
                    {
                        bsOut.Write((byte)GamePacketID.CLIENT_DATA_ACCEPTED);
                        bsOut.Write("edited_"+playerName);
                        RakServer.SendToClient(bsOut, guid, PacketPriority.LOW_PRIORITY, PacketReliability.RELIABLE, 0);
                    }
                    break;
            }
        }
        
        public class ClientData
        {
            public ulong Guid;
            public string PlayerName;

            public ClientData(ulong guid, string username)
            {
                Guid = guid;
                PlayerName = username;
            }
        }
    }
}