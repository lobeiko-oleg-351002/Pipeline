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
        protected IBusinessService server;

        protected bool IsServerOnline;

        private string ip;
        private IClientCallBack clientCallBack;

        public ServerInstance(string ip, IClientCallBack clientCallBack)
        {
            this.ip = ip;
            this.clientCallBack = clientCallBack;
            try
            {
                ConnectToServer(ip, clientCallBack);
            }
            catch (ConnectionFailedException ex)
            {
                throw ex;
            }
        }

        public void ConnectToServer(string ip, IClientCallBack clientCallBack)
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

        public bool IsThereConnection()
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

        public void ReconnectToServer()
        {
            ConnectToServer(ip, clientCallBack);
        }
    }
}
