using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Helper;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var listener = new TcpListener(IPAddress.Any, 4444);
            listener.Start();

            while (true)
            {
                var tcpClient = listener.AcceptTcpClient();
                new Thread(HandleRequest).Start(tcpClient);
            }
        }

        static void HandleRequest(object client)
        {
            var tcpClient = client as TcpClient;
            using (var stream = tcpClient?.GetStream())
            {
                IFormatter formatter = new BinaryFormatter();
                var msg = formatter.Deserialize(stream) as Message;

                if (msg != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{msg.TimeStamp}: {msg.From} \t{msg.Text}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(@"Deserialization ERROR!");
                }
                Console.ResetColor();
            }
            tcpClient?.Close();
        }
    }
}
