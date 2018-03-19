using BllEntities;
using Client.Forms.DataGridControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Client.EventClasses.Events
{
    [Serializable, XmlInclude(typeof(NewEvent)), XmlInclude(typeof(ClosedEvent)), XmlInclude(typeof(DeletedEvent)), XmlInclude(typeof(RegularEvent))]
    public class UiEvent
    {
        public BllEvent EventData { get; set; }
        public string Note { get; set; }
        public EventStates EventState { get; set; }
        public bool MissedStatus { get; set; }

        public UiEvent()
        {

        }

        public UiEvent(BllEvent Event,  string note, bool MissedStatus)
        {
            EventData = Event;
            Note = note;
            this.MissedStatus = MissedStatus;
        }

        public UiEvent(BllEvent Event, string note)
        {
            EventData = Event;
            Note = note;
        }

        public virtual void SetRowStyle(DataGridViewRow row)
        {

        }

        public void SetMissedStatus(DataGridViewRow row, int cellColNum)
        {
            MissedStatus = true;
            RowStyleManager.MakeCellBoldFont(row.Cells[cellColNum]);
        }

        public void SetRegularStatus(DataGridViewRow row, int cellColNum)
        {
            MissedStatus = false;
            RowStyleManager.MakeCellRegularFont(row.Cells[cellColNum]);
        }

    }
}
