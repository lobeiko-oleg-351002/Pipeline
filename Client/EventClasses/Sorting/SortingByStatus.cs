using Client.EventClasses.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.EventClasses.Sorting
{
    public class SortingByStatus : SortableColumn
    {
        public override void Sort(List<UiEvent> events)
        {
            events.Sort((x, y) => Direction * string.Compare(x.EventData.StatusLib.SelectedEntities.Count != 0 ? GetStatusString(x) : "",
                                                             y.EventData.StatusLib.SelectedEntities.Count != 0 ? GetStatusString(y) : ""));
        }

        private string GetStatusString(UiEvent Event)
        {
            var status = EventHelper.GetCurrentEventStatusWithDate(Event.EventData);
            return status.Entity.Name + " " + status.Date.ToString();
        }
    }
}
