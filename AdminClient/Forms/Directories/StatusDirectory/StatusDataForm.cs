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
    public partial class StatusDataForm : EntityDataForm
    {
        public StatusDataForm()
        {
            InitializeComponent();
        }


        public StatusDataForm(DirectoryForm parent, IBusinessService server, IBllEntity entity) : base(parent, server, entity)
        {
            InitializeComponent();
        }
    }
}
