using System;
using System.ServiceModel;
using BllEntities;
using Client;
using Client.ServerManager;
using Client.ServerManager.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerInterface;

namespace MainFormTest
{
    [TestClass]
    public class ServerConnectionTest : IClientCallBack
    {
        const string serverIp = "192.168.2.100:8085";

        public void ApproveEvent(BllEvent Event)
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ConnectToServer()
        {
            IServerInstance server = new ServerInstance(serverIp, this);
            server.ConnectToServer();
            server.PingServer();
            Assert.IsTrue(server.IsConnected());
        }

        [TestMethod]
        [ExpectedException(typeof(EndpointNotFoundException))]
        public void ConnectToServerWithWrongIp()
        {
            IServerInstance server = new ServerInstance("123", this);
            server.ConnectToServer();
            server.PingServer();
        }

        public void DisapproveEvent(BllEvent Event)
        {
            throw new NotImplementedException();
        }

        public void GetEvent(BllEvent Event)
        {
            throw new NotImplementedException();
        }

        public void Ping()
        {
            throw new NotImplementedException();
        }

        public void UpdateEvent(BllEvent Event)
        {
            throw new NotImplementedException();
        }
    }
}
