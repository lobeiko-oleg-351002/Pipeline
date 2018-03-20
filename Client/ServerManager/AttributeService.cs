using BllEntities;
using Client.ServerManager.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.ServerManager
{
    public class AttributeService
    {
        ServerInstance serverInstance;

        public AttributeService(ServerInstance serverInstance)
        {
            this.serverInstance = serverInstance;
        }

        public List<BllAttribute> GetAttributes()
        {
            while (true)
            {
                try
                {
                    IAttributeGetter ag = new AttributeGetter(serverInstance.server);
                    return ag.GetAllAttributes();
                }
                catch
                {
                    serverInstance.ConnectToServer();
                }
            }
        }
    }
}
