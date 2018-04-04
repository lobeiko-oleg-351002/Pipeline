using BllEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ServerInterface.DataInteface
{
    [ServiceContract]
    public interface IUserService
    {
        [OperationContract]
        BllUser CreateUser(BllUser entity);

        [OperationContract]
        List<BllUser> GetAllUsers();

        [OperationContract]
        void DeleteUser(int id);

        [OperationContract]
        BllUser UpdateUser(BllUser entity);

        [OperationContract]
        List<BllUser> GetApprovers();

    }
}
