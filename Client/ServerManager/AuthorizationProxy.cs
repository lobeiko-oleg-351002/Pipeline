using BllEntities; 
using Client.Misc;
using Client.ServerManager.Interface;
using ServerInterface;
using ServiceChannelManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.ServerManager
{
    public class AuthorizationProxy : ServerInstance, IAuthorizationManager
    {
        public AuthorizationProxy(IBusinessService server) : base(server)
        {
        }

        public BllUser ConnectToServerAndSignIn(string ip, IClientCallBack clientCallBack, BllUser user)
        {           
            try
            {
                ConnectToServer();
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
            catch(UserIsNullException ex)
            {
                throw ex;
            }
            catch(Exception ex)
            {
                throw new ConnectionFailedException();
            }
        }
    }
}
