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

namespace AdminClient.Forms.Directories.StatusDirectory
{
    public partial class AmendStatusForm : StatusDataForm
    {
        public AmendStatusForm()
        {
            InitializeComponent();
        }

        public AmendStatusForm(DirectoryForm parent, IBusinessService server, BllStatus entity) : base(parent, server, entity)
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
                BllStatus Status = (BllStatus)Entity;
                Status.Name = textBox1.Text;
                Entity = server.UpdateStatus(Status);
                base.button1_Click(sender, e);
            }
        }
    }
}
