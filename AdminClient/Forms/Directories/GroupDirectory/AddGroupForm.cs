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

namespace AdminClient.Forms.Directories.GroupDirectory
{
    public partial class AddGroupForm : GroupDataForm
    {
        public AddGroupForm()
        {
            InitializeComponent();
        }

        public AddGroupForm(DirectoryForm parent, IBusinessService server, BllGroup Group) : base(parent, server, Group)
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
                Entity = new BllGroup
                {
                    Name = textBox1.Text,
                };
                Entity = server.CreateGroup((BllGroup)Entity);
                base.button1_Click(sender, e);
            }
        }
    }
}
