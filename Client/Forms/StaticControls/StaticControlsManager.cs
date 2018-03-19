using BllEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.Forms.StaticControls
{
    public class StaticControlsManager
    {
        private readonly StaticControls staticControls;

        const string DATE_FORMAT = "dd.MM.yyyy";
        const string TIME_FORMAT = "HH:mm";

        public StaticControlsManager(StaticControls staticControls)
        {
            this.staticControls = staticControls;
        }

        public void PopulateTextBoxesUsingEvent(BllEvent Event)
        {
            staticControls.SenderTextBox.Text = Event.Sender.Fullname;
            staticControls.TitleTextBox.Text = Event.Name;
            staticControls.DateTextBox.Text = Event.Date.ToString(DATE_FORMAT);
            staticControls.TimeTextBox.Text = Event.Date.ToString(TIME_FORMAT);
            staticControls.DescriptionTextBox.Text = Event.Description;
        }

        public void ClearControls()
        {
            staticControls.SenderTextBox.Text = "";
            staticControls.TitleTextBox.Text = "";
            staticControls.DateTextBox.Text = "";
            staticControls.TimeTextBox.Text = "";
            staticControls.DescriptionTextBox.Clear();
        }
    }
}
