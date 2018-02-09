using BllEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.EventClasses
{
    public class UiEvent
    {
        public BllEvent EventData { get; set; }
        public EventStates EventState { get; set; }
        public string Note { get; set; }

        public UiEvent(BllEvent Event, bool isNew)
        {
            EventState = EventStates.New;
            EventData = Event;
        }

    }
}
