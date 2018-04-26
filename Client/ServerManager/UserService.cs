using BllEntities;
using Client.ServerManager.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.ServerManager
{
    public class UserService
    {
        ServerInstance serverInstance;

        public UserService(ServerInstance serverInstance)
        {
            this.serverInstance = serverInstance;
        }

        public List<BllUser> GetUsersByGroup(BllGroup group)
        {
            while (true)
            {
                try
                {
                    IUserGetter ug = new UserGetter(serverInstance.server);
                    return ug.GetUsersByGroupAndSignInDateRange(group, int.Parse(Properties.Resources.PERMISSIBLE_DATE_RANGE_IN_DAYS));
                }
                catch
                {
                    serverInstance.ConnectToServer();
                }
            }
        }

        public List<BllUser> GetApprovers()
        {
            while (true)
            {
                try
                {
                    IUserGetter ug = new UserGetter(serverInstance.server);
                    return ug.GetApprovers();
                }
                catch
                {
                    serverInstance.ConnectToServer();
                }
            }
        }

        public List<BllUser> GetReconcilers()
        {
            while (true)
            {
                try
                {
                    IUserGetter ug = new UserGetter(serverInstance.server);
                    return ug.GetReconcilers();
                }
                catch
                {
                    serverInstance.ConnectToServer();
                }
            }
        }
    }
}
