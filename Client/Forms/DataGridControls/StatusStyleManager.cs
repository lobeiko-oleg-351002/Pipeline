using Client.EventClasses;
using Client.EventClasses.Events;
using Client.Misc;
using Client.ServerManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Forms.DataGridControls
{
    public static class StatusStyleManager
    {
        const int OTK_TESTING_DAYS = 3;

        public static void SetStatusStyle(UiEvent Event, DataGridViewRow row)
        {
            try
            {
                if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_STOCK_STATUS))
                {
                    if (EventHelper.GetCurrentEventStatus(Event.EventData).Name == Globals.Globals.STATUS_STOCK)
                    {
                        if (Event.MissedStatus)
                        {
                            Event.SetMissedStatus(row, DataGridPopulationManager.GetStatusColumnNum());
                        }
                    }
                }
                else
                {
                    if (Event.MissedStatus)
                    {
                        Event.SetMissedStatus(row, DataGridPopulationManager.GetStatusColumnNum());
                    }
                }
            }
            catch
            {

            }
        }

        public static void SetMissedStatus(UiEvent Event, DataGridViewRow row)
        {
            try
            {
                if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_STOCK_STATUS))
                {
                    if (EventHelper.GetCurrentEventStatus(Event.EventData).Name == Globals.Globals.STATUS_STOCK)
                    {
                        Event.SetMissedStatus(row, DataGridPopulationManager.GetStatusColumnNum());
                    }
                }
                else
                {
                    Event.SetMissedStatus(row, DataGridPopulationManager.GetStatusColumnNum());                    
                }
            }
            catch
            {

            }
        }

        public static void MakeGreenStatusForOTK(UiEvent Event, DataGridViewRow row, DateTime now)
        {
            try
            {
                var status = Event.EventData.StatusLib.SelectedEntities.Last();
                if (status.Entity.Name == "ОТК")
                {
                    if (DateTimeHelper.AreDatesLayInRange(status.Date, now, OTK_TESTING_DAYS))
                    {
                        RowStyleManager.MakeCellGreen(row.Cells[DataGridPopulationManager.GetStatusColumnNum()]);
                    }
                }
            }
            catch
            {

            }
        }
    }
}
