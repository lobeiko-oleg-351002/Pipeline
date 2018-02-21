using Client.EventClasses.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.EventClasses.Sorting
{
    public class SortingByName : SortableColumn
    {
        public override void Sort(List<UiEvent> events)
        {
            events.Sort((x, y) => Direction * string.Compare(x.EventData.Name, y.EventData.Name));
        }
    }
}
