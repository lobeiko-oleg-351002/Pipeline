using BllEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ServerInterface
{
    [ServiceContract]
    public interface IEventApprovement
    {
        [OperationContract]
        BllEvent CreateEventAndSendToApprover(BllEvent Event);

        [OperationContract]
        BllEvent CreateEventAndSendToReconcilers(BllEvent Event);

        [OperationContract(IsOneWay = true)]
        void ApproveAndSendOutEvent(BllEvent Event);

        [OperationContract(IsOneWay = true)]
        void HandleReconcilerSigning(BllEvent Event);
    }
}
