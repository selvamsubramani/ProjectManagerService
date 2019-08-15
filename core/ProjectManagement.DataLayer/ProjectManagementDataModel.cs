namespace ProjectManagement.DataLayer
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ProjectManagementDataModel : DbContext
    {
        public ProjectManagementDataModel()
            : base("name=ProjectManagementDataModel")
        {
        }

        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .HasMany(e => e.Tasks)
                .WithRequired(e => e.Project)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Task>()
                .HasMany(e => e.ChildrenTasks)
                .WithOptional(e => e.ParentTask)
                .HasForeignKey(e => e.ParentID);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Projects)
                .WithRequired(e => e.Manager)
                .HasForeignKey(e => e.ManagerID)
                .WillCascadeOnDelete(false);
        }
    }
}
