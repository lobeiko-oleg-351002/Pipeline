namespace ORM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SelectedUser")]
    public partial class SelectedUser : ISelectedEntity
    {
        public int id { get; set; }

        public int lib_id { get; set; }

        public int entity_id { get; set; }

        public bool isEventAccepted { get; set; }

        public virtual User User { get; set; }

        public virtual UserLib UserLib { get; set; }
    }
}
