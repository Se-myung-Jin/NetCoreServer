namespace Server
{
    class SessionManager
    {
        static SessionManager instance = new SessionManager();
        public static SessionManager Instance { get { return instance; } }

        int sessionId = 0;
        Dictionary<int, ClientSession> sessionDic = new Dictionary<int, ClientSession>();
        object sessionLock = new object();

        public ClientSession GenerateSession()
        {
            lock (sessionLock)
            {
                int sessionId = ++this.sessionId;

                ClientSession session = new ClientSession();
                session.SessionId = sessionId;
                sessionDic.Add(sessionId, session);

                Console.WriteLine($"Connected : {sessionId}");
                return session;
            }
        }

        public ClientSession Find(int id)
        {
            lock (sessionLock)
            {
                ClientSession session = null;
                sessionDic.TryGetValue(id, out session);
                
                return session;
            }
        }

        public void RemoveSession(ClientSession session)
        {
            lock (sessionLock)
            {
                sessionDic.Remove(session.SessionId);
            }
        }
    }
}
