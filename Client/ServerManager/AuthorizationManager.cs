using BllEntities;
using Client.ServerManager.Interface;
using ServerInterface;
using ServiceChannelManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.ServerManager
{
    public class AuthorizationManager : ServerInstance, IAuthorizationManager
    {
        public AuthorizationManager(string ip, IClientCallBack clientCallBack) : base(ip, clientCallBack)
        {
        }

        public BllUser ConnectToServerAndSignIn(string ip, IClientCallBack clientCallBack, BllUser user)
        {           
            try
            {
                ConnectToServer(ip, clientCallBack);
                return SignIn(user);
            }
            catch (ConnectionFailedException ex)
            {
                throw ex;
            }
            catch (UserIsNullException ex)
            {
                throw ex;
            }
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
    }
}
