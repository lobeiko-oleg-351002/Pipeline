using ServerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.ServerManager.Interface
{
    public interface IServerInstance
    {
        void ConnectToServer(string ip, IClientCallBack clientCallBack);
        void ReconnectToServer();
        void PingServer();
        bool IsThereConnection();
    }
}
