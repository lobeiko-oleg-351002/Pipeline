using BllEntities;
using Client.ServerManager.Interface;
using ServerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.ServerManager
{
    public class GroupGetter : ServerInstance, IGroupGetter
    {
        public GroupGetter(IBusinessService server) : base(server)
        {
        }

        public List<BllGroup> GetAllGroups()
        {
            try
            {
                return server.GetAllGroups();
            }
            catch
            {
                throw new ConnectionFailedException();
            }
        }
    }
}
