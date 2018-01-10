﻿using BllEntities;
using ServerInterface.DataInteface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServerInterface
{
    [ServiceContract(CallbackContract = typeof(IClientCallBack))]
    public interface IBusinessService : IStatusService, IEventTypeService, IAttributeService, IGroupService, IUserService
    {
        [OperationContract]
        BllUser SignIn(string login, string password);

        [OperationContract]
        IEnumerable<BllUser> GetUsersByGroup(BllGroup group);

        [OperationContract]
        BllEvent CreateAndSendOutEvent(BllEvent Event);

        [OperationContract]
        BllEvent UpdateAndSendOutEvent(BllEvent Event, BllUser updater);

        [OperationContract]
        List<BllEvent> GetEventsForUser(BllUser user);

        [OperationContract]
        void RegisterClient(string clientName);

        [OperationContract]
        string PingServer();



    }

}
