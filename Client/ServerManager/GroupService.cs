using BllEntities;
using Client.ServerManager.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.ServerManager
{
    public class GroupService
    {
        ServerInstance serverInstance;

        public GroupService(ServerInstance serverInstance)
        {
            this.serverInstance = serverInstance; 
        }

        public List<BllGroup> GetGroups()
        {
            while (true)
            {
                try
                {
                    IGroupGetter gg = new GroupGetter(serverInstance.server);
                    return gg.GetAllGroups();
                }
                catch
                {
                    serverInstance.ConnectToServer();
                }
            }
        }
    }
}
