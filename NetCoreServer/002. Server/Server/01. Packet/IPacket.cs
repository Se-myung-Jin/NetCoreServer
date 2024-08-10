namespace Server
{
    public interface IPacket
    {
        ushort PacketId { get; }

        void ReadPacket(ArraySegment<byte> segment);
        ArraySegment<byte> WritePacket();
    }
}
