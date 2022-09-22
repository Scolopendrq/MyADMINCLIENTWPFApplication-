using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Hardcodet.Wpf.TaskbarNotification;
using Hardcodet.Wpf.TaskbarNotification.Interop;

namespace ITCLIENT
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public TcpClient _tcpclient;
        public MYClientHandler _client;

        public string _host;
        public int _port = 8888;

        public MainWindow()
        {
            InitializeComponent();
            mYb.Background = Brushes.Wheat;
            text.Text = File.ReadAllText("IpServerConfig.txt");
        }

        private void TaskbarIcon_MouseDown(object sender, RoutedEventArgs e)
        {
            this.Show();
        }

        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
            TaskBar.Visibility = Visibility.Visible;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TaskBar.Dispose();
            Environment.Exit(0);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _tcpclient = new TcpClient();
            Thread receiveThread= null;


            if (mYb.Content.ToString() == "Connect")
            {
                try
                {
                    
                    _host = text.Text;


                    _tcpclient.Connect(_host, _port); //подключение клиента
                    _client = new MYClientHandler(this._tcpclient);
                    _client.keepreceive = true;
                    _client.SendMessage();
                    // запускаем новый поток для получения данных
                    receiveThread = new Thread(new ThreadStart(_client.ReceiveMessage));
                    receiveThread.Start(); //старт потока
                    mYb.Content = "Disconnect";
                    mYb.Background = Brushes.GreenYellow;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (mYb.Content == "Disconnect") {

                try
                {
                    _client.keepreceive = false;
                    _client.Disconnect();
                    if (receiveThread!=null)
                    {
                        receiveThread.Abort();
                    }
                    
                    mYb.Background = Brushes.Wheat;
                    mYb.Content = "Connect";
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
                    
            }
        }

        private void High_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
