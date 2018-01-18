using BllEntities;
using BllEntities.Interface;
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
    public partial class UserDirectoryForm : DirectoryForm
    {
        List<BllUser> Users;

        public UserDirectoryForm() : base(null)
        {
            InitializeComponent();
        }

        public UserDirectoryForm(IBusinessService server) : base(server)
        {
            InitializeComponent();
            RefreshData();
        }

        public override void RefreshData()
        {
            dataGridView1.Rows.Clear();
            Users = server.GetAllUsers();
            foreach (var User in Users)
            {
                AddNewRow(User);
            }
        }

        protected override void button1_Click(object sender, EventArgs e)
        {
            AddUserForm addUserForm = new AddUserForm(this, server, null);
            addUserForm.ShowDialog(this);
            if (addUserForm.Entity != null)
            {
                AddNewRow(addUserForm.Entity);
                Users.Add((BllUser)addUserForm.Entity);
            }
        }

        protected override void button2_Click(object sender, EventArgs e)
        {
            int i = dataGridView1.SelectedRows[0].Index;
            AmendUserForm amendUserForm = new AmendUserForm(this, server, Users[i]);
            amendUserForm.ShowDialog(this);
            UpdateRow(amendUserForm.Entity, i);
        }

        protected override void button3_Click(object sender, EventArgs e)
        {
            int i = dataGridView1.SelectedRows[0].Index;
            server.DeleteUser(Users[i].Id);
            dataGridView1.Rows.RemoveAt(i);
            Users.RemoveAt(i);
            base.button3_Click(sender, e);
        }

        protected override void UpdateRow(IBllEntity entity, int row)
        {
            BllUser data = (BllUser)entity;
            Users[row] = data;
            FillRow(data, dataGridView1.Rows[row]);
        }

        protected override DataGridViewRow FillRow(IBllEntity entity, DataGridViewRow row)
        {
            BllUser data = (BllUser)entity;
            row.Cells[0].Value = data.Fullname;
            row.Cells[1].Value = data.Login;
            row.Cells[2].Value = data.Group.Name;
            return row;
        }

        protected override void AddNewRow(IBllEntity entity)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridView1);
            dataGridView1.Rows.Add(FillRow((BllUser)entity, row));
        }
    }
}
