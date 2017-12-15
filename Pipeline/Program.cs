using ServerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadLine();
            ServiceHost serviceHost = new ServiceHost(typeof(Methods));
            serviceHost.Open();
            Console.WriteLine("Host is working. Press ENTER for exit...");
            Console.ReadLine();
        }
    }
}
