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
        public GroupGetter(string ip, IClientCallBack clientCallBack) : base(ip, clientCallBack)
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
