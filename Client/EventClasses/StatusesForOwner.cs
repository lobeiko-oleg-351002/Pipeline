using BllEntities;
using Client.ServerManager;
using Client.ServerManager.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.EventClasses
{
    public static class StatusesForOwner
    {
        public static BllStatus StatusDELETED { get; private set; }
        public static BllStatus StatusCLOSED { get; private set; }

        public static void Init(ServerInstance serverInstance)
        {
            if (serverInstance.IsConnected())
            {
                IStatusGetter sg = new StatusGetter(serverInstance.server);
                StatusDELETED = sg.GetStatusDELETED();
                StatusCLOSED = sg.GetStatusCLOSED();
            }
        }

        public static List<BllStatus> GetStatusesForOwner()
        {
            List<BllStatus> res = new List<BllStatus>();
            if (StatusCLOSED != null)
            {
                res.Add(StatusCLOSED);
                res.Add(StatusDELETED);
            }
            return res;
        }

        public static bool IsStatusForOwner(BllStatus status)
        {
            if (StatusCLOSED != null)
            {
                if ((status.Id == StatusCLOSED.Id) || (status.Id == StatusDELETED.Id))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
