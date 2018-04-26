using BllEntities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BllEntities
{
    public class BllReconcilerLib : IBllEntity
    {
        public int Id { get; set; }

        public List<BllSelectedUserReconciler> SelectedEntities { get; set; }

        public BllReconcilerLib()
        {
            SelectedEntities = new List<BllSelectedUserReconciler>();
        }
    }
}
