using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class Listener
    {
        Socket listenSocket;
        Func<Session> sessionFactory;

        public void StartListening(IPEndPoint endPoint, Func<Session> sessionFactory, int acceptorCount = 10, int backLog = 100)
        {
            listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.sessionFactory += sessionFactory;

            listenSocket.Bind(endPoint);

            listenSocket.Listen(backLog);

            for (int i = 0; i < acceptorCount; i++)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
                StartAccept(args);
            }
        }

        void StartAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            bool pending = listenSocket.AcceptAsync(args);
            if (pending == false)
            {
                OnAcceptCompleted(null, args);
            }
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                ProcessAccept(args);
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }

            StartAccept(args);
        }

        void ProcessAccept(SocketAsyncEventArgs args)
        {
            Session session = sessionFactory.Invoke();
            session.InitializeSession(args.AcceptSocket);

            session.OnConnected(args.AcceptSocket.RemoteEndPoint);
        }
    }
}
