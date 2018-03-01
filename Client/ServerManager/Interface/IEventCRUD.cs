using BllEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.ServerManager.Interface
{
    public interface IEventCRUD
    {
        BllEvent UpdateAcceptedUsersAndSendOutEvent(BllEvent Event, BllUser sender);
        BllEvent UpdateStatusAndSendOutEvent(BllEvent Event, BllUser sender);
        List<BllEvent> GetEventsForUser(BllUser user);
        BllEvent CreateAndSendOutEvent(BllEvent Event);
        void UpdateRecieversAndSendOnEvent(BllEvent Event, List<BllUser> newRecievers);
        void UpdateEventRecievers(BllEvent Event);
    }
}
