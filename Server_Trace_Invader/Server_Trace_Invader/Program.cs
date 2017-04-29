using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace SerVEr_FInaLe
{
    class Program
    {
        static void Main(string[] args)
        {
            
            string command = "";
            Server s = new Server();
            s.Start();
            while (true)
            {
                command = Console.ReadLine();
                switch (command.ToLower())
                {
                    case "stop":
                        s.StopServer();
                        break;
                }
            }
        }
    }
}