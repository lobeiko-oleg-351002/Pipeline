using BllEntities;
using ServerInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AdminClient.Forms.Directories.EventTypeDirectory
{
    public partial class AddEventTypeForm : EventTypeDataForm
    {
        public AddEventTypeForm()
        {
            InitializeComponent();
        }

        public AddEventTypeForm(DirectoryForm parent, IBusinessService server, BllEventType EventType) : base(parent, server, EventType)
        {
            InitializeComponent();
        }

        protected override void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Введите название", "Оповещение");
            }
            else
            {
                Entity = new BllEventType
                {
                    Name = textBox1.Text,
                };
                Entity = server.CreateEventType((BllEventType)Entity);
                base.button1_Click(sender, e);
            }
        }
    }
}
