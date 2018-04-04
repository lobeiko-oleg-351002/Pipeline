using BllEntities;
using Client.ServerManager.Interface;
using ServerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.ServerManager
{
    public class EventCRUD : ServerInstance, IEventCRUD
    {
        public EventCRUD(IBusinessService server) : base(server)
        {
        }

        public List<BllEvent> GetEventsForUser(BllUser user)
        {
            try
            {
                return server.GetEventsForUser(user);
            }
            catch (Exception ex)
            {
                throw new ConnectionFailedException(ex.Message);
            }
        }

        public BllEvent UpdateStatusAndSendOutEvent(BllEvent Event, BllUser sender)
        {
            try
            {
                return server.UpdateStatusAndSendOutEvent(Event, sender);
            }
            catch
            {
                throw new ConnectionFailedException();
            }
        }

        public BllEvent UpdateAcceptedUsersAndSendOutEvent(BllEvent Event, BllUser sender)
        {
            try
            {
                return server.UpdateAcceptedUsersAndSendOutEvent(Event, sender);
            }
            catch
            {
                throw new ConnectionFailedException();
            }
        }

        public BllEvent CreateAndSendOutEvent(BllEvent Event)
        {
            try
            {
                return server.CreateAndSendOutEvent(Event);
            }
            catch
            {
                throw new ConnectionFailedException();
            }
        }

        public BllEvent CreateEventAndSendToApprover(BllEvent Event)
        {
            try
            {
                return server.CreateEventAndSendToApprover(Event);
            }
            catch
            {
                throw new ConnectionFailedException();
            }
        }

        public void ApproveAndSendOutEvent(BllEvent Event)
        {
            try
            {
                server.ApproveAndSendOutEvent(Event);
            }
            catch
            {
                throw new ConnectionFailedException();
            }
        }

        public void UpdateRecieversAndSendOnEvent(BllEvent Event, List<BllUser> newRecievers)
        {
            try
            {
                server.UpdateRecieversAndSendOnEvent(Event, newRecievers);
            }
            catch
            {
                throw new ConnectionFailedException();
            }
        }

        public void UpdateEventRecievers(BllEvent Event)
        {
            try
            {
                server.UpdateEventRecievers(Event);
            }
            catch
            {
                throw new ConnectionFailedException();
            }
        }
    }
}
