using System;

namespace UnityConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string ip;
            int port;

            if(args.Length == 2)
            {
                ip = args[0];
                int.TryParse(args[1], out port);
            }
            else
            {
                Console.Write("IP: ");
                ip = Console.ReadLine();
                Console.Write("Port: ");
                int.TryParse(Console.ReadLine(), out port);
            }
            var consoleClient = new ConsoleClient(ip, port);
            consoleClient.StartClient();
        }        
    }
}
