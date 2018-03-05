using BllEntities;
using ServerInterface;
using ServiceChannelManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ServiceChannelManager
{
    public class ServerInstance : DuplexClientBase<IBusinessService>, IServerInstance 
    {
        protected bool IsServerOnline;

        public ServerInstance(InstanceContext callbackInstance, WSDualHttpBinding binding, EndpointAddress endpointAddress) : base(callbackInstance, binding, endpointAddress)
        {

        }

        public void ConnectToServer()
        {
            try
            {
                Channel.PingServer();
                IsServerOnline = true;
            }
            catch (EndpointNotFoundException ex)
            {
                IsServerOnline = false;
                throw ex;
            }
            catch(Exception ex)
            {
                IsServerOnline = false;
               // throw new ConnectionFailedException();
            }
        }

        public bool IsConnected()
        {
            return IsServerOnline;
        }

        public void PingServer()
        {
            try
            {
                Channel.PingServer();
            }
            catch(Exception ex)
            {
                throw new ConnectionFailedException();
            }
        }

        public List<BllAttribute> GetAllAttributes()
        {
            try
            {
                return Channel.GetAllAttributes();
            }
            catch (Exception ex)
            {
                throw new ConnectionFailedException();
            }
        }

        public BllUser ConnectToServerAndSignIn(string ip, IClientCallBack clientCallBack, BllUser user)
        {
            try
            {
                ConnectToServer();
                return SignIn(user);
            }
            catch (ConnectionFailedException ex)
            {
                throw ex;
            }
            catch (UserIsNullException ex)
            {
                throw ex;
            }
        }

        public BllUser SignIn(BllUser User)
        {
            try
            {
                User = Channel.SignIn(User.Login, User.Password);
                if (User == null)
                {
                    throw new UserIsNullException();
                }
                return User;
            }
            catch (UserIsNullException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new ConnectionFailedException();
            }
        }

        public List<BllEvent> GetEventsForUser(BllUser user)
        {
            try
            {
                return Channel.GetEventsForUser(user);
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
                return Channel.UpdateStatusAndSendOutEvent(Event, sender);
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
                return Channel.UpdateAcceptedUsersAndSendOutEvent(Event, sender);
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
                return Channel.CreateAndSendOutEvent(Event);
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
                Channel.UpdateRecieversAndSendOnEvent(Event, newRecievers);
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
                Channel.UpdateEventRecievers(Event);
            }
            catch
            {
                throw new ConnectionFailedException();
            }
        }

        public List<BllGroup> GetAllGroups()
        {
            try
            {
                return Channel.GetAllGroups();
            }
            catch
            {
                throw new ConnectionFailedException();
            }
        }

        public BllStatus GetStatusDELETED()
        {
            try
            {
                return Channel.GetStatusDeleted();
            }
            catch (Exception ex)
            {
                throw new ConnectionFailedException();
            }
        }

        public BllStatus GetStatusCLOSED()
        {
            try
            {
                return Channel.GetStatusClosed();
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
                return Channel.GetAllStatuses();
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
                return Channel.GetAllStatusesExceptDeletedAndClosed();
            }
            catch (Exception ex)
            {
                throw new ConnectionFailedException();
            }
        }

        public List<BllUser> GetUsersByGroupAndSignInDateRange(BllGroup group, int permissibleRangeInDays)
        {
            try
            {
                return Channel.GetUsersByGroupAndSignInDateRange(group, permissibleRangeInDays);
            }
            catch
            {
                throw new ConnectionFailedException();
            }
        }

        public DateTime GetDateTime()
        {
            try
            {
                return Channel.GetDateTime();
            }
            catch
            {
                throw new ConnectionFailedException();
            }
        }
    }
}
