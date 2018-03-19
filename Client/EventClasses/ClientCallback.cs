using BllEntities;
using Client.EventClasses.Events;
using Client.EventClasses.Sorting;
using Client.Forms;
using Client.Forms.EventControls;
using Client.Misc;
using Client.Misc.FileService;
using Client.Misc.Serialization;
using Client.ServerManager;
using Client.ServerManager.Interface;
using ServerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Client.EventClasses
{
    public class ClientCallback : IClientCallBack
    {
        EventManager eventManager;
        IFormControllerSet formControllerSet;
        public ClientCallback(IFormControllerSet set)
        {
            this.eventManager = set.eventManager;
            formControllerSet = set;
        }

        public void GetEvent(BllEvent Event)
        {
            UiEvent newEvent = new NewEvent(Event, "");
            eventManager.AddNewEvent(newEvent);
            eventManager.SerializeEvents();            
            Signal.PlaySignalAccordingToEventConfigValue();
            eventManager.SortEventsUsingLastOrderFromCache();
            eventManager.HideClosedEventsAccordingToConfigValue();
        }

        public void Ping()
        {
            
        }

        public void UpdateEvent(BllEvent Event)
        {
            int row = eventManager.GetEventNumById(Event.Id);
            UiEvent updatingEvent = eventManager.GetEventByRowNum(row);
            if (EventHelper.IsTargetStatusObsolete(Event, updatingEvent.EventData))
            {
                updatingEvent.EventData = Event;
                eventManager.UpdateEventAccordingToCurrentStatus(updatingEvent, row);
            }
            updatingEvent.EventData = Event;
            formControllerSet.dataGridControlsManager.UpdateSelectedEvent(Event, row);
            eventManager.SerializeEvents();           
        }
    }
}
