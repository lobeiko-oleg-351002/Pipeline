namespace ORM
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ServiceDB : DbContext
    {
        public ServiceDB()
            : base("name=ServiceDB")
        {
        }

        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>()
                .HasMany(e => e.User)
                .WithOptional(e => e.Group)
                .HasForeignKey(e => e.group_id);
        }
    }
}
