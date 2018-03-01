using BllEntities;
using Client.EventClasses.Events;
using Client.EventClasses.Sorting;
using Client.Misc;
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
    public class UiEventManager : IClientCallBack
    {
        DataGridManager dataGridManager;
        DataGridView dataGridView;
        List<UiEvent> Events;
        MainForm ownerForm;
        public ClientLauncher client { private get; set; }

        const int DIRECTION_MULTIPLIER = -1;
        int SortDirection = 1;
        int SelectedEventNum = -1;
        UiEvent SelectedEvent = null;

        const int CHECK_OTK_STATUS_FREQUENCY_SEC = 60 * 60;
        const int OTK_TESTING_DAYS = 3;

        public UiEventManager(DataGridView dataGridView, MainForm ownerForm)
        {
            this.dataGridView = dataGridView;
            this.ownerForm = ownerForm;
            dataGridManager = new DataGridManager(dataGridView);
            Events = new List<UiEvent>();
        }

        public void GetEvent(BllEvent Event)
        {
            UiEvent newEvent = new NewEvent(Event, "");
            AddNewEvent(newEvent);
            SerializeEvents();            
            Signal.PlaySignalAccordingToEventConfigValue();
            SortEventsUsingLastOrderFromCache();
        }

        private void AddNewEvent(UiEvent Event)
        {
            Events.Add(Event);
            dataGridManager.AddRowToDataGridUsingEvent(dataGridView, Event);
            ownerForm.indication.IncNewEventsCount();
            SortEventsUsingLastOrderFromCache();
        }

        private void SortEventsUsingLastOrderFromCache()
        {
            string sortCol = AppConfigManager.GetKeyValue(Properties.Resources.TAG_SORT_NAME);
            SortableColumn CurrentSorting = dataGridManager.GetSortingUsingColHeader(sortCol);
            CurrentSorting.Direction = AppConfigManager.GetIntKeyValue(Properties.Resources.TAG_SORT_DIR);
            CurrentSorting.Sort(Events);
            dataGridManager.PopulateDataGrid(Events, dataGridView);
        }

        public void SortEventsUsingHeader(string header)
        {
            SortableColumn CurrentSorting = dataGridManager.GetSortingUsingColHeader(header);
            SortDirection *= DIRECTION_MULTIPLIER;
            CurrentSorting.Direction = SortDirection;
            CurrentSorting.Sort(Events);
            dataGridManager.PopulateDataGrid(Events, dataGridView);
            SaveSortColumnAndDirectionToCache(header, SortDirection);
        }

        private void SaveSortColumnAndDirectionToCache(string header, int dir)
        {
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_NAME, header);
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_DIR, dir.ToString());
        }

        public void Ping()
        {
            
        }

        public void UpdateEvent(BllEvent Event)
        {
            int updatedEventNum = FindUpdatedEventInEventSequenceById(Event.Id);
            var updatingEvent = Events[updatedEventNum];
            if (updatedEventNum >= 0)
            {
                if (EventHelper.IsTargetStatusObsolete(Event, updatingEvent.EventData))
                {
                    updatingEvent.EventData = Event;
                    UpdateEventAccordingToCurrentStatus(updatingEvent, updatedEventNum);
                }
                updatingEvent.EventData = Event;
                ownerForm.UpdateSelectedEvent(Event, updatedEventNum);
                SerializeEvents();
            }
        }

        private int FindUpdatedEventInEventSequenceById(int eventId)
        {
            for (int i = 0; i < Events.Count; i++)
            {
                if (Events[i].EventData.Id == eventId)
                {
                    return i;
                }
            }
            return -1;
        }

        private void UpdateEventAccordingToCurrentStatus(UiEvent Event, int rowNum)
        {
            dataGridManager.SetStatusInRow(dataGridView.Rows[rowNum], Event.EventData);
            var previousState = Events[rowNum].EventState;
            Events[rowNum] = EventHelper.CreateEventAccordingToStatusOrUser(Event, ownerForm.GetCurrentUser());
            HandleSwitchEventToRemoved(Events[rowNum], previousState);
            Events[rowNum].SetRowStyle(dataGridView.Rows[rowNum]);
            HandleMissedStatusIndication(Events[rowNum]);
            Events[rowNum].SetMissedStatus(dataGridView.Rows[rowNum], dataGridManager.GetStatusColumnNum());
            Signal.PlaySignalAccordingToStatusConfigValue();
        }

        private void HandleSwitchEventToRemoved(UiEvent currentEvent, EventStates prevState)
        {
            var currentStatus = EventHelper.GetCurrentEventStatus(currentEvent.EventData);
            if (StatusesForOwner.IsStatusForOwner(currentStatus))
            {
                if (prevState == EventStates.NewEvent)
                {
                    ownerForm.indication.DecNewEventsCount();
                }
                DeleteCurrentUserFromRecieversAndUpdateEvent(currentEvent);
            }
        }

        private void DeleteCurrentUserFromRecieversAndUpdateEvent(UiEvent currentEvent)
        {
            var recievers = currentEvent.EventData.RecieverLib.SelectedEntities;
            try
            {
                recievers.Remove(recievers.Single(r => r.Entity.Id == client.GetUser().Id));

                IEventCRUD eventCrud = new EventCRUD(client.GetServerInstance().server);
                eventCrud.UpdateEventRecievers(currentEvent.EventData);
            }
            catch(Exception ex)
            {

            }
        }

        public void DeleteUserInRemovedEvents()
        {
            foreach(var item in Events)
            {
                if (StatusesForOwner.IsStatusForOwner(EventHelper.GetCurrentEventStatus(item.EventData)))
                {
                    DeleteCurrentUserFromRecieversAndUpdateEvent(item);
                }
            }
        }

        private void HandleMissedStatusIndication(UiEvent Event)
        {
            if (Event.MissedStatus)
            {
                ownerForm.indication.InvokeMissedStatusIndication();
            }
            else
            {
                ownerForm.indication.IncNewStatusesCount();
            }
        }

        public void GetEventList()
        {
            if (!client.GetServerInstance().IsConnected())
            {
                Events = SerializeEventManager.DeserializeEventsFromCache();
                dataGridManager.PopulateDataGrid(Events, dataGridView);
            }
            else
            {
                List<BllEvent> eventsFromServer = GetEventsFromServerForCurrentUser();
                List<UiEvent> wrappedEventsFromServer = CreateSuitableUiEvents(eventsFromServer);
                List<UiEvent> cachedEvents = SerializeEventManager.DeserializeEventsFromCache();
                if (cachedEvents != null)
                {
                    AddLocalCachedEventsAndGetUpdateEventsFromServerUsingCache(wrappedEventsFromServer, cachedEvents);
                }
                AddEventsFromServerAndDownloadTheirFiles(wrappedEventsFromServer);
            }
            HideClosedEventsAccordingToConfigValue();
            SortEventsUsingLastOrderFromCache();
            SerializeEvents();
        }

        private List<BllEvent> GetEventsFromServerForCurrentUser()
        {
            bool success = false;
            while (!success)
            {
                success = true;
                try
                {
                    IEventCRUD eventCrud = new EventCRUD(client.GetServerInstance().server);
                    {
                        return eventCrud.GetEventsForUser(ownerForm.GetCurrentUser());
                    }
                }
                catch(Exception ex)
                {
                    success = false;
                    client.PingServerAndIndicateHisStateOnControls();
                }
            }
            return null;
        }

        private List<UiEvent> CreateSuitableUiEvents(List<BllEvent> events)
        {
            List<UiEvent> suitableEvents = new List<UiEvent>();
            foreach (var item in events)
            {
                suitableEvents.Add(EventHelper.CreateEventAccordingToStatusOrUser(new UiEvent(item, ""), ownerForm.GetCurrentUser()));
            }
            return suitableEvents;
        }

        private void AddLocalCachedEventsAndGetUpdateEventsFromServerUsingCache(List<UiEvent> serverEvents, List<UiEvent> cachedEvents)
        {
            for (int i = 0; i < cachedEvents.Count; i++)
            {
                var cachedItem = cachedEvents[i];
                bool isCachedItemMatchsInListFromServer = false;
                for (int j = 0; j < serverEvents.Count; j++)
                {
                    var item = serverEvents[j];
                    if (item.EventData.Id == cachedItem.EventData.Id)
                    {
                        isCachedItemMatchsInListFromServer = true;
                        item.Note = cachedItem.Note;                       
                        if (item.MissedStatus)
                        {
                            ownerForm.indication.IncNewStatusesCount();
                        }

                        if (EventHelper.IsTargetStatusObsolete(item.EventData, cachedItem.EventData))
                        {
                            HandleMissedStatusIndication(item);
                            item.MissedStatus = true;
                        }
                        else
                        {
                            item.MissedStatus = cachedItem.MissedStatus;
                        }

                        try
                        {
                            if (StatusesForOwner.IsStatusForOwner(EventHelper.GetCurrentEventStatus(item.EventData)))
                            {
                                DeleteCurrentUserFromRecieversAndUpdateEvent(item);
                            }
                        }
                        catch(Exception ex)
                        {

                        }
                        item = EventHelper.CreateEventAccordingToStatusOrUser(item, ownerForm.GetCurrentUser());
                        break;
                    }
                }
                if (isCachedItemMatchsInListFromServer == false)
                {
                    cachedItem = EventHelper.CreateEventAccordingToStatusOrUser(cachedItem, ownerForm.GetCurrentUser());
                    AddRegularEvent(cachedItem);
                }
            }
        }

        private void AddRegularEvent(UiEvent Event)
        {
            Events.Add(Event);
            dataGridManager.AddRowToDataGridUsingEvent(dataGridView, Event);
            MakeGreenStatusForOTK(Events.Count-1, GetDateTimeNow());
        }

        private void MakeGreenStatusForOTK(int rowNum, DateTime now)
        {
            try
            {
                var status = Events[rowNum].EventData.StatusLib.SelectedEntities.Last();
                if (status.Entity.Name == "ОТК")
                {
                    if (DateTimeHelper.AreDatesLayInRange(status.Date, now, OTK_TESTING_DAYS))
                    {
                        RowStyleManager.MakeCellGreen(dataGridView.Rows[rowNum].Cells[dataGridManager.GetStatusColumnNum()]);
                    }
                }
            }
            catch
            {

            }
        }

        public void AddNewEventAndSerialize(BllEvent Event)
        {
            UiEvent wrappedEvent = new RegularEvent(Event,  "");
            Events.Add(wrappedEvent);
            dataGridManager.AddRowToDataGridUsingEvent(dataGridView, wrappedEvent);
            SortEventsUsingLastOrderFromCache();
            SerializeEvents();
        }

        private void AddEventsFromServerAndDownloadTheirFiles(List<UiEvent> events)
        {
            foreach (var item in events)
            {
                if (item.EventState == EventStates.NewEvent)
                {
                    AddNewEvent(item);                    
                }
                else
                {
                    AddRegularEvent(item);
                }
                FileDownloader.DownloadEventFilesUsingFilepathLib(item.EventData.FilepathLib);
            }
        }

        public void HideClosedEventsAccordingToConfigValue()
        {
            if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_HIDE_CLOSED))
            {
                int days = AppConfigManager.GetIntKeyValue(Properties.Resources.TAG_HIDE_ALLOWANCE);
                DateTime now = DateTime.Now;
                for (int i = 0; i < Events.Count; i++)
                {
                    if (Events[i].EventState == EventStates.ClosedEvent)
                    {
                        var currentStatusDate = Events[i].EventData.StatusLib.SelectedEntities.Last().Date;
                        if (currentStatusDate.AddDays(days).CompareTo(now) < 0)
                        {
                            dataGridManager.MakeRowInvisible(dataGridView.Rows[i]);
                        }
                        else
                        {
                            dataGridManager.MakeRowVisible(dataGridView.Rows[i]);
                        }
                    }
                }
            }
        }

        public void ShowClosedEvents()
        {
            for (int i = 0; i < Events.Count; i++)
            {
                if (Events[i].EventState == EventStates.ClosedEvent)
                {
                    dataGridView.Rows[i].Visible = true;
                }
            }           
        }

        public UiEvent GetSelectedEvent(int row)
        {
            SelectedEventNum = row;
            SelectedEvent = Events[row];
            RemoveMissedStatus();

            return SelectedEvent;
        }

        private void RemoveMissedStatus()
        {
            if (SelectedEvent.MissedStatus)
            {
                ownerForm.indication.DecNewStatusesCount();
                SelectedEvent.SetRegularStatus(dataGridView.Rows[SelectedEventNum], dataGridManager.GetStatusColumnNum());
                //Events[SelectedEventNum] = SelectedEvent;
                SerializeEvents();
            }
        }

        public void AddStatusToSelectedEvent(BllStatus status)
        {
            SelectedEvent = AddStatusToUiEvent(SelectedEvent, status);
            Events[SelectedEventNum] = SelectedEvent;
            IEventCRUD eventCrud = new EventCRUD(client.GetServerInstance().server);
            SelectedEvent.EventData = eventCrud.UpdateStatusAndSendOutEvent(SelectedEvent.EventData, ownerForm.GetCurrentUser());
            var newStatus = SelectedEvent.EventData.StatusLib.SelectedEntities.Last();
            ownerForm.AddStatusToDataGrid(newStatus.Entity.Name, newStatus.Date);
            dataGridManager.SetStatusInRow(dataGridView.Rows[SelectedEventNum], SelectedEvent.EventData);
            SelectedEvent.SetRowStyle(dataGridView.Rows[SelectedEventNum]);
            ownerForm.SelectBlankStatus();
            SerializeEvents();        
        }

        private UiEvent AddStatusToUiEvent(UiEvent Event, BllStatus status)
        {
            Event.EventData.StatusLib.SelectedEntities.Add(new BllSelectedStatus { Entity = status });
            return EventHelper.CreateEventAccordingToStatusOrUser(Event, ownerForm.GetCurrentUser());
        }

        public void AdmitEventAsAcquainted()
        {
            IEventCRUD crud = new EventCRUD(client.GetServerInstance().server);
            SelectedEvent.EventData = crud.UpdateAcceptedUsersAndSendOutEvent(SelectedEvent.EventData, ownerForm.GetCurrentUser());
            SelectedEvent = new RegularEvent(SelectedEvent);
            Events[SelectedEventNum] = SelectedEvent;
            SelectedEvent.SetRowStyle(dataGridView.Rows[SelectedEventNum]);
            ownerForm.indication.DecNewEventsCount();
            SerializeEvents();
        }

        public void RemoveEvent(int rowNum)
        {
            Events.RemoveAt(rowNum);
            dataGridView.Rows.RemoveAt(rowNum);
            SerializeEvents();
        }

        public void SetEventNote(string note)
        {
            SelectedEvent.Note = note;
            dataGridManager.SetNoteToRow(dataGridView.Rows[SelectedEventNum], note);
        }

        public void SerializeEvents()
        {
            SerializeEventManager.SerializeEventsBackground(Events);
        }

        public void StartCheckoutsOnTimer()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                while (true)
                {
                    HandleStatusesForOTK();
                    Thread.Sleep(CHECK_OTK_STATUS_FREQUENCY_SEC);
                }
            }).Start();
        }

        private void HandleStatusesForOTK()
        {
            if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_OTK_STATUS))
            {
                DateTime now = GetDateTimeNow();
                for (int i = 0; i < Events.Count; i++)
                {
                    MakeGreenStatusForOTK(i, now);
                }
            }
        }

        private DateTime GetDateTimeNow()
        {
            client.PingServerAndIndicateHisStateOnControls();
            if (client.isServerOnline)
            {
                return client.GetServerInstance().server.GetDateTime();
            }
            else
            {
                return DateTime.Now;
            }
        }

    }
}
