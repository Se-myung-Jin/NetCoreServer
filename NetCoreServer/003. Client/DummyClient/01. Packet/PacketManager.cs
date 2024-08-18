using ServerCore;

namespace DummyClient
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
            makePacketDic.Add((ushort)PacketID.S_BroadcastEnterGame, MakePacket<S_BroadcastEnterGame>);
            makePacketDic.Add((ushort)PacketID.S_BroadcastLeaveGame, MakePacket<S_BroadcastLeaveGame>);
            makePacketDic.Add((ushort)PacketID.S_PlayerList, MakePacket<S_PlayerList>);
            makePacketDic.Add((ushort)PacketID.S_BroadcastMove, MakePacket<S_BroadcastMove>);

            packetHandlerDic.Add((ushort)PacketID.S_BroadcastEnterGame, PacketHandler.S_BroadcastEnterGameHandler);
            packetHandlerDic.Add((ushort)PacketID.S_BroadcastLeaveGame, PacketHandler.S_BroadcastLeaveGameHandler);
            packetHandlerDic.Add((ushort)PacketID.S_PlayerList, PacketHandler.S_PlayerListHandler);
            packetHandlerDic.Add((ushort)PacketID.S_BroadcastMove, PacketHandler.S_BroadcastMoveHandler);
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
