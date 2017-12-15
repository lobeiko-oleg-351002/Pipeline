using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ServerInterface
{
    [ServiceContract]
    public interface IMethods
    {
        [OperationContract]
        string GetTestString();


    }
}
