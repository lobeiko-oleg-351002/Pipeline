using BllEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.ServerManager.Interface
{
    public interface IUserGetter
    {
        List<BllUser> GetUsersByGroupAndSignInDateRange(BllGroup group);
        List<BllUser> GetApprovers();
        List<BllUser> GetReconcilers();
    }
}
