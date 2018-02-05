using BLL.Services;
using BLL.Services.Interface;
using BllEntities;
using DAL.Entities;
using DAL.Repositories;
using DAL.Repositories.Interface;
using Globals;
using ORM;
using ServerInterface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class BusinessService : IBusinessService
    {

        //private static ServiceDB serviceDB;
        //private static IUnitOfWork uow;

        private static Dictionary<string, IClientCallBack> Clients = new Dictionary<string, IClientCallBack>();
        private static object locker = new object();

        private const string SERVER_STATE = "Online";
        private const string TAG_VERSION = "CLIENT_VERSION";


        public static void Init()
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);

                if (!uow.Statuses.IsContainsWithName(Globals.Globals.STATUS_CLOSED))
                {
                    uow.Statuses.Create(new DalStatus { Name = Globals.Globals.STATUS_CLOSED });
                }
                if (!uow.Statuses.IsContainsWithName(Globals.Globals.STATUS_DELETED))
                {
                    uow.Statuses.Create(new DalStatus { Name = Globals.Globals.STATUS_DELETED });
                }
            }
        }

        public void SetClientVersion(string version)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[TAG_VERSION] != null)
            {
                config.AppSettings.Settings[TAG_VERSION].Value = version;
            }
            else
            {
                config.AppSettings.Settings.Add(TAG_VERSION, version);
            }
            config.Save(ConfigurationSaveMode.Minimal);
        }

        public string GetClientVersion()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[TAG_VERSION] != null)
            {
                return config.AppSettings.Settings[TAG_VERSION].Value;
            }
            return null;
        }


        public BllEvent CreateAndSendOutEvent(BllEvent Event)
        {
            try
            {
                var datetime = DateTime.Now;
                Event.Date = datetime;
                using (ServiceDB serviceDB = new ServiceDB())
                {
                    IUnitOfWork uow = new UnitOfWork(serviceDB);
                    IEventService eventService = new EventService(uow);
                    BllEvent res = eventService.Create(Event);
                    new Thread(() =>
                    {
                        InvokeEventWithRecievers(Event);
                    }).Start();
                    return res;
                }
            }
            catch(Exception ex)
            {
                LogWriter.WriteMessage("CreateAndSendOutEvent", ex.Message + ex.InnerException, Event.Sender.Fullname);
                return Event;
            }

        }

        public List<BllEvent> GetEventsForUser(BllUser user)
        {
            try
            {
                using (ServiceDB serviceDB = new ServiceDB())
                {
                    IUnitOfWork uow = new UnitOfWork(serviceDB);
                    IEventService eventService = new EventService(uow);
                    var events = eventService.GetEventsForUser(user).ToList();

                    return events;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteMessage("GetEventsForUser", ex.Message + ex.InnerException, user.Fullname);
                return null;
            }
        }

        public List<BllUser> GetUsersByGroup(BllGroup group)
        {
            try
            {
                using (ServiceDB serviceDB = new ServiceDB())
                {
                    IUnitOfWork uow = new UnitOfWork(serviceDB);
                    IUserService userService = new UserService(uow);
                    return userService.GetUsersByGroup(group.Id);
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteMessage("GetUsersByGroup", ex.Message + ex.InnerException, group.Name);
                return null;
            }
        }

        public List<BllUser> GetUsersByGroupAndSignInDateRange(BllGroup group, int permissibleRangeInDays)
        {
            try
            {
                using (ServiceDB serviceDB = new ServiceDB())
                {
                    IUnitOfWork uow = new UnitOfWork(serviceDB);
                    IUserService userService = new UserService(uow);
                    return userService.GetUsersByGroupAndSignInDateRange(group.Id, permissibleRangeInDays);
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteMessage("GetUsersByGroupAndSignInDateRange", ex.Message + ex.InnerException, group.Name);
                return null;
            }
        }


        public BllEvent UpdateAcceptedUsersAndSendOutEvent(BllEvent Event, BllUser updater)
        {
            try
            {
                using (ServiceDB serviceDB = new ServiceDB())
                {
                    IUnitOfWork uow = new UnitOfWork(serviceDB);
                    UserLibService userservice = new UserLibService(uow);
                    Event.RecieverLib = userservice.Update(Event.RecieverLib);

                    new Thread(() =>
                    {
                        UpdateEventWithUsers(Event, updater);
                    }).Start();
                    return Event;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteMessage("UpdateAcceptedUsersAndSendOutEvent", ex.Message + ex.InnerException, updater.Fullname);
                return Event;
            }
        }

        private BllUserLib AddUsersToLib(BllUserLib lib, List<BllUser> users)
        {
            foreach(var item in users)
            {
                lib.SelectedEntities.Add(new BllSelectedUser { Entity = item });
            }
            return lib;
        }

        private void UpdateEventWithRecieversExceptUsers(BllEvent Event, List<BllUser> users)
        {
            try
            {
                foreach (var reciever in Event.RecieverLib.SelectedEntities)
                {
                    try
                    {
                        if (!users.Contains(reciever.Entity))
                        {
                            Clients[reciever.Entity.Login].UpdateEvent(Event);
                        }
                    }
                    catch (Exception ex)
                    {
                        Clients.Remove(reciever.Entity.Login);
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteMessage("UpdateEventWithRecieversExceptUsers", ex.Message + ex.InnerException, "");
            }
        }

        public void UpdateRecieversAndSendOnEvent(BllEvent Event, List<BllUser> newRecievers)
        {
            try
            {
                using (ServiceDB serviceDB = new ServiceDB())
                {
                    IUnitOfWork uow = new UnitOfWork(serviceDB);
                    UserLibService userservice = new UserLibService(uow);
                    Event.RecieverLib = AddUsersToLib(Event.RecieverLib, newRecievers);
                    Event.RecieverLib = userservice.Update(Event.RecieverLib);
                    new Thread(() =>
                    {
                        InvokeEventWithUsers(Event, newRecievers);
                        UpdateEventWithRecieversExceptUsers(Event, newRecievers);
                    }).Start();
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteMessage("UpdateRecieversAndSendOutEvent", ex.Message + ex.InnerException, "");
            }
        }

        public BllEvent UpdateStatusAndSendOutEvent(BllEvent Event, BllUser updater)
        {
            try
            {
                var datetime = DateTime.Now;
                Event.StatusLib.SelectedEntities.Last().Date = datetime;
                using (ServiceDB serviceDB = new ServiceDB())
                {
                    IUnitOfWork uow = new UnitOfWork(serviceDB);
                    StatusLibService service = new StatusLibService(uow);
                    Event.StatusLib = service.Update(Event.StatusLib);

                    UserLibService userservice = new UserLibService(uow);
                    Event.RecieverLib = userservice.Update(Event.RecieverLib);

                    new Thread(() =>
                    {
                        UpdateEventWithUsers(Event, updater);
                    }).Start();
                    return Event;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteMessage("UpdateStatusAndSendOutEvent", ex.Message + ex.InnerException, updater.Fullname);
                return Event;
            }
        }



        private void UpdateEventWithUsers(BllEvent Event, BllUser updater)
        {
            try
            {
                foreach (var reciever in Event.RecieverLib.SelectedEntities)
                {
                    try
                    {
                        if (updater.Id != reciever.Entity.Id)
                        {
                            Clients[reciever.Entity.Login].UpdateEvent(Event);
                        }
                    }
                    catch (Exception ex)
                    {
                        Clients.Remove(reciever.Entity.Login);
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteMessage("UpdateEventWithUsers", ex.Message + ex.InnerException, updater.Fullname);
            }
        }

        private void InvokeEventWithRecievers(BllEvent Event)
        {
            foreach (var reciever in Event.RecieverLib.SelectedEntities)
            {
                try
                {
                    if (Event.Sender.Id != reciever.Entity.Id)
                    {
                        Clients[reciever.Entity.Login].GetEvent(Event);
                    }
                }
                catch (Exception ex)
                {
                    Clients.Remove(reciever.Entity.Login);
                }
            }
        }

        private void InvokeEventWithUsers(BllEvent Event, List<BllUser> users)
        {
            foreach (var user in users)
            {
                try
                {
                    Clients[user.Login].GetEvent(Event);
                }
                catch (Exception ex)
                {
                    Clients.Remove(user.Login);
                }
            }
        }


        private void RegisterClient(string key)
        {
            if (key != null && key != "")
            {
                try
                {
                    IClientCallBack callback = OperationContext.Current.GetCallbackChannel<IClientCallBack>();
                    lock (locker)
                    {
                        //remove the old client
                        if (Clients.Keys.Contains(key))
                            Clients.Remove(key);
                        Clients.Add(key, callback);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("RegisterClient", ex);
                }
            }
        }

        public static int GetUsersOnlineCount()
        {
            return Clients.Count;
        }

        public BllUser SignIn(string login, string password)
        {
            try
            {
                using (ServiceDB serviceDB = new ServiceDB())
                {
                    IUnitOfWork uow = new UnitOfWork(serviceDB);
                    IUserService service = new UserService(uow);
                    BllUser user = service.Authorize(login, password);
                    if (user != null)
                    {
                        RegisterClient(user.Login);
                        user.SignInDate = DateTime.Now;
                        service.Update(user);
                    }
                    return user;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteMessage("SignIn", ex.Message + ex.InnerException, login);
                return null;
            }
        }

        public static void PingClients()
        {
            lock (locker)
            {
                var inactiveClients = new List<string>();
                foreach (var client in Clients)
                {
                    try
                    {
                        client.Value.Ping() ;
                    }
                    catch (Exception ex)
                    {
                        inactiveClients.Add(client.Key);
                    }
                }

                if (inactiveClients.Count > 0)
                {
                    foreach (var client in inactiveClients)
                    {
                        Clients.Remove(client);
                    }
                }
            }
        }

        public string PingServer()
        {
            return SERVER_STATE;
        }

        #region StatusService
        public List<BllStatus> GetAllStatuses()
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IStatusService statusService = new StatusService(uow);
                return statusService.GetAll().ToList();
            }
        }

        public BllStatus CreateStatus(BllStatus entity)
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IStatusService service = new StatusService(uow);
                return service.Create(entity);
            }
        }

        public void DeleteStatus(int id)
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IStatusService service = new StatusService(uow);
                service.Delete(id);
            }
        }

        public BllStatus UpdateStatus(BllStatus entity)
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IStatusService service = new StatusService(uow);
                return service.Update(entity);
            }
        }

        public bool IsContainsWithName(string name)
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IStatusService service = new StatusService(uow);
                return service.IsContainsWithName(name);
            }
        }

        public List<BllStatus> GetAllStatusesExceptDeletedAndClosed()
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IStatusService service = new StatusService(uow);
                return service.GetAllStatusesExceptDeletedAndClosed();
            }
        }

        public BllStatus GetStatusDeleted()
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IStatusService service = new StatusService(uow);
                return service.GetStatusDeleted();
            }
        }

        public BllStatus GetStatusClosed()
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IStatusService service = new StatusService(uow);
                return service.GetStatusClosed();
            }
        }
        #endregion

        #region AttributeService
        public List<BllAttribute> GetAllAttributes()
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IAttributeService AttributeService = new AttributeService(uow);
                return AttributeService.GetAll().ToList();
            }
        }

        public BllAttribute CreateAttribute(BllAttribute entity)
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IAttributeService service = new AttributeService(uow);
                return service.Create(entity);
            }
        }

        public void DeleteAttribute(int id)
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IAttributeService service = new AttributeService(uow);
                service.Delete(id);
            }
        }

        public BllAttribute UpdateAttribute(BllAttribute entity)
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IAttributeService service = new AttributeService(uow);
                return service.Update(entity);
            }
        }
        #endregion

        #region GroupService
        public List<BllGroup> GetAllGroups()
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IGroupService GroupService = new GroupService(uow);
                return GroupService.GetAll().ToList();
            }
        }

        public BllGroup CreateGroup(BllGroup entity)
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IGroupService service = new GroupService(uow);
                return service.Create(entity);
            }
        }

        public void DeleteGroup(int id)
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IGroupService service = new GroupService(uow);
                service.Delete(id);
            }
        }

        public BllGroup UpdateGroup(BllGroup entity)
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IGroupService service = new GroupService(uow);
                return service.Update(entity);
            }
        }
        #endregion

        #region UserService
        public List<BllUser> GetAllUsers()
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IUserService UserService = new UserService(uow);
                return UserService.GetAll().ToList();
            }
        }

        public BllUser CreateUser(BllUser entity)
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IUserService service = new UserService(uow);
                return service.Create(entity);
            }
        }

        public void DeleteUser(int id)
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IUserService service = new UserService(uow);
                service.Delete(id);
            }
        }

        public BllUser UpdateUser(BllUser entity)
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IUserService service = new UserService(uow);
                return service.Update(entity);
            }
        }
        #endregion

        #region EventTypeService
        public List<BllEventType> GetAllEventTypes()
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IEventTypeService EventTypeService = new EventTypeService(uow);
                return EventTypeService.GetAll().ToList();
            }
        }

        public BllEventType CreateEventType(BllEventType entity)
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IEventTypeService service = new EventTypeService(uow);
                return service.Create(entity);
            }
        }

        public void DeleteEventType(int id)
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IEventTypeService service = new EventTypeService(uow);
                service.Delete(id);
            }
        }

        public BllEventType UpdateEventType(BllEventType entity)
        {
            using (ServiceDB serviceDB = new ServiceDB())
            {
                IUnitOfWork uow = new UnitOfWork(serviceDB);
                IEventTypeService service = new EventTypeService(uow);
                return service.Update(entity);
            }
        }
        #endregion



    }
}
