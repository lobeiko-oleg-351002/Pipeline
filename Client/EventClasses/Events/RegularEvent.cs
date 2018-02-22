using BllEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.EventClasses.Events
{
    [Serializable]
    public class RegularEvent : UiEvent
    {
        public RegularEvent()
        {

        }

        public RegularEvent(BllEvent Event, string note) : base(Event, note)
        {
            EventState = EventStates.RegularEvent;
        }

        public RegularEvent(UiEvent Event) : base(Event.EventData, Event.Note, Event.MissedStatus)
        {
            EventState = EventStates.RegularEvent;
        }

        public override void SetRowStyle(DataGridViewRow row)
        {
            RowStyleManager.MakeRegularRowFont(row);
            RowStyleManager.MakeRowBackgroundAsRegular(row);
        }
    }
}
