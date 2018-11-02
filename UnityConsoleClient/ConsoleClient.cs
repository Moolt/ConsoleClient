using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace UnityConsoleClient
{
    public class ConsoleClient
    {
        private string _ip;
        private int _port;

        public ConsoleClient(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        public void StartClient()
        {
            try
            {
                HandleServerCommunication();
                Console.WriteLine("Connection to server lost...");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
        }

        private void HandleServerCommunication()
        {
            Console.WriteLine("Attempting to connect to server...");
            var client = new TcpClient(_ip, _port);
            Console.WriteLine("Server connection established.");
            Console.WriteLine();

            var reader = new StreamReader(client.GetStream());
            var writer = new StreamWriter(client.GetStream());

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
            reader.Close();
            writer.Close();            
            client.Close();
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
                if(ColorCodes.IsColorCoded(line))
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
