using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace KULLABERG
{
    class UDPConnection
    {
        private const int ListenPort = 12010;

        public void StartListening()
        {
            bool done = false;
            UdpClient Listener = new UdpClient(ListenPort);
            IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.126"), ListenPort);

            string ReceivedData;
            byte[] ReceivedByteAray;

            try
            {
                Console.WriteLine("Waiting for broadcast");

                AI.thread.Start();
                while (!done)
                {
                    ReceivedByteAray = Listener.Receive(ref EndPoint);
                    ReceivedData = Encoding.ASCII.GetString(ReceivedByteAray, 0, ReceivedByteAray.Length);

                    string[] games = ReceivedData.Replace("????", "¤").Split('¤');
                    foreach (string g in games)
                    {
                        string[] tmp = g.Split('|');
                        if (tmp[0] == "69")
                        {
                            AI.getitpls = tmp;
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
