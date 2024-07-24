using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    public class Session
    {
        Socket socket;

        SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();

        Queue<byte[]> sendQueue = new Queue<byte[]>();
        List<ArraySegment<byte>> pendingList = new List<ArraySegment<byte>>();

        object sendLock = new object();

        int disconnected = 0;

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

                        Console.WriteLine($"Transferred bytes: {sendArgs.BytesTransferred}");

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
                    string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine($"[From Client] {recvData}");

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
