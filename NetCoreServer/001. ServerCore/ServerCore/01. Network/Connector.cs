using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class Connector
    {
        Func<Session> sessionFactory;
        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.sessionFactory = sessionFactory;

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnectCompleted);
            args.RemoteEndPoint = endPoint;
            args.UserToken = socket;

            StartConnect(args);
        }

        void StartConnect(SocketAsyncEventArgs args)
        {
            Socket socket = args.UserToken as Socket;
            if (socket == null) return;

            bool pending = socket.ConnectAsync(args);
            if (pending == false)
            {
                OnConnectCompleted(null, args);
            }
        }

        void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                ProcessConnect(args);
            }
            else
            {
                Console.WriteLine($"OnConnectCompleted Failed: {args.SocketError}");
            }
        }

        void ProcessConnect(SocketAsyncEventArgs args)
        {
            Session session = sessionFactory.Invoke();
            session.InitializeSession(args.ConnectSocket);
            session.OnConnected(args.RemoteEndPoint);
        }
    }
}
