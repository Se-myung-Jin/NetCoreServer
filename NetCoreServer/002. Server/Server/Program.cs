using System.Net;
using System.Text;
using ServerCore;

namespace Server
{

    public class GameSession : Session
    {
        public override void OnConnected(EndPoint e)
        {
            Console.WriteLine($"OnConnected : {e}");

            byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
            Send(sendBuff);
            Thread.Sleep(1000);
            Disconnect();
        }

        public override void OnDisconnected(EndPoint e)
        {
            Console.WriteLine($"OnDisconnected : {e}");
        }

        public override void OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Client] {recvData}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry entry = Dns.GetHostEntry(host);
            IPAddress ipAddr = entry.AddressList[0];
            IPEndPoint endpoint = new IPEndPoint(ipAddr, 8994);

            Listener listener = new Listener();
            listener.StartListening(endpoint, () => { return new GameSession(); });

            Console.WriteLine("Listening...");

            try
            {
                while (true)
                {
                    ;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
