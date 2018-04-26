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

        public static BllSelectedStatus GetCurrentEventStatusWithDate(BllEvent source)
        {
            if (source.StatusLib.SelectedEntities.Count == 0)
            {
                throw new NoItemsInCollectionException();
            }
            return source.StatusLib.SelectedEntities.Last();
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
            if (Event.ReconcilerLib != null)
            {
                foreach (var item in Event.ReconcilerLib.SelectedEntities)
                {
                    if (AreUsersEqual(item.Entity, User) && (item.IsEventReconciled != null))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool AreUsersEqual(BllUser user1, BllUser user2)
        {
            if (user1.Login == user2.Login)
            {
                return true;
            }
            return false;
        }

        public static bool DidUserAcquaintByLogin(BllUser user, List<BllSelectedUser> list)
        {
            foreach (var item in list)
            {
                if (item.Entity.Login == user.Login)
                {
                    if (item.IsEventAccepted)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public static bool IsUserReconciler(BllUser user, BllReconcilerLib lib)
        {
            if (lib != null)
            {
                foreach (var item in lib.SelectedEntities)
                {
                    if (item.Entity.Login == user.Login)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void MarkRecieverInLib(BllUserLib RecieverLib, BllUser reciever)
        {
            foreach (var item in RecieverLib.SelectedEntities)
            {
                if (AreUsersEqual(item.Entity, reciever))
                {
                    item.IsEventAccepted = true;
                    break;
                }
            }
        }

        public static List<UiEvent> CreateSuitableUiEvents(List<BllEvent> events, BllUser user)
        {
            List<UiEvent> suitableEvents = new List<UiEvent>();
            foreach (var item in events)
            {
                suitableEvents.Add(CreateEventAccordingToStatusOrUser(new UiEvent(item, ""), user));
            }
            return suitableEvents;
        }

        public static bool IsEventReconciled(BllEvent Event)
        {
            foreach(var item in Event.ReconcilerLib.SelectedEntities)
            {
                if (item.IsEventReconciled == null)
                {
                    return false;
                }
                else
                {
                    if (item.IsEventReconciled.Value == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static void AdmitReconciledUserInEvent(BllEvent Event, BllUser User, bool agreement)
        {
            foreach(var item in Event.ReconcilerLib.SelectedEntities)
            {
                if (item.Entity.Id == User.Id)
                {
                    item.IsEventReconciled = agreement;
                    break;
                }
            }
        }

        public static bool HasUserReconciled(BllEvent Event, BllUser User)
        {
            if ( AreUsersEqual(Event.Sender, User))
            {
                return true;
            }
            foreach (var item in Event.ReconcilerLib.SelectedEntities)
            {
                if (AreUsersEqual(item.Entity, User) && (item.IsEventReconciled != null))
                {
                        return true;
                }
            }
            return false;
        }
    }


}
