using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ORM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SelectedUserReconciler")]
    public partial class SelectedUserReconciler : ISelectedEntity
    {
        public int id { get; set; }

        public int lib_id { get; set; }

        public int entity_id { get; set; }

        public bool? isEventReconciled { get; set; }

        public virtual User User { get; set; }

        public virtual ReconcilerLib ReconcilerLib { get; set; }
    }
}
