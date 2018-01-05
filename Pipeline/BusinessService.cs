using BLL.Services;
using BLL.Services.Interface;
using BllEntities;
using DAL.Repositories;
using DAL.Repositories.Interface;
using ORM;
using ServerInterface;
using ServerInterfaceForLauncher;
using System;
using System.Collections.Generic;
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

        private static ServiceDB serviceDB;
        private static IUnitOfWork uow;

        private static Dictionary<string, IServerCallBack> Clients = new Dictionary<string, IServerCallBack>();
        private static object locker = new object();

        private const string SERVER_STATE = "Online";


        public static void Init()
        {
            serviceDB = new ServiceDB();
            uow = new UnitOfWork(serviceDB);
        }

  
        public BllEvent CreateAndSendOutEvent(BllEvent Event)
        {
            var datetime = DateTime.Now;
            Event.Date = datetime;
            Event.StatusLib.SelectedEntities.Last().Date = datetime;
            IEventService eventService = new EventService(uow);
            BllEvent res = eventService.Create(Event);
            new Thread(() =>
            {
                InvokeEventWithUsers(Event);
            }).Start();
            return res;

        }


        public BllEvent UpdateAndSendOutEvent(BllEvent Event, BllUser updater)
        {
            var datetime = DateTime.Now;
            Event.StatusLib.SelectedEntities.Last().Date = datetime;
            StatusLibService service = new StatusLibService(uow);
            Event.StatusLib = service.Update(Event.StatusLib);
            new Thread(() =>
            {
                UpdateEventWithUsers(Event, updater);
            }).Start();
            return Event;
        }

        private void UpdateEventWithUsers(BllEvent Event, BllUser updater)
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

        private void InvokeEventWithUsers(BllEvent Event)
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

        public IEnumerable<BllAttribute> GetAllAttributes()
        {
            IAttributeService attributeService = new AttributeService(uow);
            return attributeService.GetAll();
        }


        public IEnumerable<BllEventType> GetAllEventTypes()
        {
            IEventTypeService eventTypeService = new EventTypeService(uow);
            return eventTypeService.GetAll();
        }


        public IEnumerable<BllGroup> GetAllGroups()
        {
            IGroupService groupService = new GroupService(uow);
            return groupService.GetAll();
        }


        public List<BllStatus> GetAllStatuses()
        {
            IStatusService statusService = new StatusService(uow);
            return statusService.GetAll().ToList(); 
        }

        public List<BllEvent> GetEventsForUser(BllUser user)
        {
            IEventService eventService = new EventService(uow);
            return eventService.GetEventsForUser(user).ToList();
        }

        public IEnumerable<BllUser> GetUsersByGroup(BllGroup group)
        {
            IUserService userService = new UserService(uow);
            return userService.GetUsersByGroup(group.Id);
        }

        public void RegisterClient(string login)
        {
            if (login != null && login != "")
            {
                try
                {
                    IServerCallBack callback = OperationContext.Current.GetCallbackChannel<IServerCallBack>();
                    lock (locker)
                    {
                        //remove the old client
                        if (Clients.Keys.Contains(login))
                            Clients.Remove(login);
                        Clients.Add(login, callback);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        public BllUser SignIn(string login, string password)
        {
            IUserService service = new UserService(uow);
            return service.Authorize(login, password);
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


    }
}
