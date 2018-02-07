using System;
using Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MainFormTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ConnectToServer()
        {
            string ExecutablePath = "E:\\Projects\\Visual Studio\\Pipeline\\MainFormTest\\bin\\Debug\\Client.exe";
            MainForm form = new MainForm(ExecutablePath);
            form.Show();
            //form.ConnectToServer();
            form.Close();
            Assert.AreEqual(true, form.isServerOnline);
        }

        [TestInitialize]
        public void RenameConfigFile()
        {
            //const string actualConfigName = "MainFormTest.exe.config";
            //if (!System.IO.File.Exists(actualConfigName))
            //{
            //    System.IO.File.Move("Client.exe.config", actualConfigName);
            //}
        }
    }
}
