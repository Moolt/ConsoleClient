
namespace UnityConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleClient client;

            if(args.Length == 2)
            {
                var ip = args[0];
                int.TryParse(args[1], out int port);
                client = new ConsoleClient(ip, port);
            }
            else
            {
                client = new ConsoleClient();
            }

            client.StartClient().ConfigureAwait(true);
        }        
    }
}
