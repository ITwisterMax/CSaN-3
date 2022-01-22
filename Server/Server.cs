using System;

namespace Server
{
    class Server
    {
        static void Main(string[] args)
        {
            // Ввод ip и запроса
            Console.Write("Enter ip: ");
            string ip = Console.ReadLine();

            Console.WriteLine("Server is running!");
            Console.WriteLine("Waiting for user connection...");

            var server = new GeneralLibrary.Server.FingerServer();
            server.Start(ip);
        }
    }
}
