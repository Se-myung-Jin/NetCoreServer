using System.Net;
using ServerCore;

namespace DummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            PacketManager.Instance.RegisterPacketHandler();

            string host = Dns.GetHostName();
            IPHostEntry entry = Dns.GetHostEntry(host);
            IPAddress ipAddr = entry.AddressList[0];
            IPEndPoint endpoint = new IPEndPoint(ipAddr, 8994);

            Connector connector = new Connector();
            connector.Connect(endpoint, () => { return SessionManager.Instance.GenerateSession(); });

            while (true)
            {
                try
                {
                    SessionManager.Instance.SendForEach();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                Thread.Sleep(250);
            }
            
        }
    }
}
