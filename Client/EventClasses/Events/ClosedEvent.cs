using BllEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.EventClasses.Events
{
    [Serializable]
    public class ClosedEvent : UiEvent
    {
        public ClosedEvent()
        {

        }

        public ClosedEvent(BllEvent Event, string note) : base(Event, note)
        {
            EventState = EventStates.ClosedEvent;
        }

        public ClosedEvent(UiEvent Event) : base(Event.EventData, Event.Note, Event.MissedStatus)
        {
            EventState = EventStates.ClosedEvent;
        }

        public override void SetRowStyle(DataGridViewRow row)
        {
            RowStyleManager.MakeRowBackgroungAsClosed(row);
            RowStyleManager.MakeRegularRowFont(row);
        }
    }
}
