using ServerCore;

namespace Server
{
    public class PacketHandler
    {
        public static void PlayerInfoReqHandler(PacketSession session, IPacket packet)
        {
            PlayerInfoReq req = packet as PlayerInfoReq;

            Console.WriteLine($"PlayerInfoReq : {req.PlayerId}, {req.Name}");

            foreach (PlayerInfoReq.SkillInfo skill in req.Skills)
            {
                Console.WriteLine($"Skill ({skill.id}) ({skill.level}) ({skill.duration})");
            }
        }

        public static void ChatReqHandler(PacketSession session, IPacket packet)
        {
            ChatReq req = packet as ChatReq;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null) return;

            GameRoom room = clientSession.Room;
            room.Push(() => room.Broadcast(clientSession, req.Chat));
        }

        public static void ChatResHandler(PacketSession session, IPacket packet)
        {
            ChatRes res = packet as ChatRes;

        }
    }
}
