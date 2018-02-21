using Client.EventClasses.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.EventClasses.Sorting
{
    public abstract class SortableColumn
    {
        public int Direction { get; set; }
        public abstract void Sort(List<UiEvent> events);
    }
}
