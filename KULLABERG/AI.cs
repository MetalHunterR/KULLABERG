using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;


namespace KULLABERG
{
    class AI
    {
        public class Vector
        {
            public double x, y;

            public Vector(double X, double Y)
            {
                x = X;
                y = Y;
            }

            public static Vector operator +(Vector a, Vector b)
            {
                return new Vector(a.x + b.x, a.y + b.y);
            }

            public static Vector operator +(Vector a, int b)
            {
                return new Vector(a.x + b, a.y + b);
            }

            public static Vector operator -(Vector a, Vector b)
            {
                return new Vector(a.x - b.x, a.y - b.y);
            }
            public static double operator %(Vector a, Vector b)
            {
                return Math.Sqrt(Math.Pow(a.x - b.x, 2.0) + Math.Pow(a.y - b.y, 2.0));
            }

            public static Vector operator /(Vector a, double b)
            {
                return new Vector(a.x / b, a.y / b);
            }

            public static Vector operator *(Vector a, double b)
            {
                return new Vector(a.x * b, a.y * b);
            }
        }

        public static Thread thread = new Thread(new ThreadStart(Threadd));

        public static int kapux = 1000;
        public static string[] getitpls = new string[15];

        static Vector p1;
        static Vector p2;
        static Vector p3;
        static Vector ball;

        public static Vector CalcDirAToB(Vector a, Vector b)
        {
            Vector ret = a - b;
            //double max = Math.Min(ret.x, ret.y);
            return ret /* max*/;
        }

        public static Vector CalcDistanceToGoal()
        {
            Vector GoalPoint = new Vector(kapux, 350);

            //double dist = p1 % GoalPoint;
            //Console.WriteLine(dist);
            return new Vector(0, 0);
        }

        public static int RandNeg()
        {
            Random rand = new Random();
            if (rand.Next(2) > 0) return -1;
            return 1;
        }

        public static Vector[] AIi(Vector p1, Vector p2, Vector p3, Vector ball, double maxspeed)
        {
            Random rand = new Random();
            Vector[] ret = new Vector[3];

            for (int i = 0; i < 3; ++i)
            {
                ret[i] = new Vector(0, 0);
            }

            if (Math.Max(Math.Abs(kapux - p1.x), Math.Abs(kapux - p2.x)) == Math.Abs(kapux - p1.x))
            {
                Vector temp = new Vector(ball.x - 15, ball.y);
                ret[0] = CalcDirAToB(p1, temp) * maxspeed;

                temp = new Vector(Math.Abs(kapux - 1000), 350);
                ret[1] = CalcDirAToB(p2, temp) * maxspeed;
            }
            else
            {
                Vector temp = new Vector(ball.x - 15, ball.y);
                ret[1] = CalcDirAToB(p2, temp) * maxspeed;

                temp = new Vector(Math.Abs(kapux - 1000), 350);
                ret[0] = CalcDirAToB(p1, temp) * maxspeed;

            }
            ret[2] = CalcDirAToB(p3, ball + (rand.Next(2, 10) * 10 * RandNeg())) * maxspeed;
            CalcDistanceToGoal();

            return ret;
        }

        public static Vector[] AI2(Vector p1, Vector p2, Vector p3, Vector ball, double maxspeed)
        {
            Vector[] ret = new Vector[3];

            ret[0] = ball - p1;
            ret[1] = ball - p2;
            ret[2] = ball - p3;

            return ret;
        }

        public static Vector STV(string s1, string s2)
        {
            return new Vector(double.Parse(s1), double.Parse(s2));
        }

        public static string GetFinalMsg(string[] sa)
        {
            //Console.WriteLine("curr: " + UDPConnection.gameid);
            string s = "";

            /*
            p1 = STV(sa[3], sa[4]);
            p2 = STV(sa[5], sa[6]);
            p3 = STV(sa[7], sa[8]);
            ball = STV(sa[1], sa[2]);
            */

            CalcDistanceToGoal();
            Vector[] vs = AIi(STV(sa[3], sa[4]), STV(sa[5], sa[6]), STV(sa[7], sa[8]), STV(sa[1], sa[2]), 4);

            for (int i = 0; i < 3; ++i)
            {
                s += vs[i].x + "\n" + vs[i].y + "\n";
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
