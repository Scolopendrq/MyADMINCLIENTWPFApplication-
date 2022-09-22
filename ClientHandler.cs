using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ITCLIENT
{
    public class MYClientHandler
    {

        TcpClient client;

        NetworkStream stream;

        public bool keepreceive;
        public MYClientHandler(TcpClient _client) { 
            this.client = _client;
            this.stream = _client.GetStream();
            this.keepreceive = true;
        }  
        // отправка сообщений
        public void SendMessage()
        {
            

          
                string message = Environment.UserName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
           
        }
        // получение сообщений
        public void ReceiveMessage()
        {
            while (keepreceive)
            {
                try
                {
                    byte[] data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    //вывод сообщения
                    if (File.ReadAllText("ExeConfig.txt").Contains(message))
                    {
                      
                        if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory), message))){
                            Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) , message));
                        } // файл существует?
                             // запустить
                    }
                    if (message == "Shutdown")
                    {
                        Disconnect();
                        Process.Start("shutdown", "/s /t 0");
                    }
                    
                }
                catch
                {
                    
                    Disconnect();
                }
            }
        }

        public void Disconnect()
        {
            try
            {
                if (stream != null)
                    stream.Close();//отключение потока
                if (client != null)
                    client.Close();//отключение клиента
              
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            
        }
        


        }
}
