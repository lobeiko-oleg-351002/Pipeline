using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ServerInterfaceForLauncher
{
    [ServiceContract]
    public interface ILauncherMethods
    {
        [OperationContract]
        string GetClientVersion();

        [OperationContract]
        string GetUpdatePath();
    }
}
