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

        public void DisapproveEvent(BllEvent Event)
        {
            try
            {
                int row = formControllerSet.eventManager.GetEventNumById(Event.Id);
                UiEvent updatingEvent = formControllerSet.eventManager.GetEventByRowNum(row);
                updatingEvent.EventData = Event;
                formControllerSet.dataGridControlsManager.SetDisapproveMark(row);
                formControllerSet.eventManager.SerializeEventsBackground();
            }
            catch(Exception ex)
            {
                LogWriter.WriteMessage("DissaproveEvent", ex.Message, Event.Name);
            }
        }

        public void ApproveEvent(BllEvent Event)
        {
            try
            {
                int row = formControllerSet.eventManager.GetEventNumById(Event.Id);
                UiEvent updatingEvent = formControllerSet.eventManager.GetEventByRowNum(row);
                updatingEvent.EventData = Event;
                formControllerSet.dataGridControlsManager.UpdateSelectedEvent(Event, row);
                formControllerSet.dataGridControlsManager.SetApproverToRow(row);
                formControllerSet.eventManager.SerializeEventsBackground();
            }
            catch (Exception ex)
            {
                LogWriter.WriteMessage("ApproveEvent", ex.Message, Event.Name);
            }
        }

        public void GetEvent(BllEvent Event)
        {
            try
            {
                UiEvent newEvent = new NewEvent(Event, "");
                formControllerSet.eventManager.AddNewEvent(newEvent);
                formControllerSet.eventManager.SerializeEventsBackground();
                Signal.PlaySignalAccordingToEventConfigValue();
                formControllerSet.eventManager.SortEventsUsingLastOrderFromCache();
                formControllerSet.eventManager.HideClosedEventsAccordingToConfigValue();
            }
            catch (Exception ex)
            {
                LogWriter.WriteMessage("GetEvent", ex.Message, Event.Name);
            }
        }

        public void Ping()
        {
            
        }

        public void UpdateEvent(BllEvent Event)
        {
            try
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
                formControllerSet.eventManager.SerializeEventsBackground();
            }
            catch(Exception ex)
            {
                LogWriter.WriteMessage("UpdateEvent", ex.Message, Event.Name);
            }
        }
    }
}
