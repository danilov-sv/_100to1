using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolicySocketServices
{
    class Program
    {
        static void Main(string[] args)
        {
            PolicySocketServer server = new PolicySocketServer();
            server.StartSocketServer();

            Console.Read();
        }
    }
}
