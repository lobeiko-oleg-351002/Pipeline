using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Forms.StaticControls
{
    public class StaticControls : FormControls
    {
        public readonly TextBox SenderTextBox;
        public readonly TextBox DateTextBox;
        public readonly TextBox TimeTextBox;
        public readonly TextBox TitleTextBox;
        public readonly RichTextBox DescriptionTextBox;

        public StaticControls(TextBox SenderTextBox, TextBox DateTextBox, TextBox TimeTextBox, TextBox TitleTextBox,
            RichTextBox DescriptionTextBox, IFormControllerSet parent) : base(parent)
        {
            this.SenderTextBox = SenderTextBox;
            this.TimeTextBox = TimeTextBox;
            this.DateTextBox = DateTextBox;
            this.TitleTextBox = TitleTextBox;
            this.DescriptionTextBox = DescriptionTextBox;
        }
    }
}
