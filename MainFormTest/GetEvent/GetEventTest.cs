using BllEntities;
using Client;
using Client.Forms;
using MainFormTest.GetEvent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainFormTest
{
    [TestClass]
    public class GetEventTest
    {
        const string DATE_FORMAT = "dd.MM.yyyy";
        const string TIME_FORMAT = "HH:mm";

        [TestMethod]
        public void GetEventPositiveTest()
        {
           // string ExecutablePath = "E:\\Projects\\Visual Studio\\Pipeline\\MainFormTest\\bin\\Debug\\Client.exe";
            TestFormControls testControls = new TestFormControls();
            IFormControllerSet set = new FormControllerSet();
            testControls.InitFormController(set);

            TestEvent testEvent = new TestEvent();
            set.eventManager.clientCallback.GetEvent(testEvent);

            bool isTaskbarFlashing = set.indication.IsFlashing;
            //bool isTrayHighlights = set.indication.trayIconController.currentIconName == Client.Misc.IndicationService.IconName.NewEvent;
            bool isEventSelected = IsEventSelected(testControls, testEvent);
            bool isRowBold = IsRowBold(testControls.dataGridTestControls.dataGridView.Rows[0]);
            Assert.IsTrue(isTaskbarFlashing && isEventSelected && isRowBold);
            
           //isDataContainersPopulatedProperly
           //isProperRowSelected
           //isListSortedProperly
           //HasFilesDownloaded
           //isRowBold
        }

        private bool IsEventSelected(TestFormControls formControllerInitializer, TestEvent testEvent)
        {
            return IsRowPopulatedProperly(formControllerInitializer, testEvent)
                && IsStaticDataControlsPopulatedProperly(formControllerInitializer, testEvent)
                && IsStatusSetProperly(formControllerInitializer, testEvent);
        }

        private bool IsRowPopulatedProperly(TestFormControls formControllerInitializer, TestEvent testEvent)
        {
            var selectedRow = formControllerInitializer.dataGridTestControls.dataGridView.Rows[0];
            selectedRow.Selected = true;

            bool senderMatches = selectedRow.Cells[0].Value.ToString() == testEvent.Sender.Fullname;
            bool titleMatches = selectedRow.Cells[1].Value.ToString() == testEvent.Name;
            bool dateMatches = selectedRow.Cells[2].Value.ToString() == testEvent.Date.ToString(DATE_FORMAT);
            bool timeMatches = selectedRow.Cells[3].Value.ToString() == testEvent.Date.ToString(TIME_FORMAT);
            bool statusMatches = selectedRow.Cells[4].Value.ToString() == GetStatusNameFromEvent(testEvent) + " " + GetStatusDateFromEvent(testEvent);
            bool descriptionMatches = selectedRow.Cells[7].Value.ToString() == testEvent.Description;

            return (senderMatches
                 && titleMatches
                 && dateMatches
                 && timeMatches
                 && statusMatches
                 && descriptionMatches);
        }

        private string GetStatusNameFromEvent(BllEvent Event)
        {
            return Event.StatusLib.SelectedEntities[0].Entity.Name;
        }

        private string GetStatusDateFromEvent(BllEvent Event)
        {
            return Event.StatusLib.SelectedEntities[0].Date.ToString();
        }

        private bool IsStaticDataControlsPopulatedProperly(TestFormControls formControllerInitializer, TestEvent testEvent)
        {
            var controls = formControllerInitializer.staticTestControls;
            bool senderMatches = controls.sender.Text == testEvent.Sender.Fullname;
            bool titleMatches = controls.title.Text == testEvent.Name;
            bool dateMatches = controls.date.Text == testEvent.Date.ToString(DATE_FORMAT);
            bool timeMatches = controls.time.Text == testEvent.Date.ToString(TIME_FORMAT);
            bool descriptionMatches = controls.description.Text == testEvent.Description;
            return (senderMatches
                && titleMatches
                && dateMatches
                && timeMatches
                && descriptionMatches);
        }

        private bool IsStatusSetProperly(TestFormControls formControllerInitializer, TestEvent testEvent)
        {
            var row = formControllerInitializer.statusTestControls.dataGridView.Rows[0];
            bool statusMatches = row.Cells[0].Value.ToString() == GetStatusNameFromEvent(testEvent);
            bool dateMatches = row.Cells[1].Value.ToString() == testEvent.StatusLib.SelectedEntities.Last().Date.ToString();
            return (statusMatches && dateMatches);
        }

        private bool IsRowBold(DataGridViewRow row)
        {
            foreach(DataGridViewCell cell in row.Cells)
            {
                if (!cell.Style.Font.Bold)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
