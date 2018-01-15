namespace ORM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Event")]
    public partial class Event : IOrmEntity
    {
        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }

        [StringLength(50)]
        public string description { get; set; }

        public int status_lib_id { get; set; }

        public int filepath_lib_id { get; set; }

        public int attribute_lib_id { get; set; }

        public int receiver_lib_id { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime date { get; set; }

        public int sender_id { get; set; }

        public int type_id { get; set; }

        public virtual AttributeLib AttributeLib { get; set; }

        public virtual EventType EventType { get; set; }

        public virtual FilepathLib FilepathLib { get; set; }

        public virtual StatusLib StatusLib { get; set; }

        public virtual User User { get; set; }

        public virtual UserLib UserLib { get; set; }
    }
}
