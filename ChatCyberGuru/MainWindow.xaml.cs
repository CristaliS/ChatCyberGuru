using System;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Documents;
using Helper;

namespace ChatCyberGuru
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        //192.168.2.94
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            var text = new TextRange(rtbMessage.Document.ContentStart, rtbMessage.Document.ContentEnd).Text;
            var from = "Stepan";
            var to = "Tatiana";
            var msg = new Message(text, from, to, DateTime.Now);

            var tcpClient = new TcpClient(@"127.0.0.1", 4444);
            IFormatter formatter = new BinaryFormatter();
            var stream = tcpClient.GetStream();
            formatter.Serialize(stream, msg);
        }
    }
}
