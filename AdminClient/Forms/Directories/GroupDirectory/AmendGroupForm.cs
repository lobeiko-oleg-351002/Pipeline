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
    public partial class AmendGroupForm : GroupDataForm
    {
        public AmendGroupForm()
        {
            InitializeComponent();
        }

        public AmendGroupForm(DirectoryForm parent, IBusinessService server, BllGroup entity) : base(parent, server, entity)
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
                BllGroup Group = (BllGroup)Entity;
                Group.Name = textBox1.Text;
                Entity = server.UpdateGroup(Group);
                base.button1_Click(sender, e);
            }
        }
    }
}
