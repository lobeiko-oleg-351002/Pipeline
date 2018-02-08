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
        public EventCRUD(string ip, IClientCallBack clientCallBack) : base(ip, clientCallBack)
        {
        }

        public List<BllEvent> GetEventsForUser(BllUser user)
        {
            try
            {
                return server.GetEventsForUser(user);
            }
            catch
            {
                throw new ConnectionFailedException();
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
    }
}
