using BllEntities;
using Client.EventClasses;
using Client.Misc;
using Client.ServerManager;
using ServerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Client.ServerManager
{
    public class ClientLauncher
    {
        const string IP_KEY = "hostIP";
        const int PING_SLEEPTIME_MS = 10000;

        public bool isServerOnline { get; private set; }
        ServerInstance serverInstance;
        IClientCallBack callback;
        Authorization authorization;
        MainForm ownerForm;

        public ClientLauncher(IClientCallBack clientCallBack, MainForm ownerForm)
        {
            callback = clientCallBack;
            this.ownerForm = ownerForm;
            SetServerInstance();
            authorization = new Authorization();
        }

        public void Launch()
        {
            try
            {
                PingServerAndIndicateHisStateOnControls();
                StartPingingServerBackground();
            }
            catch (UserIsNullException ex) //user has not logged in
            {
                throw ex;
            }
        }

        private void StartPingingServerBackground()
        {
            new Thread(() =>
            {
                while (true)
                {
                    Thread.CurrentThread.IsBackground = true;
                    PingServerAndIndicateHisStateOnControls();
                    Thread.Sleep(PING_SLEEPTIME_MS);
                }
            }).Start();
        }

        private void SetServerInstance()
        {
            string ip = AppConfigManager.GetKeyValue(IP_KEY);
            //string ip = "192.168.2.100:8085";
            serverInstance = new ServerInstance(ip, callback);
        }

        private void ConnectToServerAndAuthorize()
        {
            try
            {                
                serverInstance.ConnectToServer();
                authorization.Authorize(serverInstance);
            }
            catch (ConnectionFailedException ex)
            {
                throw ex;
            }
            catch (UserIsNullException ex)
            {
                throw ex;
            }
        }

        public void PingServerAndIndicateHisStateOnControls() //fix rename
        {
            try
            {
                if (isServerOnline == false)
                {
                    ConnectToServerAndAuthorize();
                    ownerForm.SetControlsAccordingToServerOnline();
                    isServerOnline = true;
                }
                else
                {
                    serverInstance.PingServer();
                }
            }
            catch (Exception ex)
            {
                if (isServerOnline == true)
                {
                    isServerOnline = false;
                    ownerForm.SetControlsAccordingToServerOffline();
                }
            }
        }

        public ServerInstance GetServerInstance()
        {
            return serverInstance;
        }

        public BllUser GetUser()
        {
            return authorization.User;
        }
    }
}
