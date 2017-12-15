using ServerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ServiceChannelManager
{
    public class ServiceChannelManagerSingleton
    {
        private static IMethods SourceChannel;
        public IMethods Methods
            => SourceChannel ?? (SourceChannel = CreateChannel<IMethods>("http://192.168.2.144:8080/ServerInterface/"));

        private static ServiceChannelManagerSingleton instance;

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
        private static T CreateChannel<T>(string serviceAddress)
        {
            EndpointIdentity spn = EndpointIdentity.CreateSpnIdentity("Server"); // dns
            Uri uri = new Uri(serviceAddress);
            var address = new EndpointAddress(uri, spn);
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = 2147483647;
            ChannelFactory<T> factory = new ChannelFactory<T>(binding, address);
            return factory.CreateChannel();
        }
    }
}
