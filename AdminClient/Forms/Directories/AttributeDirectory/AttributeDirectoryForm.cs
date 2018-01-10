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

namespace AdminClient.Forms.Directories.AttributeDirectory
{
    public partial class AttributeDirectoryForm : DirectoryForm
    {
        List<BllAttribute> Attributes;

        public AttributeDirectoryForm() : base(null)
        {
            InitializeComponent();
        }

        public AttributeDirectoryForm(IBusinessService server) : base(server)
        {
            InitializeComponent();
            RefreshData();
        }

        public override void RefreshData()
        {
            dataGridView1.Rows.Clear();
            Attributes = server.GetAllAttributes();
            foreach (var Attribute in Attributes)
            {
                AddNewRow(Attribute);
            }
        }

        protected override void button1_Click(object sender, EventArgs e)
        {
            AddAttributeForm addAttributeForm = new AddAttributeForm(this, server, null);
            addAttributeForm.ShowDialog(this);
            AddNewRow(addAttributeForm.Entity);
        }

        protected override void button2_Click(object sender, EventArgs e)
        {
            int i = dataGridView1.SelectedRows[0].Index;
            AmendAttributeForm amendAttributeForm = new AmendAttributeForm(this, server, Attributes[i]);
            amendAttributeForm.ShowDialog(this);
            UpdateRow(amendAttributeForm.Entity, i);
        }

        protected override void button3_Click(object sender, EventArgs e)
        {
            int i = dataGridView1.SelectedRows[0].Index;
            server.DeleteAttribute(Attributes[i].Id);
            dataGridView1.Rows.RemoveAt(i);
            Attributes.RemoveAt(i);
            base.button3_Click(sender, e);
        }

        protected override void UpdateRow(IBllEntity entity, int row)
        {
            BllAttribute data = (BllAttribute)entity;
            Attributes[row] = data;
            FillRow(data, dataGridView1.Rows[row]);
        }

        protected override DataGridViewRow FillRow(IBllEntity entity, DataGridViewRow row)
        {
            BllAttribute data = (BllAttribute)entity;
            row.Cells[0].Value = data.Name;
            return row;
        }

        protected override void AddNewRow(IBllEntity entity)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridView1);
            dataGridView1.Rows.Add(FillRow((BllAttribute)entity, row));
        }
    }
}
