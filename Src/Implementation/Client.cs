using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using ITCLIENT.Src.Abstract;

namespace ITCLIENT.Src.Implementation
{
    public class Client : IClient
    {
        public bool CanReceive { get; set; } 

        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        
        private const string ExeFileConfig = @"../Configs/ExeConfig.txt";

        public Client(TcpClient client) { 
            _client = client;
            _stream = client.GetStream();
            CanReceive = true;
        }  
      
        public void SendMessage()
        {
            string message = Environment.UserName;
            byte[] data = Encoding.Unicode.GetBytes(message);
            _stream.Write(data, 0, data.Length);
        }
 
        public void TryReceiveMessage()
        {
            while (CanReceive) 
            {
                try
                {
                    ReceiveMessage();
                }   
                catch
                {
                    Disconnect(new Exception());   
                }
            }
        }

        public void Disconnect(Exception nullValueError)
        {
            if (_stream == null || _client == null)
                MessageBox.Show(nullValueError.Message);
            
            _stream?.Close();
            _client?.Close();
        }
        
        private void ReceiveMessage()
        {
            string message = GetStreamData();

            if (!File.ReadAllText(ExeFileConfig).Contains(message))
                return;

            CombinePathAndMessage(message);

            if (message != "Shutdown")
                return;
            
            Disconnect(new Exception());
            Process.Start("shutdown", "/s /t 0");
        }
        
        private string GetStreamData()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();

            while (_stream.DataAvailable)
            {
                var bytes = _stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }

            return builder.ToString();
        }

        
        private void CombinePathAndMessage(string message)
        {
            var combinedPath = Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.CommonDesktopDirectory),
                message);

            if (!File.Exists(combinedPath))
                return;
            
            Process.Start(combinedPath);
        }
    }
}
