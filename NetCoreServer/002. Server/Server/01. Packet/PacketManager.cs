using ServerCore;

namespace Server
{
    public class PacketManager
    {
        static PacketManager instance;
        public static PacketManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PacketManager();
                }

                return instance;
            }
        }
        Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> makePacketDic = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();
        Dictionary<ushort, Action<PacketSession, IPacket>> packetHandlerDic = new Dictionary<ushort, Action<PacketSession, IPacket>>();

        public void RegisterPacketHandler()
        {
            makePacketDic.Add((ushort)PacketID.C_LeaveGame, MakePacket<C_LeaveGame>);
            makePacketDic.Add((ushort)PacketID.C_Move, MakePacket<C_Move>);

            packetHandlerDic.Add((ushort)PacketID.C_LeaveGame, PacketHandler.C_LeaveGameHandler);
            packetHandlerDic.Add((ushort)PacketID.C_Move, PacketHandler.C_MoveHandler);
        }

        void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
        {
            T packet = new T();
            packet.ReadPacket(buffer);

            Action<PacketSession, IPacket> action = null;
            if (packetHandlerDic.TryGetValue(packet.PacketId, out action))
            {
                action.Invoke(session, packet);
            }
        }

        public void OnParsePacket(PacketSession session, ArraySegment<byte> buffer)
        {
            ushort count = 0;

            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;

            Action<PacketSession, ArraySegment<byte>> action = null;
            if (makePacketDic.TryGetValue(id, out action))
            {
                action.Invoke(session, buffer);
            }
        }
    }
}
