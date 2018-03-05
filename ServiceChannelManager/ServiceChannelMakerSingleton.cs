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
            InitBinding(binding);
            DuplexChannelFactory<T> factory = new DuplexChannelFactory<T>(new InstanceContext(handler), binding, serviceAddress);
            return factory.CreateChannel();
        }

        private static void InitBinding(NetTcpBinding binding)
        {
            binding.CloseTimeout = TimeSpan.FromSeconds(120);
            binding.OpenTimeout = TimeSpan.FromSeconds(120);
            binding.ReceiveTimeout = TimeSpan.FromSeconds(600);
            binding.SendTimeout = TimeSpan.FromSeconds(120);
            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            binding.MaxBufferSize = 2147483647;
            binding.MaxBufferPoolSize = 2147483647;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.Security.Mode = SecurityMode.None;
            binding.TransferMode = TransferMode.Buffered;
        }

    }
}
