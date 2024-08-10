using System.Net;
using ServerCore;

namespace Server
{
    class Program
    {
        public static GameRoom Room = new GameRoom();

        static void FlushRoom()
        {
            Room.Push(() => Room.Flush());
            JobTimer.Instance.Push(FlushRoom, 250);
        }

        static void Main(string[] args)
        {
            PacketManager.Instance.RegisterPacketHandler();

            string host = Dns.GetHostName();
            IPHostEntry entry = Dns.GetHostEntry(host);
            IPAddress ipAddr = entry.AddressList[0];
            IPEndPoint endpoint = new IPEndPoint(ipAddr, 8994);

            Listener listener = new Listener();
            listener.StartListening(endpoint, () => { return SessionManager.Instance.GenerateSession(); });

            Console.WriteLine("Listening...");

            FlushRoom();

            try
            {
                while (true)
                {
                    JobTimer.Instance.Flush();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
