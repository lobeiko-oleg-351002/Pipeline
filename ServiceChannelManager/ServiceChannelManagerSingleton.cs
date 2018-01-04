using ServerInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ServiceChannelManager
{
    public class ServiceChannelManagerSingleton
    {
        private static IBusinessService SourceChannel;

        private static ServiceChannelManagerSingleton instance;

        public IBusinessService GetServerMethods(IServerCallBack handler)
        {
            return (SourceChannel = CreateChannel<IBusinessService>("net.tcp://192.168.2.144:8080/ServerInterface/", handler));
        }


        public static ServiceChannelManagerSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServiceChannelManagerSingleton();
                }
                return instance;
            }
        }
        private static T CreateChannel<T>(string serviceAddress, IServerCallBack handler)
        {
            NetTcpBinding binding = new NetTcpBinding("businessTcpBinding");
            DuplexChannelFactory<T> factory = new DuplexChannelFactory<T>(new InstanceContext(handler), binding, serviceAddress);
            //factory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            return factory.CreateChannel();
        }

    }
}
