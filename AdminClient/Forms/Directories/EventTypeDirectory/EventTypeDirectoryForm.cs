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

namespace AdminClient.Forms.Directories.EventTypeDirectory
{
    public partial class EventTypeDirectoryForm : DirectoryForm
    {
        List<BllEventType> EventTypes;

        public EventTypeDirectoryForm() : base(null)
        {
            InitializeComponent();
        }

        public EventTypeDirectoryForm(IBusinessService server) : base(server)
        {
            InitializeComponent();
            RefreshData();
        }

        public override void RefreshData()
        {
            dataGridView1.Rows.Clear();
            EventTypes = server.GetAllEventTypes();
            foreach (var EventType in EventTypes)
            {
                AddNewRow(EventType);
            }
        }

        protected override void button1_Click(object sender, EventArgs e)
        {
            AddEventTypeForm addEventTypeForm = new AddEventTypeForm(this, server, null);
            addEventTypeForm.ShowDialog(this);
            AddNewRow(addEventTypeForm.Entity);
        }

        protected override void button2_Click(object sender, EventArgs e)
        {
            int i = dataGridView1.SelectedRows[0].Index;
            AmendEventTypeForm amendEventTypeForm = new AmendEventTypeForm(this, server, EventTypes[i]);
            amendEventTypeForm.ShowDialog(this);
            UpdateRow(amendEventTypeForm.Entity, i);
        }

        protected override void button3_Click(object sender, EventArgs e)
        {
            int i = dataGridView1.SelectedRows[0].Index;
            server.DeleteEventType(EventTypes[i].Id);
            dataGridView1.Rows.RemoveAt(i);
            EventTypes.RemoveAt(i);
            base.button3_Click(sender, e);
        }

        protected override void UpdateRow(IBllEntity entity, int row)
        {
            BllEventType data = (BllEventType)entity;
            EventTypes[row] = data;
            FillRow(data, dataGridView1.Rows[row]);
        }

        protected override DataGridViewRow FillRow(IBllEntity entity, DataGridViewRow row)
        {
            BllEventType data = (BllEventType)entity;
            row.Cells[0].Value = data.Name;
            return row;
        }

        protected override void AddNewRow(IBllEntity entity)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridView1);
            dataGridView1.Rows.Add(FillRow((BllEventType)entity, row));
        }
    }
}
