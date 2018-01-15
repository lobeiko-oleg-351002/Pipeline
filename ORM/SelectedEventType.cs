namespace ORM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SelectedEventType")]
    public partial class SelectedEventType : ISelectedEntity
    {
        public int id { get; set; }

        public int entity_id { get; set; }

        public int lib_id { get; set; }

        public virtual EventType EventType { get; set; }

        public virtual EventTypeLib EventTypeLib { get; set; }
    }
}
