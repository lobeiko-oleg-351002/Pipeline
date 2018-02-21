using BllEntities;
using Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainFormTest
{
    [TestClass]
    public class GetEventTest
    {
        [TestMethod]
        public void GetEventPositiveTest()
        {
            string ExecutablePath = "E:\\Projects\\Visual Studio\\Pipeline\\MainFormTest\\bin\\Debug\\Client.exe";
            MainForm mainForm = new MainForm(ExecutablePath);

            BllEvent testEvent = CreateTestEvent();
           // mainForm.GetEvent(testEvent);
            //isTaskBarHighliths
            //isTrayHighlights
            //isDataContainersPopulatedProperly
            //isProperRowSelected
            //isListSortedProperly
            //HasFilesDownloaded
            //isRowBold
        }

        private BllEvent CreateTestEvent()
        {
            return new BllEvent
            {
                Id = 1,
                AttributeLib = CreateAttributeLib(),
                Date = DateTime.Now,
                Description = "test",
                FilepathLib = CreateFilepathLib(),
                Name = "TestEvent1",
                RecieverLib = CreateUserLib(),
                StatusLib = CreateStatusLib(),
                Sender = CreateUser("sender"),
                Type = CreateEventType("eventtype")
            };
        }

        private BllAttributeLib CreateAttributeLib()
        {
            BllAttributeLib attributeLib = new BllAttributeLib();
            attributeLib.SelectedEntities.Add(new BllSelectedEntity<BllAttribute> { Entity = CreateAttribute("test1") });
            return attributeLib;
        }

        private BllFilepathLib CreateFilepathLib()
        {
            BllFilepathLib filepathLib = new BllFilepathLib();
            return filepathLib;
        }

        private BllUserLib CreateUserLib()
        {
            BllUserLib userLib = new BllUserLib();
            userLib.SelectedEntities.Add(new BllSelectedUser { Entity = CreateUser("test user") });
            return userLib;
        }

        private BllStatusLib CreateStatusLib()
        {
            BllStatusLib StatusLib = new BllStatusLib();
            StatusLib.SelectedEntities.Add(new BllSelectedStatus { Entity = CreateStatus("test Status") });
            return StatusLib;
        }

        private BllAttribute CreateAttribute(string text)
        {
            return new BllAttribute
            {
                Name = text
            };
        }

        private BllUser CreateUser(string text)
        {
            return new BllUser
            {
                Fullname = text
            };
        }

        private BllStatus CreateStatus(string text)
        {
            return new BllStatus
            {
                Name = text
            };
        }

        private BllEventType CreateEventType(string text)
        {
            return new BllEventType
            {
                Name = text
            };
        }
    }
}
