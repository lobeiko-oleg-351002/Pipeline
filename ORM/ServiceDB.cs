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

        public virtual DbSet<Attribute> Attributes { get; set; }
        public virtual DbSet<AttributeLib> AttributeLibs { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<EventType> EventTypes { get; set; }
        public virtual DbSet<EventTypeLib> EventTypeLibs { get; set; }
        public virtual DbSet<Filepath> Filepaths { get; set; }
        public virtual DbSet<FilepathLib> FilepathLibs { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<SelectedAttribute> SelectedAttributes { get; set; }
        public virtual DbSet<SelectedEventType> SelectedEventTypes { get; set; }
        public virtual DbSet<SelectedStatus> SelectedStatuses { get; set; }
        public virtual DbSet<SelectedUser> SelectedUsers { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<StatusLib> StatusLibs { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserLib> UserLibs { get; set; }
        public virtual DbSet<ReconcilerLib> ReconcilerLibs { get; set; }
        public virtual DbSet<SelectedUserReconciler> SelectedUserReconcilers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attribute>()
                .HasMany(e => e.SelectedAttribute)
                .WithRequired(e => e.Attribute)
                .HasForeignKey(e => e.entity_id);

            modelBuilder.Entity<AttributeLib>()
                .HasMany(e => e.Event)
                .WithRequired(e => e.AttributeLib)
                .HasForeignKey(e => e.attribute_lib_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AttributeLib>()
                .HasMany(e => e.SelectedAttribute)
                .WithRequired(e => e.AttributeLib)
                .HasForeignKey(e => e.lib_id);

            modelBuilder.Entity<EventType>()
                .HasMany(e => e.Event)
                .WithRequired(e => e.EventType)
                .HasForeignKey(e => e.type_id);

            modelBuilder.Entity<EventType>()
                .HasMany(e => e.SelectedEventType)
                .WithRequired(e => e.EventType)
                .HasForeignKey(e => e.entity_id);

            modelBuilder.Entity<EventTypeLib>()
                .HasMany(e => e.SelectedEventType)
                .WithRequired(e => e.EventTypeLib)
                .HasForeignKey(e => e.lib_id);

            modelBuilder.Entity<EventTypeLib>()
                .HasMany(e => e.User)
                .WithRequired(e => e.EventTypeLib)
                .HasForeignKey(e => e.eventTypeLib_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FilepathLib>()
                .HasMany(e => e.Event)
                .WithRequired(e => e.FilepathLib)
                .HasForeignKey(e => e.filepath_lib_id);

            modelBuilder.Entity<FilepathLib>()
                .HasMany(e => e.Filepath)
                .WithRequired(e => e.FilepathLib)
                .HasForeignKey(e => e.lib_id);

            modelBuilder.Entity<Group>()
                .HasMany(e => e.User)
                .WithRequired(e => e.Group)
                .HasForeignKey(e => e.group_id);

            modelBuilder.Entity<Status>()
                .HasMany(e => e.SelectedStatus)
                .WithRequired(e => e.Status)
                .HasForeignKey(e => e.entity_id);

            modelBuilder.Entity<StatusLib>()
                .HasMany(e => e.Event)
                .WithRequired(e => e.StatusLib)
                .HasForeignKey(e => e.status_lib_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<StatusLib>()
                .HasMany(e => e.SelectedStatus)
                .WithRequired(e => e.StatusLib)
                .HasForeignKey(e => e.lib_id);

            modelBuilder.Entity<StatusLib>()
                .HasMany(e => e.User)
                .WithRequired(e => e.StatusLib)
                .HasForeignKey(e => e.statusLib_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Event)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.sender_id);

            modelBuilder.Entity<User>()
                .HasMany(e => e.SelectedUser)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.entity_id);

            modelBuilder.Entity<User>()
                .HasMany(e => e.SelectedUserReconciler)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.entity_id);

            modelBuilder.Entity<UserLib>()
                .HasMany(e => e.Event)
                .WithRequired(e => e.UserLib)
                .HasForeignKey(e => e.receiver_lib_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserLib>()
                .HasMany(e => e.SelectedUser)
                .WithRequired(e => e.UserLib)
                .HasForeignKey(e => e.lib_id);

            modelBuilder.Entity<ReconcilerLib>()
                .HasMany(e => e.Event)
                .WithRequired(e => e.Reconcilers)
                .HasForeignKey(e => e.reconciler_lib_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ReconcilerLib>()
                .HasMany(e => e.SelectedUserReconciler)
                .WithRequired(e => e.ReconcilerLib)
                .HasForeignKey(e => e.lib_id);
        }
    }
}
