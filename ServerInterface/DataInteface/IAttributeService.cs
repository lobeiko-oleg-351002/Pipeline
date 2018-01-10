using BllEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ServerInterface.DataInteface
{
    [ServiceContract]
    public interface IAttributeService
    {
        [OperationContract]
        BllAttribute CreateAttribute(BllAttribute entity);

        [OperationContract]
        List<BllAttribute> GetAllAttributes();

        [OperationContract]
        void DeleteAttribute(int id);

        [OperationContract]
        BllAttribute UpdateAttribute(BllAttribute entity);

    }
}
