using Client.ServerManager.Interface;
using ServerInterface;
using ServiceChannelManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Client.ServerManager
{
    public class ServerInstance : IServerInstance 
    {
        public IBusinessService server { get; private set; }

        protected bool IsServerOnline;

        private string ip;
        private IClientCallBack clientCallBack;

        public ServerInstance(string ip, IClientCallBack clientCallBack)
        {
            this.ip = ip;
            this.clientCallBack = clientCallBack;
        }

        public ServerInstance(IBusinessService server)
        {
            this.server = server;
        }

        public void ConnectToServer()
        {
            server = GetServerInstanceUsingIp(ip, clientCallBack);
            try
            {
                server.PingServer();
                IsServerOnline = true;
            }
            catch (EndpointNotFoundException ex)
            {
                IsServerOnline = false;
                throw ex;
            }
            catch
            {
                IsServerOnline = false;
                throw new ConnectionFailedException();
            }
        }

        private IBusinessService GetServerInstanceUsingIp(string ip, IClientCallBack clientCallBack)
        {
            return ServiceChannelMakerSingleton.Instance.GetServerMethods(clientCallBack, ip);
        }

        public bool IsConnected()
        {
            return IsServerOnline;
        }

        public void PingServer()
        {
            try
            {
                server.PingServer();
            }
            catch
            {
                throw new ConnectionFailedException();
            }
        }
    }
}
