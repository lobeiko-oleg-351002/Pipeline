using BllEntities;
using Globals;
using ServerInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AdminClient.Forms.Directories.StatusDirectory
{
    public partial class AddStatusForm : StatusDataForm
    {
        public AddStatusForm()
        {
            InitializeComponent();
        }

        public AddStatusForm(DirectoryForm parent, IBusinessService server, BllStatus status) : base(parent, server, status)
        {
            InitializeComponent();
        }

        protected override void button1_Click(object sender, EventArgs e)
        {
            if ((textBox1.Text == Globals.Globals.STATUS_CLOSED) || (textBox1.Text == Globals.Globals.STATUS_DELETED))
            {
                MessageBox.Show("Такие статусы уже существуют");
            }
            if (textBox1.Text == "")
            {
                MessageBox.Show("Введите название", "Оповещение");
            }
            else
            {
                Entity = new BllStatus
                {
                    Name = textBox1.Text,
                };
                Entity = server.CreateStatus((BllStatus)Entity);
                base.button1_Click(sender, e);
            }
        }
    }
}
