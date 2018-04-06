using BllEntities;
using Client.CustomExceptions;
using Client.EventClasses;
using Client.EventClasses.Events;
using Client.EventClasses.Sorting;
using Client.Forms;
using Client.Forms.DataGridControls;
using Client.Misc;
using Client.Misc.FileService;
using Client.Misc.Serialization;
using Client.ServerManager;
using Client.ServerManager.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Client.Forms.EventControls
{
    public class EventManager
    {
        DataGridPopulationManager dataGridPopulationManager;
        DataGridView dataGridView;
        EventControls eventControls;
        public List<UiEvent> Events { get; private set; }
        public ClientCallback clientCallback;

        const int DIRECTION_MULTIPLIER = -1;
        int SortDirection = 1;
        int SelectedEventNum = -1;
        public UiEvent SelectedEvent = null;

        const int CHECK_OTK_STATUS_FREQUENCY_SEC = 60 * 60;

        public EventManager(EventControls eventControls)
        {
            this.eventControls = eventControls;
            dataGridPopulationManager = eventControls.ControllerSet.dataGridControlsManager.dataGridPopulationManager;
            dataGridView = eventControls.ControllerSet.dataGridControlsManager.dataGridControls.DataGrid;
            clientCallback = new ClientCallback(eventControls.ControllerSet);
            Events = new List<UiEvent>();
        }

        public void AddNewEvent(UiEvent Event)
        {
            Events.Add(Event);
            dataGridPopulationManager.AddRowToDataGridUsingEvent(Event);
            eventControls.ControllerSet.indication.IncNewEventsCount();
            SortEventsUsingLastOrderFromCache();
        }

        public void SortEventsUsingLastOrderFromCache()
        {
            string sortCol = AppConfigManager.GetKeyValue(Properties.Resources.TAG_SORT_NAME);
            SortableColumn CurrentSorting = dataGridPopulationManager.GetSortingUsingColHeader(sortCol);
            CurrentSorting.Direction = AppConfigManager.GetIntKeyValue(Properties.Resources.TAG_SORT_DIR);
            CurrentSorting.Sort(Events);
            dataGridPopulationManager.PopulateDataGrid(Events);
        }

        public void SortEventsUsingHeader(string header)
        {
            SortableColumn CurrentSorting = dataGridPopulationManager.GetSortingUsingColHeader(header);
            SortDirection *= DIRECTION_MULTIPLIER;
            CurrentSorting.Direction = SortDirection;
            CurrentSorting.Sort(Events);
            dataGridPopulationManager.PopulateDataGrid(Events);
            SaveSortColumnAndDirectionToCache(header, SortDirection);
            HideClosedEventsAccordingToConfigValue();
        }

        private void SaveSortColumnAndDirectionToCache(string header, int dir)
        {
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_NAME, header);
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_DIR, dir.ToString());
        }

        public void SetEventNote(string note)
        {
            SelectedEvent.Note = note;
            dataGridPopulationManager.SetNoteToRow(dataGridView.Rows[SelectedEventNum], note);
        }

        public void AdmitEventAsAcquainted()
        {
            IEventCRUD crud = new EventCRUD(eventControls.ControllerSet.client.GetServerInstance().server);
            SelectedEvent.EventData = crud.UpdateAcceptedUsersAndSendOutEvent(SelectedEvent.EventData, eventControls.ControllerSet.client.GetUser());
            SetSelectedEventRegular();
        }

        public void AdmitEventAsApproved()
        {
            IEventCRUD crud = new EventCRUD(eventControls.ControllerSet.client.GetServerInstance().server);
            crud.ApproveAndSendOutEvent(SelectedEvent.EventData);
            SetSelectedEventRegular();
        }

        private void SetSelectedEventRegular()
        {
            SelectedEvent = new RegularEvent(SelectedEvent);
            Events[SelectedEventNum] = SelectedEvent;
            SelectedEvent.SetRowStyle(dataGridView.Rows[SelectedEventNum]);
            eventControls.ControllerSet.indication.DecNewEventsCount();
            SerializeEventsBackground();
        }

        public int GetEventNumById(int eventId)
        {
            for (int i = 0; i < Events.Count; i++)
            {
                if (Events[i].EventData.Id == eventId)
                {
                    return i;
                }
            }
            throw new NoItemsInCollectionException();
        }

        public UiEvent GetEventByRowNum(int row)
        {
            return Events[row];
        }

        public void UpdateEventAccordingToCurrentStatus(UiEvent Event, int rowNum)
        {
            dataGridPopulationManager.SetStatusInRow(dataGridView.Rows[rowNum], Event.EventData);
            var previousState = Events[rowNum].EventState;
            Events[rowNum] = EventHelper.CreateEventAccordingToStatusOrUser(Event, eventControls.ControllerSet.client.GetUser());
            HandleSwitchEventToRemoved(Events[rowNum], previousState);
            Events[rowNum].SetRowStyle(dataGridView.Rows[rowNum]);
            HandleMissedStatusIndication(Events[rowNum]);
            StatusStyleManager.SetMissedStatus(Events[rowNum], dataGridView.Rows[rowNum]);
            Signal.PlaySignalAccordingToStatusConfigValue();
        }

        private void HandleSwitchEventToRemoved(UiEvent currentEvent, EventStates prevState)
        {
            var currentStatus = EventHelper.GetCurrentEventStatus(currentEvent.EventData);
            if (StatusesForOwner.IsStatusForOwner(currentStatus))
            {
                if (prevState == EventStates.NewEvent)
                {
                    eventControls.ControllerSet.indication.DecNewEventsCount();
                }
                DeleteCurrentUserFromRecieversAndUpdateEvent(currentEvent);
            }
        }

        private void DeleteCurrentUserFromRecieversAndUpdateEvent(UiEvent currentEvent)
        {
            var recievers = currentEvent.EventData.RecieverLib.SelectedEntities;
            try
            {
                recievers.Remove(recievers.Single(r => r.Entity.Id == eventControls.ControllerSet.client.GetUser().Id));

                IEventCRUD eventCrud = new EventCRUD(eventControls.ControllerSet.client.GetServerInstance().server);
                eventCrud.UpdateEventRecievers(currentEvent.EventData);
            }
            catch (Exception ex)
            {

            }
        }

        public void DeleteUserInRemovedEvents()
        {
            foreach (var item in Events)
            {
                try
                {
                    if (StatusesForOwner.IsStatusForOwner(EventHelper.GetCurrentEventStatus(item.EventData)))
                    {
                        DeleteCurrentUserFromRecieversAndUpdateEvent(item);
                    }
                }
                catch
                {

                }
            }
        }

        private void HandleMissedStatusIndication(UiEvent Event)
        {
            if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_STOCK_STATUS))
            {
                if (EventHelper.GetCurrentEventStatus(Event.EventData).Name == Globals.Globals.STATUS_STOCK)
                {
                    CheckMissedStatusIndication(Event);
                }
            }
            else
            {
                CheckMissedStatusIndication(Event);
            }
        }

        private void CheckMissedStatusIndication(UiEvent Event)
        {
            if (Event.MissedStatus)
            {
                eventControls.ControllerSet.indication.InvokeMissedStatusIndication();
            }
            else
            {
                eventControls.ControllerSet.indication.IncNewStatusesCount();
            }
        }

        public void GetEventList()
        {
            Serializer serializer = new Serializer();
            if (!eventControls.ControllerSet.client.GetServerInstance().IsConnected())
            {
                Events = serializer.DeserializeEventsFromCache();
                dataGridPopulationManager.PopulateDataGrid(Events);
            }
            else
            {
                List<BllEvent> eventsFromServer = GetEventsFromServerForCurrentUser();
                List<UiEvent> uiEventsFromServer = EventHelper.CreateSuitableUiEvents(eventsFromServer, eventControls.ControllerSet.client.GetUser());
                List<UiEvent> cachedEvents = serializer.DeserializeEventsFromCache();
                AddLocalCachedEventsAndGetUpdateEventsFromServerUsingCache(uiEventsFromServer, cachedEvents);
                AddEventsFromServerAndDownloadTheirFiles(uiEventsFromServer);
            }
            HideClosedEventsAccordingToConfigValue();
            SortEventsUsingLastOrderFromCache();
            SerializeEventsBackground();
        }

        private List<BllEvent> GetEventsFromServerForCurrentUser()
        {
            bool success = false;
            while (!success)
            {
                success = true;
                try
                {
                    IEventCRUD eventCrud = new EventCRUD(eventControls.ControllerSet.client.GetServerInstance().server);
                    {
                        return eventCrud.GetEventsForUser(eventControls.ControllerSet.client.GetUser());
                    }
                }
                catch (Exception ex)
                {
                    success = false;
                    eventControls.ControllerSet.client.PingServerAndIndicateHisStateOnControls();
                }
            }
            return null;
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
                        UpdateCachedEventUsingEventFromServer(item, cachedItem);
                        break;
                    }
                }
                if (isCachedItemMatchsInListFromServer == false)
                {
                    cachedItem = EventHelper.CreateEventAccordingToStatusOrUser(cachedItem, eventControls.ControllerSet.client.GetUser());
                    AddRegularEvent(cachedItem);
                }
            }
        }

        private void UpdateCachedEventUsingEventFromServer(UiEvent serverEvent, UiEvent cachedEvent)
        {
            serverEvent.Note = cachedEvent.Note;

            if (EventHelper.IsTargetStatusObsolete(serverEvent.EventData, cachedEvent.EventData))
            {
                HandleMissedStatusIndication(serverEvent);
                serverEvent.MissedStatus = true;
            }
            else
            {
                serverEvent.MissedStatus = cachedEvent.MissedStatus;
            }

            if (serverEvent.MissedStatus)
            {
                eventControls.ControllerSet.indication.IncNewStatusesCount();
            }

            try
            {
                if (StatusesForOwner.IsStatusForOwner(EventHelper.GetCurrentEventStatus(serverEvent.EventData)))
                {
                    DeleteCurrentUserFromRecieversAndUpdateEvent(serverEvent);
                }
            }
            catch (Exception ex)
            {

            }
            serverEvent = EventHelper.CreateEventAccordingToStatusOrUser(serverEvent, eventControls.ControllerSet.client.GetUser());
        }

        private void AddRegularEvent(UiEvent Event)
        {
            Events.Add(Event);
            dataGridPopulationManager.AddRowToDataGridUsingEvent(Event);
            StatusStyleManager.MakeGreenStatusForOTK(Event, dataGridView.Rows[Events.Count - 1], DateTimeHelper.GetDateTimeNow(eventControls.ControllerSet.client));
        }

        public void AddNewEventAndSerialize(BllEvent Event)
        {
            UiEvent wrappedEvent = new RegularEvent(Event, "");
            Events.Add(wrappedEvent);
            dataGridPopulationManager.AddRowToDataGridUsingEvent(wrappedEvent);
            SortEventsUsingLastOrderFromCache();
            HideClosedEventsAccordingToConfigValue();
            SerializeEventsBackground();
        }

        private void AddEventsFromServerAndDownloadTheirFiles(List<UiEvent> events)
        {
            foreach (var item in events)
            {
                try
                {
                    if (StatusesForOwner.IsStatusForOwner(EventHelper.GetCurrentEventStatus(item.EventData)))
                    {
                        DeleteCurrentUserFromRecieversAndUpdateEvent(item);
                    }
                }
                catch { }

                if (item.EventState == EventStates.NewEvent)
                {
                    AddNewEvent(item);
                }
                else
                {
                    AddRegularEvent(item);
                }
                FileDownloader fileDownloader = new FileDownloader();
                fileDownloader.DownloadEventFilesUsingFilepathLib(item.EventData.FilepathLib);
            }
        }

        public void HideClosedEventsAccordingToConfigValue()
        {
            if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_HIDE_CLOSED))
            {
                int days = AppConfigManager.GetIntKeyValue(Properties.Resources.TAG_HIDE_ALLOWANCE);
                DateTime now = DateTimeHelper.GetDateTimeNow(eventControls.ControllerSet.client);
                for (int i = 0; i < Events.Count; i++)
                {
                    if ((Events[i].EventState == EventStates.ClosedEvent) && (Events[i].MissedStatus == false))
                    {
                        var currentStatusDate = EventHelper.GetCurrentEventStatusWithDate(Events[i].EventData).Date;
                        if (currentStatusDate.AddDays(days).CompareTo(now) < 0)
                        {
                            dataGridPopulationManager.MakeRowInvisible(dataGridView.Rows[i]);
                        }
                        else
                        {
                            dataGridPopulationManager.MakeRowVisible(dataGridView.Rows[i]);
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

        public UiEvent SetSelectedEventUsingNum(int row)
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
                eventControls.ControllerSet.indication.DecNewStatusesCount();
                SelectedEvent.SetRegularStatus(dataGridView.Rows[SelectedEventNum], DataGridPopulationManager.GetStatusColumnNum());
                SerializeEventsBackground();
            }
        }

        public void AddStatusToSelectedEventAndIndicateHim(BllStatus status)
        {
            SelectedEvent = AddStatusToUiEvent(SelectedEvent, status);
            Events[SelectedEventNum] = SelectedEvent;

            IEventCRUD eventCrud = new EventCRUD(eventControls.ControllerSet.client.GetServerInstance().server);
            SelectedEvent.EventData = eventCrud.UpdateStatusAndSendOutEvent(SelectedEvent.EventData, eventControls.ControllerSet.client.GetUser());

            IndicateNewStatus();

            eventControls.ControllerSet.statusControlsManager.SelectBlankStatus();
            SerializeEventsBackground();
        }

        private UiEvent AddStatusToUiEvent(UiEvent Event, BllStatus status)
        {
            Event.EventData.StatusLib.SelectedEntities.Add(new BllSelectedStatus { Entity = status });
            return EventHelper.CreateEventAccordingToStatusOrUser(Event, eventControls.ControllerSet.client.GetUser());
        }

        private void IndicateNewStatus()
        {
            var newStatus = EventHelper.GetCurrentEventStatusWithDate(SelectedEvent.EventData);
            eventControls.ControllerSet.statusControlsManager.AddStatusToDataGrid(newStatus.Entity.Name, newStatus.Date);
            dataGridPopulationManager.SetStatusInRow(dataGridView.Rows[SelectedEventNum], SelectedEvent.EventData);
            SelectedEvent.SetRowStyle(dataGridView.Rows[SelectedEventNum]);
        }

        public void RemoveEvent(int rowNum)
        {
            Events.RemoveAt(rowNum);
            dataGridView.Rows.RemoveAt(rowNum);
            SerializeEventsBackground();
        }

        public void SerializeEventsBackground()
        {
            Serializer serializer = new Serializer();
            serializer.SerializeEventsBackground(Events);
        }

        public void SerializeEvents()
        {
            Serializer serializer = new Serializer();
            serializer.SerializeUiEventsToCache(Events);
        }

        public void StartOtkStatusCheckoutsOnTimer()
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
                DateTime now = DateTimeHelper.GetDateTimeNow(eventControls.ControllerSet.client);
                for (int i = 0; i < Events.Count; i++)
                {
                    StatusStyleManager.MakeGreenStatusForOTK(Events[i], dataGridView.Rows[i], now);
                }
            }
        }

    }
}
