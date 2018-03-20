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
        IFormControllerSet formControllerSet;
        public ClientCallback(IFormControllerSet set)
        {
            formControllerSet = set;
        }

        public void GetEvent(BllEvent Event)
        {
            UiEvent newEvent = new NewEvent(Event, "");
            formControllerSet.eventManager.AddNewEvent(newEvent);
            formControllerSet.eventManager.SerializeEvents();            
            Signal.PlaySignalAccordingToEventConfigValue();
            formControllerSet.eventManager.SortEventsUsingLastOrderFromCache();
            formControllerSet.eventManager.HideClosedEventsAccordingToConfigValue();
        }

        public void Ping()
        {
            
        }

        public void UpdateEvent(BllEvent Event)
        {
            int row = formControllerSet.eventManager.GetEventNumById(Event.Id);
            UiEvent updatingEvent = formControllerSet.eventManager.GetEventByRowNum(row);
            if (EventHelper.IsTargetStatusObsolete(Event, updatingEvent.EventData))
            {
                updatingEvent.EventData = Event;
                formControllerSet.eventManager.UpdateEventAccordingToCurrentStatus(updatingEvent, row);
            }
            updatingEvent.EventData = Event;
            formControllerSet.dataGridControlsManager.UpdateSelectedEvent(Event, row);
            formControllerSet.eventManager.SerializeEvents();           
        }
    }
}
