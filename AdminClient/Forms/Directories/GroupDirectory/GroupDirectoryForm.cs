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

namespace AdminClient.Forms.Directories.GroupDirectory
{
    public partial class GroupDirectoryForm : DirectoryForm
    {
        List<BllGroup> Groups;

        public GroupDirectoryForm() : base(null)
        {
            InitializeComponent();
        }

        public GroupDirectoryForm(IBusinessService server) : base(server)
        {
            InitializeComponent();
            RefreshData();
        }

        public override void RefreshData()
        {
            dataGridView1.Rows.Clear();
            Groups = server.GetAllGroups();
            foreach (var Group in Groups)
            {
                AddNewRow(Group);
            }
        }

        protected override void button1_Click(object sender, EventArgs e)
        {
            AddGroupForm addGroupForm = new AddGroupForm(this, server, null);
            addGroupForm.ShowDialog(this);
            if (addGroupForm.Entity != null)
            {
                AddNewRow(addGroupForm.Entity);
                Groups.Add((BllGroup)addGroupForm.Entity);
            }
        }

        protected override void button2_Click(object sender, EventArgs e)
        {
            int i = dataGridView1.SelectedRows[0].Index;
            AmendGroupForm amendGroupForm = new AmendGroupForm(this, server, Groups[i]);
            amendGroupForm.ShowDialog(this);
            UpdateRow(amendGroupForm.Entity, i);
        }

        protected override void button3_Click(object sender, EventArgs e)
        {
            int i = dataGridView1.SelectedRows[0].Index;
            server.DeleteGroup(Groups[i].Id);
            dataGridView1.Rows.RemoveAt(i);
            Groups.RemoveAt(i);
            base.button3_Click(sender, e);
        }

        protected override void UpdateRow(IBllEntity entity, int row)
        {
            BllGroup data = (BllGroup)entity;
            Groups[row] = data;
            FillRow(data, dataGridView1.Rows[row]);
        }

        protected override DataGridViewRow FillRow(IBllEntity entity, DataGridViewRow row)
        {
            BllGroup data = (BllGroup)entity;
            row.Cells[0].Value = data.Name;
            return row;
        }

        protected override void AddNewRow(IBllEntity entity)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridView1);
            dataGridView1.Rows.Add(FillRow((BllGroup)entity, row));
        }
    }
}
