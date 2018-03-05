using ServerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceChannelManager
{
    public interface IServerInstance
    {
        void ConnectToServer();
        void PingServer();
        bool IsConnected();
    }
}
