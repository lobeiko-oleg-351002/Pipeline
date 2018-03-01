using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using BllEntities;

namespace ServerInterface
{
    public class BusinessClientProxy : DuplexClientBase<IBusinessService>, IBusinessService
    {
        public BusinessClientProxy(InstanceContext callbackInstance, WSDualHttpBinding binding, EndpointAddress endpointAddress) : base(callbackInstance, binding, endpointAddress)
        {

        }

        public BllEvent CreateAndSendOutEvent(BllEvent Event)
        {
            throw new NotImplementedException();
        }

        public BllAttribute CreateAttribute(BllAttribute entity)
        {
            throw new NotImplementedException();
        }

        public BllEventType CreateEventType(BllEventType entity)
        {
            throw new NotImplementedException();
        }

        public BllGroup CreateGroup(BllGroup entity)
        {
            throw new NotImplementedException();
        }

        public BllStatus CreateStatus(BllStatus entity)
        {
            throw new NotImplementedException();
        }

        public BllUser CreateUser(BllUser entity)
        {
            throw new NotImplementedException();
        }

        public void DeleteAttribute(int id)
        {
            throw new NotImplementedException();
        }

        public void DeleteEventType(int id)
        {
            throw new NotImplementedException();
        }

        public void DeleteGroup(int id)
        {
            throw new NotImplementedException();
        }

        public void DeleteStatus(int id)
        {
            throw new NotImplementedException();
        }

        public void DeleteUser(int id)
        {
            throw new NotImplementedException();
        }

        public List<BllAttribute> GetAllAttributes()
        {
            throw new NotImplementedException();
        }

        public List<BllEventType> GetAllEventTypes()
        {
            throw new NotImplementedException();
        }

        public List<BllGroup> GetAllGroups()
        {
            throw new NotImplementedException();
        }

        public List<BllStatus> GetAllStatuses()
        {
            throw new NotImplementedException();
        }

        public List<BllStatus> GetAllStatusesExceptDeletedAndClosed()
        {
            throw new NotImplementedException();
        }

        public List<BllUser> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public string GetClientVersion()
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime()
        {
            throw new NotImplementedException();
        }

        public List<BllEvent> GetEventsForUser(BllUser user)
        {
            throw new NotImplementedException();
        }

        public BllStatus GetStatusClosed()
        {
            throw new NotImplementedException();
        }

        public BllStatus GetStatusDeleted()
        {
            throw new NotImplementedException();
        }

        public List<BllUser> GetUsersByGroup(BllGroup group)
        {
            throw new NotImplementedException();
        }

        public List<BllUser> GetUsersByGroupAndSignInDateRange(BllGroup group, int permissibleRangeInDays)
        {
            throw new NotImplementedException();
        }

        public bool IsContainsWithName(string name)
        {
            throw new NotImplementedException();
        }

        public string PingServer()
        {
            throw new NotImplementedException();
        }

        public void SetClientVersion(string version)
        {
            throw new NotImplementedException();
        }

        public BllUser SignIn(string login, string password)
        {
            throw new NotImplementedException();
        }

        public BllEvent UpdateAcceptedUsersAndSendOutEvent(BllEvent Event, BllUser updater)
        {
            throw new NotImplementedException();
        }

        public BllAttribute UpdateAttribute(BllAttribute entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateEventRecievers(BllEvent Event)
        {
            throw new NotImplementedException();
        }

        public BllEventType UpdateEventType(BllEventType entity)
        {
            throw new NotImplementedException();
        }

        public BllGroup UpdateGroup(BllGroup entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateRecieversAndSendOnEvent(BllEvent Event, List<BllUser> newRecievers)
        {
            throw new NotImplementedException();
        }

        public BllStatus UpdateStatus(BllStatus entity)
        {
            throw new NotImplementedException();
        }

        public BllEvent UpdateStatusAndSendOutEvent(BllEvent Event, BllUser updater)
        {
            throw new NotImplementedException();
        }

        public BllUser UpdateUser(BllUser entity)
        {
            throw new NotImplementedException();
        }
    }
}
