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
            BusinessService.Init();
            Console.ReadLine();
            ServiceHost host = new ServiceHost(typeof(BusinessService));
            host.Open();
            ServiceHost fileService = new ServiceHost(typeof(FileService));
            fileService.Open();

            while(true)
            {
                Console.WriteLine("Host is working.");
                Thread.Sleep(10000);
                BusinessService.PingClients();

                //Console.ReadLine();
            }


        }
    }
}
