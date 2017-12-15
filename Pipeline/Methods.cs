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
        public string GetCurrentVersion()
        {
            return Pipeline.Properties.Resources.ResourceManager.GetString("version");
        }

        public string GetTestString()
        {
            return "TEST";
        }

        public string GetUpdatePath()
        {
            return Pipeline.Properties.Resources.ResourceManager.GetString("updatePath");
        }
    }
}
