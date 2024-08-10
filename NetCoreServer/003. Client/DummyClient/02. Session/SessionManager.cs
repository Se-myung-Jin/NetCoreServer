namespace DummyClient
{
    class SessionManager
    {
        static SessionManager instance = new SessionManager();
        public static SessionManager Instance { get { return instance; } }

        List<ServerSession> sessionList = new List<ServerSession>();
        object listLock = new object();

        public ServerSession GenerateSession()
        {
            lock (listLock)
            {
                ServerSession session = new ServerSession();
                sessionList.Add(session);
                return session;
            }
        }

        public void SendForEach()
        {
            lock (listLock)
            {
                foreach (ServerSession session in sessionList)
                {
                    ChatReq req = new ChatReq();
                    req.Chat = $"Hello Server!";

                    ArraySegment<byte> segment = req.WritePacket();
                    session.Send(segment);
                }
            }
        }
    }
}
