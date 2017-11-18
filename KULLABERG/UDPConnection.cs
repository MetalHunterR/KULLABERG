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

                if (Listener.Available != 0)
                {
                    Console.WriteLine("We received data");
                }
                AI.thread.Start();

                while (!done)
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();

                    ReceivedByteAray = Listener.Receive(ref EndPoint);
                    Console.WriteLine("Received a broadcast from {0}", EndPoint.ToString());
                    ReceivedData = Encoding.ASCII.GetString(ReceivedByteAray, 0, ReceivedByteAray.Length);

                    string[] games = ReceivedData.Replace("????", "").Split('|');
                    for (int i = 0; i < games.Length; i++)
                    {
                        if (games[i].Split('|')[0] == "69")
                        {
                            string[] s = new string[15];
                            for (int j = i; j < i + 15; j++)
                            {
                                s[j - i] = games[j];
                                Console.WriteLine(games[j]);
                            }
                            AI.getitpls = s;
                            break;
                        }
                    }

                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    Console.WriteLine("Runtime: " + elapsedMs);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
