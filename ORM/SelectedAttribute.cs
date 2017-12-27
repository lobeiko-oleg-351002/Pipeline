namespace ORM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SelectedAttribute")]
    public partial class SelectedAttribute : ISelectedEntity
    {
        public int id { get; set; }

        public int lib_id { get; set; }

        public int entity_id { get; set; }

        public virtual Attribute Attribute { get; set; }

        public virtual AttributeLib AttributeLib { get; set; }
    }
}
