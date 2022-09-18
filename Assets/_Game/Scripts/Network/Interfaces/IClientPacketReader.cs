namespace Game.Network
{
    public interface IClientPacketReader
    {
        public void ReceivePacket(NetworkPackets.Packet packet);
    }
}