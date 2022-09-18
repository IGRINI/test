namespace Game.Common
{
    public static class NetworkPackets
    {
        public abstract class Packet
        {
            public GamePacketID GamePacketID;
            public uint PacketSize;
            public BitStream PacketStream;
            public ulong LocalTime;
        }
        
        public class ClientDataRequest : Packet {}

        public class ClientDataAccepted : Packet
        {
            public string PayLoad;
        }
    }
}