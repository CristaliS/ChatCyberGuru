using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using Helper;

namespace ChatServer
{
    class Program
    {
        private static Dictionary<string, string> _users = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            new Thread(RunChat).Start();
            new Thread(RunUsers).Start();
            Thread.Sleep(Timeout.Infinite);
        }

        static void RunUsers()
        {
            var listener = new TcpListener(IPAddress.Any, 7777);
            listener.Start();

            while (true)
            {
                var tcpClient = listener.AcceptTcpClient();
                new Thread(HandleUsersUpdate).Start(tcpClient);
            }
        }

        static void RunChat()
        {
            var listener = new TcpListener(IPAddress.Any, 4444);
            listener.Start();

            while (true)
            {
                var tcpClient = listener.AcceptTcpClient();
                new Thread(HandleRequest).Start(tcpClient);
            }
        }

        static void HandleUsersUpdate(object client)
        {
            var tcpClient = client as TcpClient;
            using (var stream = tcpClient?.GetStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, _users);
            }
            tcpClient?.Close();
        }

        static void HandleRequest(object client)
        {
            try
            {
                var tcpClient = client as TcpClient;
                using (var stream = tcpClient?.GetStream())
                {
                    IFormatter formatter = new BinaryFormatter();
                    var msg = formatter.Deserialize(stream) as Message;

                    if (msg != null)
                    {
                        if (!_users.ContainsKey(msg.From))
                        {
                            _users.Add(msg.From, tcpClient?.Client.RemoteEndPoint.ToString());
                        }

                        string user;
                        if (!_users.TryGetValue(msg.To, out user))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(@"User Not Found!");
                            Console.ResetColor();
                            return;
                        }
                        var tcpCl = new TcpClient(user.Split(':')[0], 8888);
                        IFormatter fmt = new BinaryFormatter();
                        var strm = tcpCl.GetStream();
                        fmt.Serialize(strm, msg);

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
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
