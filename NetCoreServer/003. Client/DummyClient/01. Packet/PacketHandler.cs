using ServerCore;

namespace DummyClient
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
            
        }

        public static void ChatResHandler(PacketSession session, IPacket packet)
        {
            ChatRes res = packet as ChatRes;
            ServerSession serverSession = session as ServerSession;

            //if (res.PlayerId == 1)
                //Console.WriteLine(res.Chat);
        }
    }
}
