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
            try
            {
                BusinessService.Init();
                ServiceHost host = new ServiceHost(typeof(BusinessService));
                host.Open();
                ServiceHost fileService = new ServiceHost(typeof(FileService));
                fileService.Open();
                ServiceHost launcherService = new ServiceHost(typeof(LauncherService));
                launcherService.Open();                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            new Thread(() =>
            {
                while (true)
                {
                    Thread.CurrentThread.IsBackground = true;
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]: Host is working. Users online: " + BusinessService.GetUsersOnlineCount() + ".");
                    Thread.Sleep(10000);
                    //BusinessService.PingClients();
                }
            }).Start();

            while (true) ;
        }
    }
}
