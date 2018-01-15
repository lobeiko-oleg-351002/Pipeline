namespace ORM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Filepath")]
    public partial class Filepath : IOrmEntity
    {
        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string path { get; set; }

        public int lib_id { get; set; }

        public virtual FilepathLib FilepathLib { get; set; }
    }
}
