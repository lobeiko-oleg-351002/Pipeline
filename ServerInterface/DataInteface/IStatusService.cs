using BllEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ServerInterface.DataInteface
{
    [ServiceContract]
    public interface IStatusService
    {
        [OperationContract]
        BllStatus CreateStatus(BllStatus entity);

        [OperationContract]
        List<BllStatus> GetAllStatuses();

        [OperationContract]
        void DeleteStatus(int id);

        [OperationContract]
        BllStatus UpdateStatus(BllStatus entity);

    }
}
