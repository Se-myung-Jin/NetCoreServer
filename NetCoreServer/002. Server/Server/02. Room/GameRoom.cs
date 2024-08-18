using ServerCore;

namespace Server
{
    public class GameRoom : IRoom<ClientSession>, IJobQueue
    {
        List<ClientSession> sessionList = new List<ClientSession>();
        JobQueue jobQueue = new JobQueue();
        List<ArraySegment<byte>> pendingList = new List<ArraySegment<byte>>();

        public void Broadcast(ArraySegment<byte> segment)
        {
            pendingList.Add(segment);
        }

        public void Enter(ClientSession session)
        {
            sessionList.Add(session);
            session.Room = this;

            S_PlayerList players = new S_PlayerList();
            foreach (ClientSession s in sessionList)
            {
                players.players.Add(new S_PlayerList.Player()
                {
                    isSelf = (s == session),
                    playerId = s.SessionId,
                    posX = s.PosX,
                    posY = s.PosY,
                    posZ = s.PosZ,
                });
            }
            session.Send(players.WritePacket());

            S_BroadcastEnterGame enter = new S_BroadcastEnterGame();
            enter.playerId = session.SessionId;
            enter.posX = 0;
            enter.posY = 0;
            enter.posZ = 0;
            Broadcast(enter.WritePacket());
        }

        public void Leave(ClientSession session)
        {
            sessionList.Remove(session);

            S_BroadcastLeaveGame leave = new S_BroadcastLeaveGame();
            leave.playerId = session.SessionId;
            Broadcast(leave.WritePacket());
        }

        public void Push(Action job)
        {
            jobQueue.Push(job);
        }

        public void Flush()
        {
            foreach (ClientSession session in sessionList)
            {
                session.Send(pendingList);
            }

            Console.WriteLine($"Flush Memory Count : {pendingList.Count}");
            pendingList.Clear();
        }

        public void Move(ClientSession session, C_Move movePacket)
        {
            session.PosX = movePacket.posX;
            session.PosY = movePacket.posY;
            session.PosZ = movePacket.posZ;

            S_BroadcastMove move = new S_BroadcastMove();
            move.playerId = session.SessionId;
            move.posX = movePacket.posX;
            move.posY = movePacket.posY;
            move.posZ = movePacket.posZ;
            Broadcast(move.WritePacket());
        }
    }
}
