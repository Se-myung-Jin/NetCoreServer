using ServerCore;

namespace Server
{
    public class GameRoom : IRoom<ClientSession>, IJobQueue
    {
        List<ClientSession> sessionList = new List<ClientSession>();
        JobQueue jobQueue = new JobQueue();
        List<ArraySegment<byte>> pendingList = new List<ArraySegment<byte>>();

        public void Broadcast(ClientSession session, string chat)
        {
            ChatRes res = new ChatRes();
            res.PlayerId = session.SessionId;
            res.Chat = $"{chat} I am {res.PlayerId}";

            ArraySegment<byte> segment = res.WritePacket();

            pendingList.Add(segment);
        }

        public void Enter(ClientSession session)
        {
            sessionList.Add(session);
            session.Room = this;
        }

        public void Leave(ClientSession session)
        {
            sessionList.Remove(session);
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
    }
}
