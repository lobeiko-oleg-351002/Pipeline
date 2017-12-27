namespace ORM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SelectedStatus : ISelectedEntity
    {
        public int id { get; set; }

        public int lib_id { get; set; }

        public int entity_id { get; set; }

        public DateTime date { get; set; }

        public virtual Status Status { get; set; }

        public virtual StatusLib StatusLib { get; set; }
    }
}
