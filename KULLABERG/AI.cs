using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;
using System.Numerics;

namespace KULLABERG
{
    class AI
    {
        public static Thread thread = new Thread(new ThreadStart(Threadd));

        public static int kapux = 1000;
        public static string[] getitpls = new string[15];

        static Vector2 p1;
        static Vector2 p2;
        static Vector2 p3;
        static Vector2 ball;

        public static Vector2 CalcDirAToB(Vector2 a, Vector2 b)
        {
            Vector2 ret = a - b;
            double max = Math.Max(ret.X, ret.Y);
            return Vector2.Divide(a, b);
        }

        public static Vector2 CalcDistanceToGoal()
        {
            Vector2 GoalPoint = new Vector2(kapux, 350);

            //double dist = p1 % GoalPoint;
            //Console.WriteLine(dist);
            return new Vector2(0, 0);
        }

        public static int RandNeg()
        {
            Random rand = new Random();
            if (rand.Next(2) > 0) return -1;
            return 1;
        }

        public static Vector2[] AIi(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 ball, float maxspeed)
        {
            Random rand = new Random();
            Vector2[] ret = new Vector2[3];

            for (int i = 0; i < 3; ++i)
            {
                ret[i] = new Vector2(0, 0);
            }

            if (Math.Max(Math.Abs(kapux - p1.X), Math.Abs(kapux - p2.X)) == Math.Abs(kapux - p1.X))
            {
                Vector2 temp = new Vector2(ball.X - 15, ball.Y);
                ret[0] = Vector2.Multiply(CalcDirAToB(p1, temp), maxspeed);//CalcDirAToB(p1, temp) * maxspeed;

                temp = new Vector2(Math.Abs(kapux - 1000), 350);
                ret[1] = CalcDirAToB(p2, temp) * maxspeed;
            }
            else
            {
                Vector2 temp = new Vector2(ball.X - 15, ball.Y);
                ret[1] = CalcDirAToB(p2, temp) * maxspeed;

                temp = new Vector2(Math.Abs(kapux - 1000), 350);
                ret[0] = CalcDirAToB(p1, temp) * maxspeed;

            }
            ret[2] = CalcDirAToB(p3, Vector2.Multiply(ball, (rand.Next(2, 10) * 10 * RandNeg())) * maxspeed);
            CalcDistanceToGoal();

            return ret;
        }

        public static Vector2[] AI2(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 ball, double maxspeed)
        {
            Vector2[] ret = new Vector2[3];

            ret[0] = p1-ball;
            ret[1] = p2-ball;
            ret[2] = p3-ball;

            return ret;
        }

        public static Vector2 STV(string s1, string s2)
        {
            return new Vector2(float.Parse(s1), float.Parse(s2));
        }

        public static string GetFinalMsg(string[] sa)
        {
            //Console.WriteLine("curr: " + UDPConnection.gameid);
            string s = "";

            p1 = STV(sa[3], sa[4]);
            p2 = STV(sa[5], sa[6]);
            p3 = STV(sa[7], sa[8]);
            ball = STV(sa[1], sa[2]);

            CalcDistanceToGoal();
            Vector2[] vs = AI2(STV(sa[3], sa[4]), STV(sa[5], sa[6]), STV(sa[7], sa[8]), STV(sa[1], sa[2]), 4);

            for (int i = 0; i < 3; ++i)
            {
                s += vs[i].X + "\n" + vs[i].Y + "\n";
            }
            return s;
        }

        public static void sendmsg(HttpListenerContext context, HttpListenerResponse response, string[] sa)
        {
            string msg = GetFinalMsg(sa);

            byte[] buffer = Encoding.UTF8.GetBytes(msg);

            response.ContentLength64 = buffer.Length;
            Stream st = response.OutputStream;
            st.Write(buffer, 0, buffer.Length);

            context.Response.Close();
        }

        static void Threadd()
        {
            HttpListener server = new HttpListener();  // this is the http server
            server.Prefixes.Add("http://192.168.1.86:11001/no_way_out/");  //we set a listening address here (localhost)

            server.Start();   // and start the server

            HttpListenerContext context = server.GetContext();
            HttpListenerResponse response = context.Response;
            //Console.WriteLine(response.Headers);
            string s = "";
            foreach (var c in context.Request.Url.Query)
                s += c;
            //Console.WriteLine(s);
            string[] s1 = s.Split('=');
            string match = s1[1].Split('&')[0];
            string player = s1[2];
            if (player == "1")
                kapux = 1000;
            else
                kapux = 0;

            UDPConnection.gameid = match;
            UDPConnection.ConnectionThread.Start();

            while (true)
            {
                context = server.GetContext();
                response = context.Response;
                //Console.WriteLine(response.Headers);
                s = "";
                foreach (var c in context.Request.Url.Query)
                    s += c;
                //Console.WriteLine(s);
                s1 = s.Split('=');
                match = s1[1].Split('&')[0];
                player = s1[2];
                if (player == "1")
                    kapux = 1000;
                else
                    kapux = 0;
                //Console.WriteLine(match + " " + player);
                UDPConnection.gameid = match;
                sendmsg(context, response, getitpls);
            }
        }
    }
}
