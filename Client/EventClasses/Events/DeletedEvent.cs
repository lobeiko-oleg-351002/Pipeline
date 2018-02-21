using BllEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.EventClasses.Events
{
    [Serializable]
    public class DeletedEvent : UiEvent
    {
        public DeletedEvent()
        {

        }

        public DeletedEvent(BllEvent Event, string note) : base(Event, note)
        {
            EventState = EventStates.DeletedEvent;
        }

        public DeletedEvent(UiEvent Event) : base(Event.EventData, Event.Note, Event.MissedStatus)
        {
            EventState = EventStates.DeletedEvent;
        }

        public override void SetRowStyle(DataGridViewRow row)
        {
            RowStyleManager.MakeRowBackgroungAsDeleted(row);
            RowStyleManager.MakeRegularRowFont(row);
        }
    }
}
