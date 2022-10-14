using System;

namespace ITCLIENT.Src.Abstract
{
    public interface IClient
    {
        bool CanReceive { get; set; }

        void TryReceiveMessage();
        void SendMessage();
        void Disconnect(Exception nullValueError); 
    }
}