using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ITCLIENT.Src.Abstract;
using ITCLIENT.Src.Implementation;

namespace ITCLIENT.Forms.MainWindowForm
{
    public partial class MainWindow : Window
    {
        private TcpClient _tcpClient;
        private IClient _client;

        private string _host;
        
        private const int Port = 8888;
        private const string ServerIpConfigPath = @"../../Configs/IpServerConfig.txt";

        public MainWindow()
        {
            InitializeComponent();
            mYb.Background = Brushes.Wheat;
            text.Text = File.ReadAllText(ServerIpConfigPath);
        }

        private void TaskbarIcon_MouseDown(object sender, RoutedEventArgs e)
        {
            Show();
        }

        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            Hide();
            TaskBar.Visibility = Visibility.Visible;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TaskBar.Dispose();
            Environment.Exit(0);
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (mYb.Content.ToString())
            {
                case "Connect": 
                    _tcpClient = new TcpClient();
                    _host = text.Text;
                    
                    SendMessageToUser();
                    StartNewThread();
                
                    ChangeMyb("Disconnect", Brushes.GreenYellow);
                    break;
                
                case "Disconnect": 
                    _client.CanReceive = false;
                    _client.Disconnect(new Exception());

                    ChangeMyb("Connect", Brushes.Wheat);
                    break;
            }
        }

        private void ChangeMyb(string content, Brush bgColor)
        {
            mYb.Content = content;
            mYb.Background = bgColor;
        }

        private void StartNewThread()
        {
            var receiveThread = new Thread(_client.TryReceiveMessage);
            receiveThread.Start();
        }

        private void SendMessageToUser()
        {
            _tcpClient.Connect(_host, Port);
            _client = new Client(_tcpClient);
            _client.CanReceive = true;
            _client.SendMessage();
        }

        private void High_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;
            
            DragMove();
        }
    }
}
