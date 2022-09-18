using Game.Network;

namespace Game.Common
{
    public static class NetworkSignals
    {
        public class ClientPacketReceived
        {
            public NetworkPackets.Packet Packet;
        }
    }
}