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
            events.Sort((x, y) => Direction * string.Compare(x.EventData.StatusLib.SelectedEntities.Count != 0 ? x.EventData.StatusLib.SelectedEntities.Last().Entity.Name : "",
                                                             y.EventData.StatusLib.SelectedEntities.Count != 0 ? y.EventData.StatusLib.SelectedEntities.Last().Entity.Name : ""));
        }
    }
}
