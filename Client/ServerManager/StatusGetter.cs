using BllEntities;
using Client.ServerManager.Interface;
using ServerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.ServerManager
{
    public class StatusGetter : ServerInstance, IStatusGetter
    {
        public StatusGetter(string ip, IClientCallBack clientCallBack) : base(ip, clientCallBack)
        {
        }

        public BllStatus GetStatusDELETED()
        {
            try
            {
                return server.GetStatusDeleted();
            }
            catch
            {
                throw new ConnectionFailedException();
            }
        }

        public BllStatus GetStatusCLOSED()
        {
            try
            {
                return server.GetStatusClosed();
            }
            catch
            {
                throw new ConnectionFailedException();
            }
        }

        public List<BllStatus> GetAllStatuses()
        {
            try
            {
                return server.GetAllStatuses();
            }
            catch
            {
                throw new ConnectionFailedException();
            }
        }

        public List<BllStatus> GetAllStatusesExceptDELETEDandCLOSED()
        {
            try
            {
                return server.GetAllStatusesExceptDeletedAndClosed();
            }
            catch
            {
                throw new ConnectionFailedException();
            }
        }
    }
}
