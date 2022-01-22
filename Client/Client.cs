using System;

namespace Client
{
    class Client
    {
        static void Main(string[] args)
        {
            // Ввод ip и запроса
            Console.Write("Enter server ip: ");
            string ip = Console.ReadLine();
            Console.Write("Enter the query string: ");
            string query = Console.ReadLine();
            Console.WriteLine("Connecting to the server...");

            var client = new GeneralLibrary.Client.FingerClient();
            client.SendQuery(ip, query);
            
            // Ответ сервера
            Console.WriteLine();
            Console.WriteLine("Server response:");
            Console.WriteLine();
            if (client.ServerResponse[0] != null)
                for (int i = 0; i < client.ServerResponse.Count; i++)
                {
                    Console.WriteLine($"{i + 1}.) Machine name: {client.ServerResponse[i][0]}\n" +
                        $"    User name: {client.ServerResponse[i][1]}\n" +
                        $"    User domain name: {client.ServerResponse[i][2]}\n" +
                        $"    OS version: {client.ServerResponse[i][3]}\n");
                }
            else
                Console.WriteLine("Node wasn't found!\n");

            // Завершение работы
            Console.Write("Press ENTER to exit the program...");
            Console.ReadLine();
            Console.Write("Disconnected from the server.");
        }
    }
}
