using BllEntities;
using Client.CustomExceptions;
using Client.EventClasses.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.EventClasses
{
    public static class EventHelper
    {
        public static bool IsTargetStatusObsolete(BllEvent source, BllEvent target)
        {
            if (source.StatusLib.SelectedEntities.Count > 0)
            {
                var selectedStatuses = target.StatusLib.SelectedEntities;
                var newstatus = source.StatusLib.SelectedEntities.Last();
                if (selectedStatuses.Count > 0)
                {
                    var oldstatus = selectedStatuses.Last();
                    if (newstatus.Date != oldstatus.Date)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public static BllStatus GetCurrentEventStatus(BllEvent source)
        {
            if (source.StatusLib.SelectedEntities.Count == 0)
            {
                throw new NoItemsInCollectionException();
            }
            return source.StatusLib.SelectedEntities.Last().Entity;
        }

        public static UiEvent CreateEventAccordingToStatusOrUser(UiEvent data, BllUser client)
        {
            try
            {
                var currentStatus = GetCurrentEventStatus(data.EventData);
                if (currentStatus.Id == StatusesForOwner.StatusDELETED.Id)
                {
                    return new DeletedEvent(data);
                }
                if (currentStatus.Id == StatusesForOwner.StatusCLOSED.Id)
                {
                    return new ClosedEvent(data);
                }
            }
            catch(NoItemsInCollectionException)
            {

            }
            if (!IsEventAcceptedByUser(data.EventData, client))
            {
                return new NewEvent(data);
            }
            return new RegularEvent(data);
        }

        public static bool IsEventAcceptedByUser(BllEvent Event, BllUser User)
        {
            foreach (var item in Event.RecieverLib.SelectedEntities)
            {
                if (AreUsersEqual(item.Entity, User) && item.IsEventAccepted)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool AreUsersEqual(BllUser user1, BllUser user2)
        {
            if (user1.Login == user2.Login)
            {
                return true;
            }
            return false;
        }
    }


}
