namespace DummyClient
{
    class SessionManager
    {
        static SessionManager instance = new SessionManager();
        public static SessionManager Instance { get { return instance; } }

        List<ServerSession> sessionList = new List<ServerSession>();
        object listLock = new object();
        Random rand = new Random();

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
                    C_Move movePacket = new C_Move();
                    movePacket.posX = rand.Next(-50, 50);
                    movePacket.posY = 0;
                    movePacket.posZ = rand.Next(-50, 50);
                    session.Send(movePacket.WritePacket());
                }
            }
        }
    }
}
