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

        private static IFileService FileService;

        private static ServiceChannelManagerSingleton instance;

        public IBusinessService GetServerMethods(IServerCallBack handler)
        {
            return (SourceChannel = CreateChannel<IBusinessService>("net.tcp://192.168.2.144:8080/ServerInterface/", handler));
        }

        public IBusinessService GetFileService(IServerCallBack handler)
        {
            return SourceChannel ?? (SourceChannel = CreateChannel<IBusinessService>("net.tcp://192.168.2.144:8080/ServerInterface/", handler));
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
            //EndpointIdentity spn = EndpointIdentity.CreateSpnIdentity("Server"); // dns
            //Uri uri = new Uri(serviceAddress);
           // var address = new EndpointAddress(uri, spn);
           // WSHttpBinding binding = new WSHttpBinding();
            //binding.MaxReceivedMessageSize = 2147483647;
            DuplexChannelFactory<T> factory = new DuplexChannelFactory<T>(new InstanceContext(handler), new NetTcpBinding(), serviceAddress);
            return factory.CreateChannel();
        }

    }
}
