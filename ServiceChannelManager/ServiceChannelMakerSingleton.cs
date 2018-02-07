using ServerInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ServiceChannelManager
{
    public class ServiceChannelMakerSingleton
    {
        private static IBusinessService SourceChannel;

        private static ServiceChannelMakerSingleton instance;

        public IBusinessService GetServerMethods(IClientCallBack handler, string hostIP)
        {
            return (SourceChannel = CreateChannel<IBusinessService>("net.tcp://" + hostIP + "/ServerInterface/", handler));
        }


        public static ServiceChannelMakerSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServiceChannelMakerSingleton();
                }
                return instance;
            }
        }

        private static T CreateChannel<T>(string serviceAddress, IClientCallBack handler)
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            DuplexChannelFactory<T> factory = new DuplexChannelFactory<T>(new InstanceContext(handler), binding, serviceAddress);
            return factory.CreateChannel();
        }

    }
}
