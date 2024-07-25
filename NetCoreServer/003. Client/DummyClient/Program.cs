using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;

namespace DummyClient
{
    class Program
    {
        public class GameSession : Session
        {
            public override void OnConnected(EndPoint e)
            {
                Console.WriteLine($"OnConnected : {e}");


                for (int i = 0; i < 5; i++)
                {
                    byte[] sendBuff = Encoding.UTF8.GetBytes($"Hello World! {i}");
                    Send(sendBuff);
                }
            }

            public override void OnDisconnected(EndPoint e)
            {
                Console.WriteLine($"OnDisconnected : {e}");
            }

            public override void OnRecv(ArraySegment<byte> buffer)
            {
                string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
                Console.WriteLine($"[From Server] {recvData}");
            }

            public override void OnSend(int numOfBytes)
            {
                Console.WriteLine($"Transferred bytes: {numOfBytes}");
            }
        }

        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry entry = Dns.GetHostEntry(host);
            IPAddress ipAddr = entry.AddressList[0];
            IPEndPoint endpoint = new IPEndPoint(ipAddr, 8994);

            Connector connector = new Connector();
            connector.Connect(endpoint, () => { return new GameSession(); });

            while (true)
            {
                try
                {

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                Thread.Sleep(100);
            }
            
        }
    }
}
