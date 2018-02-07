using BllEntities;
using ServerInterface;
using ServiceChannelManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public class ServerConnectionController
    {
        private IBusinessService server;

        public bool IsServerOnline { get; private set; }

        public ServerConnectionController(string ip, IClientCallBack clientCallBack)
        {
            ConnectToServer(ip, clientCallBack);
        }

        public BllUser ConnectToServerAndSignIn(string ip, IClientCallBack clientCallBack, BllUser user)
        {
            server = GetServerInstanceUsingIp(ip, clientCallBack);
            try
            {
                user = SignIn(user);
                IsServerOnline = true;
                return user;
            }
            catch (ConnectionFailedException ex)
            {
                IsServerOnline = false;
                throw ex;
            }
            catch (UserIsNullException ex)
            {
                throw ex;
            }
        }

        private IBusinessService GetServerInstanceUsingIp(string ip, IClientCallBack clientCallBack)
        {
            return ServiceChannelMakerSingleton.Instance.GetServerMethods(clientCallBack, ip);
        }

        public BllUser SignIn(BllUser User)
        {
            try
            {
                User = server.SignIn(User.Login, User.Password);
                if (User == null)
                {
                    throw new UserIsNullException();
                }
                return User;
            }
            catch
            {
                throw new ConnectionFailedException();
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
            catch (ConnectionFailedException ex)
            {
                IsServerOnline = false;
                throw ex;
            }
            catch (UserIsNullException ex)
            {
                throw ex;
            }
        }
    }
}
