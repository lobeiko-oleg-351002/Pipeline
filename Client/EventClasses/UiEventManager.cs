using BllEntities;
using Client.Misc;
using ServerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.EventClasses
{
    public class UiEventManager : IClientCallBack
    {
        DataGridManager dataGridManager;
        DataGridView dataGridView;
        List<UiEvent> Events;

        public UiEventManager(DataGridView dataGridView)
        {
            this.dataGridView = dataGridView;
            dataGridManager = new DataGridManager();
            dataGridManager.InitDataGridView(dataGridView);
            Events = new List<UiEvent>();
        }

        public void GetEvent(BllEvent Event)
        {
            Events.Add(new UiEvent(Event, true));
            dataGridManager.AddRowToDataGridUsingEvent(dataGridView, Event);
            SerializeEventManager.SerializeEventsBackground(Events);
            IndicateNewEventsOnTaskbar();
            TurnOutWithEventFormIfHidden();
            DealWithTrayIcon();
            PlaySignalAccordingToEventConfigValue();
       
            CurrentSorting.Sort();
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
