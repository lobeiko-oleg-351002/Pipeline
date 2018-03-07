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
        public StatusGetter(IBusinessService server) : base(server)
        {
        }

        public BllStatus GetStatusDELETED()
        {
            try
            {
                return server.GetStatusDeleted();
            }
            catch(Exception ex)
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
            catch (Exception ex)
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
            catch (Exception ex)
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
            catch(Exception ex)
            {
                throw new ConnectionFailedException();
            }
        }
    }
}
