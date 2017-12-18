using BLL.Services;
using BLL.Services.Interface;
using BllEntities;
using DAL.Repositories;
using DAL.Repositories.Interface;
using ORM;
using ServerInterface;
using ServerInterfaceForLauncher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class Methods : IMethods, ILauncherMethods
    {
        private static Methods instance;
        private static ServiceDB serviceDB;
        private static IUnitOfWork uow;

        public static void Init()
        {
            serviceDB = new ServiceDB();
            uow = new UnitOfWork(serviceDB);
        }

        public string GetCurrentVersion()
        {
            return Pipeline.Properties.Resources.ResourceManager.GetString("VERSION");
        }

        public string GetTestString()
        {
            return "TEST";
        }

        public string GetUpdatePath()
        {
            return Pipeline.Properties.Resources.ResourceManager.GetString("UPDATE_PATH");
        }

        public BllUser SignIn(string login, string password)
        {
            IUserService service = new UserService(uow);
            return service.Authorize(login, password);
        }
    }
}
