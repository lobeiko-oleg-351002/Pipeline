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

namespace AdminClient.Forms.Directories.UserDirectory
{
    public partial class AmendUserForm : UserDataForm
    {
        public AmendUserForm()
        {
            InitializeComponent();
        }

        public AmendUserForm(DirectoryForm parent, IBusinessService server, BllUser entity) : base(parent, server, entity)
        {
            InitializeComponent();
            textBox1.Text = entity.Fullname;
            textBox2.Text = entity.Login;
            checkedListBox1.SetItemChecked(0, entity.CreateRights);
            checkedListBox1.SetItemChecked(1, entity.ChangeRights);
            comboBox1.SelectedItem = entity.Group.Name;

        }

        protected override void button1_Click(object sender, EventArgs e)
        {
            BllUser User = (BllUser)Entity;

            User.Fullname = textBox1.Text;
            User.Login = textBox2.Text;
            User.Password = Sha1.Encrypt(textBox3.Text);
            User.CreateRights = checkedListBox1.CheckedIndices.Contains(0);
            User.ChangeRights = checkedListBox1.CheckedIndices.Contains(1);
            User.Group = Groups[comboBox1.SelectedIndex];
            Entity = server.UpdateUser(User);
            base.button1_Click(sender, e);
            
        }
    }
}
