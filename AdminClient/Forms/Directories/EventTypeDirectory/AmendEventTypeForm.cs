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
    public partial class AmendEventTypeForm : EventTypeDataForm
    {
        public AmendEventTypeForm()
        {
            InitializeComponent();
        }

        public AmendEventTypeForm(DirectoryForm parent, IBusinessService server, BllEventType entity) : base(parent, server, entity)
        {
            InitializeComponent();
            textBox1.Text = entity.Name;

        }

        protected override void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Введите название", "Оповещение");
            }
            else
            {
                BllEventType EventType = (BllEventType)Entity;
                EventType.Name = textBox1.Text;
                Entity = server.UpdateEventType(EventType);
                base.button1_Click(sender, e);
            }
        }
    }
}
