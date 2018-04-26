using BllEntities;
using Client.ServerManager.Interface;
using ServerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.ServerManager
{
    public class UserGetter : ServerInstance, IUserGetter
    {
        public UserGetter(IBusinessService server) : base(server)
        {
        }

        public List<BllUser> GetUsersByGroupAndSignInDateRange(BllGroup group, int permissibleRangeInDays)
        {
            try
            {
                return server.GetUsersByGroupAndSignInDateRange(group, permissibleRangeInDays);
            }
            catch
            {
                throw new ConnectionFailedException();
            }
        }

        public List<BllUser> GetApprovers()
        {
            try
            {
                return server.GetApprovers();
            }
            catch
            {
                throw new ConnectionFailedException();
            }
        }

        public List<BllUser> GetReconcilers()
        {
            try
            {
                return server.GetReconcilers();
            }
            catch
            {
                throw new ConnectionFailedException();
            }
        }
    }
}
