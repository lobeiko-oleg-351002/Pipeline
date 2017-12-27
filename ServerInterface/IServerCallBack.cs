using BllEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ServerInterface
{
    public interface IServerCallBack
    {
        [OperationContract(IsOneWay = true)]
        void GetEvent(BllEvent Event);

        [OperationContract(IsOneWay = true)]
        void Ping();

    }
}
