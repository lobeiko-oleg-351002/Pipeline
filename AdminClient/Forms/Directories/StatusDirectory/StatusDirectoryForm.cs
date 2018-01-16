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

namespace AdminClient.Forms.Directories.StatusDirectory
{
    public partial class StatusDirectoryForm : DirectoryForm
    {
        List<BllStatus> Statuses;

        public StatusDirectoryForm() : base(null)
        {
            InitializeComponent();
        }

        public StatusDirectoryForm(IBusinessService server) : base(server)
        {
            InitializeComponent();
            RefreshData();
        }

        public override void RefreshData()
        {
            dataGridView1.Rows.Clear();
            Statuses = server.GetAllStatuses();
            foreach (var status in Statuses)
            {
                AddNewRow(status);
            }
        }

        protected override void button1_Click(object sender, EventArgs e)
        {
            AddStatusForm addStatusForm = new AddStatusForm(this, server, null);
            addStatusForm.ShowDialog(this);
            AddNewRow(addStatusForm.Entity);
        }

        protected override void button2_Click(object sender, EventArgs e)
        {
            int i = dataGridView1.SelectedRows[0].Index;
            if ((Statuses[i].Name != Globals.Globals.STATUS_CLOSED) && (Statuses[i].Name != Globals.Globals.STATUS_DELETED))
            {
                AmendStatusForm amendStatusForm = new AmendStatusForm(this, server, Statuses[i]);
                amendStatusForm.ShowDialog(this);
                UpdateRow(amendStatusForm.Entity, i);
            }
            else
            {
                MessageBox.Show("Эти статусы нельзя изменить");
            }
        }

        protected override void button3_Click(object sender, EventArgs e)
        {
            int i = dataGridView1.SelectedRows[0].Index;
            if ((Statuses[i].Name != Globals.Globals.STATUS_CLOSED) && (Statuses[i].Name != Globals.Globals.STATUS_DELETED))
            {
                server.DeleteStatus(Statuses[i].Id);
                dataGridView1.Rows.RemoveAt(i);
                Statuses.RemoveAt(i);
            }
            else
            {
                MessageBox.Show("Эти статусы нельзя удалить");
            }

            base.button3_Click(sender, e);
        }

        protected override void UpdateRow(IBllEntity entity, int row)
        {
            BllStatus data = (BllStatus)entity;
            Statuses[row] = data;
            FillRow(data, dataGridView1.Rows[row]);
        }

        protected override DataGridViewRow FillRow(IBllEntity entity, DataGridViewRow row)
        {
            BllStatus data = (BllStatus)entity;
            row.Cells[0].Value = data.Name;
            return row;
        }

        protected override void AddNewRow(IBllEntity entity)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridView1);
            dataGridView1.Rows.Add(FillRow((BllStatus)entity, row));
        }
    }
}
