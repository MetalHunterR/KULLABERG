using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KULLABERG
{
    class Program
    {
        static void Main(string[] args)
        {
            UDPConnection Connection = new UDPConnection();
            Connection.StartListening();
            Console.Read();
        }
    }
}
