using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace UnityConsoleClient
{
    public class ConsoleClient
    {
        private bool connectionHasBeenEstablishedOnce = false;
        private bool openedWithExplicitParameters;
        private string _ip;
        private int _port;

        public ConsoleClient(string ip, int port)
        {
            _ip = ip;
            _port = port;
            openedWithExplicitParameters = true;
        }

        public ConsoleClient()
        {
            openedWithExplicitParameters = false;
        }

        public void StartClient()
        {
            bool exited = false;

            while (!exited)
            {
                if(!connectionHasBeenEstablishedOnce && !openedWithExplicitParameters)
                {
                    AskForInfo();
                }

                try
                {
                    exited = HandleServerCommunication();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);

                    if(connectionHasBeenEstablishedOnce)
                    {
                        Console.WriteLine("Connection to server lost.");
                    }

                    if (openedWithExplicitParameters)
                    {
                        if(!connectionHasBeenEstablishedOnce)
                        {
                            Console.WriteLine("Could not connect to the server. Please check the ip and port.");
                        }
                        Console.ReadKey();
                        exited = true;
                    }
                    else if(connectionHasBeenEstablishedOnce)
                    {
                        Thread.Sleep(500);
                        Console.WriteLine("Attempting to reconnect...");
                        Thread.Sleep(5000);
                    }
                }
            }
        }

        private void AskForInfo()
        {
            Console.Write("IP: ");
            _ip = Console.ReadLine();
            Console.Write("Port: ");
            int.TryParse(Console.ReadLine(), out _port);
        }

        private bool HandleServerCommunication()
        {
            Console.WriteLine("Attempting to connect to server...");

            using (var client = new TcpClient(_ip, _port))
            {
                connectionHasBeenEstablishedOnce = true;
                Console.WriteLine("Server connection established.");
                Console.WriteLine();

                client.ReceiveTimeout = 1000;
                client.SendTimeout = 1000;

                using (var writer = new StreamWriter(client.GetStream()))
                using (var reader = new StreamReader(client.GetStream()))
                {
                    //Print already existing output
                    HandleResponse(reader);

                    var input = string.Empty;
                    while (input != "exit" && client.Connected)
                    {
                        Console.Write("> ");
                        input = Console.ReadLine();
                        writer.WriteLine(input);
                        writer.Flush();

                        HandleResponse(reader);
                    }
                }
            }
            return true;
        }

        private void HandleResponse(StreamReader reader)
        {
            var response = ReadResponse(reader);
            PrintResponse(response);
        }

        private string ReadResponse(StreamReader reader)
        {
            var respose = string.Empty;
            int character;

            while ((character = reader.Read()) != (int)'\0')
            {
                if (character != -1)
                {
                    respose += (char)character;
                }
            }

            return respose;
        }

        private void PrintResponse(string response)
        {
            response = response.Replace("\r", string.Empty);
            var lines = response.Split('\n');

            foreach (string line in lines)
            {
                if (ColorCodes.IsColorCoded(line))
                {
                    var strippedLine = line;
                    Console.ForegroundColor = ColorCodes.ExtractColor(ref strippedLine);
                    Console.WriteLine(strippedLine);
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine(line);
                }
            }
        }
    }
}
