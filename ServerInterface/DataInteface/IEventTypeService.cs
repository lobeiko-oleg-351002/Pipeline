using BllEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ServerInterface.DataInteface
{
    [ServiceContract]
    public interface IEventTypeService
    {
        [OperationContract]
        BllEventType CreateEventType(BllEventType entity);

        [OperationContract]
        List<BllEventType> GetAllEventTypes();

        [OperationContract]
        void DeleteEventType(int id);

        [OperationContract]
        BllEventType UpdateEventType(BllEventType entity);

    }
}
