using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class Listener
    {
        Socket listenSocket;

        public void StartListening(IPEndPoint endpoint, int backLog = 100)
        {
            listenSocket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            listenSocket.Bind(endpoint);

            listenSocket.Listen(backLog);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            StartAccept(args);
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
                ProcessAccept();
            }
            else
            {
                // TODO : ERROR LOG
            }

            StartAccept(args);
        }

        void ProcessAccept()
        {
            // TODO : Process Accept
        }
    }
}
