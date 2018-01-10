using BllEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ServerInterface.DataInteface
{
    [ServiceContract]
    public interface IGroupService
    {
        [OperationContract]
        BllGroup CreateGroup(BllGroup entity);

        [OperationContract]
        List<BllGroup> GetAllGroups();

        [OperationContract]
        void DeleteGroup(int id);

        [OperationContract]
        BllGroup UpdateGroup(BllGroup entity);

    }
}
