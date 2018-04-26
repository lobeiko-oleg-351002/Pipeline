namespace ORM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User : IOrmEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            Event = new HashSet<Event>();
            SelectedUser = new HashSet<SelectedUser>();
            SelectedUserReconciler = new HashSet<SelectedUserReconciler>();
        }

        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string login { get; set; }

        [Required]
        [StringLength(50)]
        public string password { get; set; }

        [Required]
        [StringLength(50)]
        public string fullName { get; set; }

        public int group_id { get; set; }

        public int statusLib_id { get; set; }

        public int eventTypeLib_id { get; set; }

        public bool rightToApprove { get; set; }

        public bool rightToReconcile { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? signInDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Event> Event { get; set; }

        public virtual EventTypeLib EventTypeLib { get; set; }

        public virtual Group Group { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SelectedUser> SelectedUser { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SelectedUserReconciler> SelectedUserReconciler { get; set; }

        public virtual StatusLib StatusLib { get; set; }
    }
}
