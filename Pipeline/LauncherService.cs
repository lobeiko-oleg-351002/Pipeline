using ServerInterfaceForLauncher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class LauncherService : ILauncherMethods
    {


     

        public string GetCurrentVersion()
        {
            return Pipeline.Properties.Resources.ResourceManager.GetString("VERSION");
        }

   

        public string GetUpdatePath()
        {
            return Pipeline.Properties.Resources.ResourceManager.GetString("UPDATE_PATH");
        }

     

    }
}
