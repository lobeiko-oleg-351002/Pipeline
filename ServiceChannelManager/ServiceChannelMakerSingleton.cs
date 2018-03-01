using ServerInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Xml;

namespace ServiceChannelManager
{
    public class ServiceChannelMakerSingleton
    {
        private static BusinessClientProxy SourceChannel;

        private static ServiceChannelMakerSingleton instance;

        public BusinessClientProxy GetServerMethods(IClientCallBack handler, string hostIP)
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

        private static BusinessClientProxy CreateChannel<T>(string serviceAddress, IClientCallBack handler)
        {
            WSDualHttpBinding binding = new WSDualHttpBinding(WSDualHttpSecurityMode.None);
            InitBinding(binding);
            BusinessClientProxy proxy = new BusinessClientProxy(new InstanceContext(handler), binding, new EndpointAddress(serviceAddress));
            //DuplexClientBase<T> factory = new DuplexClientBase<T>(new InstanceContext(handler), binding, serviceAddress);
            return proxy;
        }

        private static void InitBinding(WSDualHttpBinding binding)
        {
            binding.CloseTimeout = TimeSpan.FromSeconds(120);
            binding.OpenTimeout = TimeSpan.FromSeconds(120);
            binding.ReceiveTimeout = TimeSpan.FromSeconds(600);
            binding.SendTimeout = TimeSpan.FromSeconds(120);
            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            binding.MaxBufferPoolSize = 2147483647;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxReceivedMessageSize = 2147483647;

            //XmlDictionaryReaderQuotas xml = new XmlDictionaryReaderQuotas();
            //xml.MaxArrayLength = 2147483647;
            //xml.MaxBytesPerRead = 2147483647;
            //xml.MaxDepth = 2147483647;
            //xml.MaxNameTableCharCount = 2147483647;
            //xml.MaxStringContentLength = 2147483647;
            //binding.ReaderQuotas = xml;
        }

    }
}
