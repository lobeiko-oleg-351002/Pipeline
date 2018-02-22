using BllEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.EventClasses.Events
{
    [Serializable]
    public class NewEvent : UiEvent
    {
        public NewEvent()
        {

        }

        public NewEvent(BllEvent Event, string note) : base(Event, note)
        {
            EventState = EventStates.NewEvent;
        }

        public NewEvent(UiEvent Event) : base(Event.EventData, Event.Note, Event.MissedStatus)
        {
            EventState = EventStates.NewEvent;
        }

        public override void SetRowStyle(DataGridViewRow row)
        {
            RowStyleManager.MakeBoldRowFont(row);
            RowStyleManager.MakeRowBackgroundAsRegular(row);
        }
    }
}
