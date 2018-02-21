using Client.EventClasses.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.EventClasses.Sorting
{
    public class SortingByDate : SortableColumn
    {
        public override void Sort(List<UiEvent> events)
        {
            events.Sort((x, y) => Direction * DateTime.Compare(x.EventData.Date, y.EventData.Date));
        }
    }
}
