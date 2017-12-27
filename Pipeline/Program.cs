using DAL.Repositories;
using DAL.Repositories.Interface;
using ORM;
using ServerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Methods.Init();
            Console.ReadLine();
            ServiceHost serviceHost = new ServiceHost(typeof(Methods));
            serviceHost.Open();

            while(true)
            {
                Console.WriteLine("Host is working.");
                Thread.Sleep(10000);
                Methods.PingClients();

                //Console.ReadLine();
            }


        }
    }
}
