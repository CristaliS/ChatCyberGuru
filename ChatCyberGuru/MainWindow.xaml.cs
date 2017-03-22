using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using Helper;

namespace ChatCyberGuru
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            new Thread(UpdateUsers).Start();
            new Thread(UpdateChat).Start();
        }

        private void UpdateChat()
        {
            var listener = new TcpListener(IPAddress.Any, 8888);
            listener.Start();

            while (true)
            {
                var tcpClient = listener.AcceptTcpClient();
                new Thread(t =>
                {
                    var client = t as TcpClient;
                    using (var stream = client?.GetStream())
                    {
                        IFormatter formatter = new BinaryFormatter();
                        var msg = formatter.Deserialize(stream) as Message;

                        if (msg != null)
                        {
                            rtbChat.Dispatcher.Invoke(() => rtbChat.AppendText($"{msg.TimeStamp}: {msg.From} \t{msg.Text}\n\r"));
                        }
                    }

                }).Start(tcpClient);
                Thread.Sleep(1000);
            }
        }

        private void UpdateUsers()
        {
            while (true)
            {
                try
                {
                    var tcpClient = new TcpClient(@"192.168.2.94", 7777);
                    var stream = tcpClient.GetStream();
                    IFormatter formatter = new BinaryFormatter();
                    var users = formatter.Deserialize(stream) as Dictionary<string, string>;
                    listUsers.Dispatcher.Invoke(() => listUsers.ItemsSource = users?.Select(user => user.Key));
                    stream.Close();
                    tcpClient.Close();
                    Thread.Sleep(500);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        //192.168.2.94
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            var text = new TextRange(rtbMessage.Document.ContentStart, rtbMessage.Document.ContentEnd).Text;
            var from = tbFrom.Text;
            var to = tbTo.Text;
            var msg = new Message(text, from, to, DateTime.Now);

            var tcpClient = new TcpClient(@"192.168.2.94", 4444);
            IFormatter formatter = new BinaryFormatter();
            var stream = tcpClient.GetStream();
            formatter.Serialize(stream, msg);
            stream.Dispose();
            tcpClient.Dispose();
            rtbMessage.Document.Blocks.Clear();
        }
    }
}
