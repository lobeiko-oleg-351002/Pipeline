using BllEntities;
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
    public interface IBusinessService : IStatusService, IEventTypeService, IAttributeService, IGroupService, IUserService, IEventApprovement
    {
        [OperationContract]
        BllUser SignIn(string login, string password);

        [OperationContract]
        List<BllUser> GetUsersByGroup(BllGroup group);

        [OperationContract]
        List<BllUser> GetUsersByGroupAndSignInDateRange(BllGroup group, int permissibleRangeInDays);

        [OperationContract]
        BllEvent CreateAndSendOutEvent(BllEvent Event);

        [OperationContract]
        void SetClientVersion(string version);
        
        [OperationContract]
        string GetClientVersion();

        [OperationContract]
        BllEvent UpdateAcceptedUsersAndSendOutEvent(BllEvent Event, BllUser updater);

        [OperationContract]
        BllEvent UpdateStatusAndSendOutEvent(BllEvent Event, BllUser updater);

        [OperationContract(IsOneWay = true)]
        void UpdateRecieversAndSendOnEvent(BllEvent Event, List<BllUser> newRecievers);

        [OperationContract]
        List<BllEvent> GetEventsForUser(BllUser user);

        [OperationContract]
        string PingServer();

        [OperationContract(IsOneWay = true)]
        void UpdateEventRecievers(BllEvent Event);

        [OperationContract]
        DateTime GetDateTime();
    }

}
