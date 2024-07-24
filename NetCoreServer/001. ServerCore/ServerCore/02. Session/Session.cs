using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public abstract class Session
    {
        Socket socket;

        SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();

        Queue<byte[]> sendQueue = new Queue<byte[]>();
        List<ArraySegment<byte>> pendingList = new List<ArraySegment<byte>>();

        object sendLock = new object();

        int disconnected = 0;

        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnDisconnected(EndPoint endPoint);
        public abstract void OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);

        public void InitializeSession(Socket socket)
        {
            this.socket = socket;

            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            
            recvArgs.SetBuffer(new byte[1024], 0, 1024);
            StartRecv();
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref disconnected, 1) == 1) return;
            
            OnDisconnected(socket.RemoteEndPoint);

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        public void Send(byte[] sendBuff)
        {
            lock (sendLock)
            {
                sendQueue.Enqueue(sendBuff);

                if (pendingList.Count == 0)
                {
                    StartSend();
                }
            }
        }

        void StartSend()
        {
            while (sendQueue.Count > 0)
            {
                byte[] buff = sendQueue.Dequeue();
                pendingList.Add(new ArraySegment<byte>(buff, 0, buff.Length));
            }

            sendArgs.BufferList = pendingList;

            bool pending = socket.SendAsync(sendArgs);
            if (pending == false)
            {
                OnSendCompleted(null, sendArgs);
            }
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (sendLock)
            {
                if (args.IsTransferSuccess())
                {
                    try
                    {
                        sendArgs.BufferList = null;
                        pendingList.Clear();

                        OnSend(sendArgs.BytesTransferred);

                        if (sendQueue.Count > 0)
                        {
                            StartSend();
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted Failed {e}");
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }

        void StartRecv()
        {
            bool pending = socket.ReceiveAsync(recvArgs);
            if (pending == false)
            {
                OnRecvCompleted(null, recvArgs);
            }
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.IsTransferSuccess())
            {
                try
                {
                    OnRecv(new ArraySegment<byte>(args.Buffer, args.Offset, args.BytesTransferred));

                    StartRecv();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed {e}");
                }
            }
            else
            {
                Disconnect();
            }
        }
    }
}
