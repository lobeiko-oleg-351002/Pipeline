using Client.EventClasses.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.EventClasses.Sorting
{
    public class SortingByAttribute : SortableColumn
    {
        public override void Sort(List<UiEvent> events)
        {
            events.Sort((x, y) => Direction * (x.EventData.AttributeLib.SelectedEntities.Count - y.EventData.AttributeLib.SelectedEntities.Count));
        }
    }
}
