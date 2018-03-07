using BllEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.ServerManager.Interface
{
    interface IStatusGetter
    {
        List<BllStatus> GetAllStatuses();
        List<BllStatus> GetAllStatusesExceptDELETEDandCLOSED();
        BllStatus GetStatusCLOSED();
        BllStatus GetStatusDELETED();
    }
}
