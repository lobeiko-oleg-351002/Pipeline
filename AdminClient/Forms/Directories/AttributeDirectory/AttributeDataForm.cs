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
    public partial class AttributeDataForm : EntityDataForm
    {
        public AttributeDataForm()
        {
            InitializeComponent();
        }


        public AttributeDataForm(DirectoryForm parent, IBusinessService server, IBllEntity entity) : base(parent, server, entity)
        {
            InitializeComponent();
        }
    }
}
