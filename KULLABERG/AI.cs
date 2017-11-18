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
        public static Thread AIThread = new Thread(new ThreadStart(MainThread));

        private static Vector2 Goal = new Vector2(1000, 350);
        public static string[] GameData = new string[15];

        private static Vector2 p1;
        private static Vector2 p2;
        private static Vector2 p3;
        private static Vector2 e1;
        private static Vector2 e2;
        private static Vector2 e3;
        private static Vector2 ball;
        private static string player = "1";

        private static float CalcDistForAtoB(Vector2 From, Vector2 To)
        {
            return Vector2.Distance(From, To);
        }

        private static float CalcDistanceToMyGoal(Vector2 PlayerPosition)
        {
            if (player == "1")
            {
                Goal.X = 0;
                return Vector2.Distance(PlayerPosition, Goal);
            }
            return Vector2.Distance(PlayerPosition, Goal);
        }

        private static Vector2 GetEnemyGoal()
        {
            if (player == "1")
            {
                return new Vector2(1000, 350);
            }
            return new Vector2(0, 350);
        }

        private static Vector2 GetNearestPlayerToBall(Vector2 P1, Vector2 P2, Vector2 P3, Vector2 Ball)
        {
            float[] Distances = new float[3];

            float min = 0;
            Vector2 Nearest = new Vector2();

            Distances[0] = Vector2.Distance(P1, Ball);
            Distances[1] = Vector2.Distance(P2, Ball);
            Distances[2] = Vector2.Distance(P3, Ball);

            for (int i = 0; i < 2; i++)
            {
                if (min > Distances[i])
                {
                    min = Distances[i];
                    Nearest = new Vector2(min);
                }
            }
            return Nearest;
        }

        private static Vector2 KickToGoal(Vector2 PlayerIn)
        {
            Vector2 Nearest = PlayerIn;
            Vector2 GoalTo = GetEnemyGoal();
            Vector2 BallPos = ball;

            Vector2.Subtract(BallPos, Nearest);

            Vector2 KickTo = BallPos;

            float AngleToBall = (float)Math.Atan2(Nearest.Y - GoalTo.Y, Nearest.X - GoalTo.X);

            if (AngleToBall > 0)
            {
                AngleToBall -= 0.20f;
                KickTo = Vector2.Transform(KickTo, Matrix4x4.CreateRotationX(AngleToBall));
                //KickTo.Y += KickTo.Y + AngleToBall;
            }
            else
            {
                AngleToBall += 0.20f;
                KickTo = Vector2.Transform(KickTo, Matrix4x4.CreateRotationX(AngleToBall));
                //KickTo = Vector2.Multiply(AngleToBall, BallPos);
            }
            return KickTo;
        }

        private static Vector2 AI_Atk(Vector2 Player)
        {
            return KickToGoal(Player);
        }

        private static Vector2 AI_Secu(Vector2 P)
        {
            int index = 1;
            float min = CalcDistForAtoB(P, e1);
            float newmin = CalcDistForAtoB(P, e2);

            if (newmin < min)
            {
                min = newmin; index = 2;
            }

            newmin = CalcDistForAtoB(P, e3);
            if (newmin < min)
            {
                min = newmin; index = 3;
            }

            switch (index)
            {
                case 1: return Vector2.Subtract(e1, P);

                case 2:
                    return Vector2.Subtract(e1, P);

                case 3:
                    return Vector2.Subtract(e1, P);

                default: return new Vector2(0, 0);
            }
        }

        private static Vector2 AI_Keep(Vector2 P)
        {
            float k = 200;

            if (CalcDistForAtoB(P, ball) < k)
            {
                return Vector2.Subtract(ball, P);
            }
            else
            {
                Vector2 poz = new Vector2();
                if (P.Y > Goal.Y + 82.5f)
                {
                    poz = new Vector2(Goal.X, Goal.Y + 82.5f);
                }
                else if (P.Y < Goal.Y - 82.5f)
                {
                    poz = new Vector2(Goal.X, Goal.Y - 82.5f);
                }
                else
                {
                    poz = new Vector2(Goal.X, ball.Y);
                }
                return Vector2.Subtract(poz, P);
            }
        }

        private static Vector2 ReScale(Vector2 v, float maxspeed)
        {
            float vMagnitude = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
            v.X = maxspeed * v.X / vMagnitude;
            v.Y = maxspeed * v.Y / vMagnitude;
            return v;
        }

        private static Vector2[] AIBrain(/*Vector2 p1, Vector2 p2, Vector2 p3, Vector2 ball, */float maxspeed)
        {
            Console.WriteLine();
            Random rand = new Random();
            Vector2[] ret = new Vector2[3];

            for (int i = 0; i < 3; ++i)
            {
                ret[i] = new Vector2(0, 0);
            }
            int gut = 0;

            #region ATK
            float p1bdist = CalcDistForAtoB(p1, ball);
            float p2bdist = CalcDistForAtoB(p2, ball);
            float p3bdist = CalcDistForAtoB(p3, ball);

            float minballdist = Math.Min(p1bdist, Math.Min(p2bdist, p3bdist));
            do
            {
                if (minballdist == p1bdist)
                {
                    if (/*ret[0] == new Vector2(0, 0) && */Math.Abs(CalcDistForAtoB(p1, GetEnemyGoal())) > Math.Abs(CalcDistForAtoB(ball, GetEnemyGoal())))
                    {
                        ret[0] = AI_Atk(p1);
                        gut++;
                        Console.WriteLine("p1 - Attacker");
                        break;
                    }
                    else
                    {
                        minballdist = Math.Min(p2bdist, p3bdist);
                    }
                }
                if (minballdist == p2bdist)
                {
                    if (/*ret[1] == new Vector2(0, 0) && */Math.Abs(CalcDistForAtoB(p2, GetEnemyGoal())) > Math.Abs(CalcDistForAtoB(ball, GetEnemyGoal())))
                    {
                        ret[1] = AI_Atk(p2);
                        gut++;
                        Console.WriteLine("p2 - Attacker");
                        break;
                    }
                    else
                    {
                        minballdist = p3bdist;
                    }
                }
                if (minballdist == p3bdist)
                {
                    if (/*ret[2] == new Vector2(0, 0) && */Math.Abs(CalcDistForAtoB(p3, GetEnemyGoal())) > Math.Abs(CalcDistForAtoB(ball, GetEnemyGoal())))
                    {
                        ret[2] = AI_Atk(p3);
                        gut++;
                        Console.WriteLine("p3 - Attacker");
                        break;
                    }
                }
            }
            while (false);
            #endregion
            /*
            #region DEF
            minballdist = Math.Min(p1bdist, Math.Min(p2bdist, p3bdist));
            do
            {
                if (minballdist == p1bdist)
                {
                    if (ret[0] == new Vector2(0, 0) && CalcDistanceToMyGoal(p1) > CalcDistanceToMyGoal(ball))
                    {
                        //ret[0] = AI_Def(p1);
                        gut++;
                        break;
                    }
                    else
                    {
                        minballdist = Math.Min(p2bdist, p3bdist);
                    }
                }
                if (minballdist == p2bdist)
                {
                    if (ret[1] == new Vector2(0, 0) && CalcDistanceToMyGoal(p2) > CalcDistanceToMyGoal(ball))
                    {
                        //ret[1] = AI_Def(p2);
                        gut++;
                        break;
                    }
                    else
                    {
                        minballdist = p3bdist;
                    }
                }
                if (minballdist == p3bdist)
                {
                    if (ret[2] == new Vector2(0, 0) && CalcDistanceToMyGoal(p3) > CalcDistanceToMyGoal(ball))
                    {
                        //ret[2] = AI_Def(p3);
                        gut++;
                        break;
                    }
                }
            }
            while (false);
            #endregion
            */
            #region Secu
            float mindistp1e = Math.Min(CalcDistForAtoB(p1, e1), Math.Min(CalcDistForAtoB(p1, e2), CalcDistForAtoB(p1, e3)));
            float mindistp2e = Math.Min(CalcDistForAtoB(p2, e1), Math.Min(CalcDistForAtoB(p2, e2), CalcDistForAtoB(p2, e3)));
            float mindistp3e = Math.Min(CalcDistForAtoB(p3, e1), Math.Min(CalcDistForAtoB(p3, e2), CalcDistForAtoB(p3, e3)));

            float mindist = Math.Min(mindistp1e, Math.Min(mindistp2e, mindistp3e));
            do
            {
                if (mindist == mindistp1e)
                {
                    if (ret[0] == new Vector2(0, 0))
                    {
                        ret[0] = AI_Secu(p1);
                        gut++;
                        Console.WriteLine("p1 - Security");
                        break;
                    }
                    else
                    {
                        mindist = Math.Min(mindistp2e, mindistp3e);
                    }
                }
                if (mindist == mindistp2e)
                {
                    if (ret[1] == new Vector2(0, 0))
                    {
                        ret[1] = AI_Secu(p2);
                        gut++;
                        Console.WriteLine("p2 - Security");
                        break;
                    }
                    else
                    {
                        mindist = mindistp3e;
                    }
                }
                if (mindist == mindistp3e)
                {
                    if (ret[2] == new Vector2(0, 0))
                    {
                        ret[2] = AI_Secu(p3);
                        gut++;
                        Console.WriteLine("p3 - Security");
                        break;
                    }
                }
            }
            while (false);
            #endregion
            #region Keep
            if (ret[0] == new Vector2(0, 0))
            {
                ret[0] = AI_Keep(p1);
                Console.WriteLine("p1 - Keeper");
            }
            if (ret[1] == new Vector2(0, 0))
            {
                ret[1] = AI_Keep(p2);
                Console.WriteLine("p2 - Keeper");
            }
            if (ret[2] == new Vector2(0, 0))
            {
                ret[2] = AI_Keep(p3);
                Console.WriteLine("p3 - Keeper");
            }
            #endregion

            for (int i = 0; i < 3; ++i)
            {
                ret[i] = ReScale(ret[i], maxspeed);
            }
            return ret;
        }

        private static Vector2 STV(string s1, string s2)
        {
            return new Vector2(float.Parse(s1), float.Parse(s2));
        }

        private static string GetFinalMsg(string[] sa)
        {
            string s = "";

            p1 = STV(sa[3], sa[4]);
            p2 = STV(sa[5], sa[6]);
            p3 = STV(sa[7], sa[8]);
            e1 = STV(sa[9], sa[10]);
            e2 = STV(sa[11], sa[12]);
            e3 = STV(sa[13], sa[14]);
            ball = STV(sa[1], sa[2]);

            Vector2[] vs = AIBrain(6.6f);

            for (int i = 0; i < 3; ++i)
            {
                s += vs[i].X + "\n" + vs[i].Y + "\n";
            }
            return s;
        }

        private static void SendMsg(HttpListenerContext context, HttpListenerResponse response, string[] sa)
        {
            string msg = GetFinalMsg(sa);
            byte[] buffer = Encoding.UTF8.GetBytes(msg);

            response.ContentLength64 = buffer.Length;
            Stream st = response.OutputStream;
            st.Write(buffer, 0, buffer.Length);

            context.Response.Close();
        }

        private static void MainThread()
        {
            HttpListener server = new HttpListener();  // this is the http server
            server.Prefixes.Add("http://192.168.1.86:11001/no_way_out/");  //we set a listening address here (localhost)
            server.Start();   // and start the server

            HttpListenerContext context = server.GetContext();
            HttpListenerResponse response = context.Response;
            string s = "";
            foreach (var c in context.Request.Url.Query)
                s += c;

            string[] s1 = s.Split('=');
            string match = s1[1].Split('&')[0];

            UDPConnection.gameid = match;
            UDPConnection.ConnectionThread.Start();

            while (true)
            {
                context = server.GetContext();
                response = context.Response;
                s = "";
                foreach (var c in context.Request.Url.Query)
                    s += c;

                s1 = s.Split('=');
                match = s1[1].Split('&')[0];
                player = s1[2];

                UDPConnection.gameid = match;
                SendMsg(context, response, GameData);
            }
        }
    }
}
