namespace ProjectManagement.DataLayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Task")]
    public partial class Task
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Task()
        {
            ChildrenTasks = new HashSet<Task>();
        }

        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsParent { get; set; }

        public int? ParentID { get; set; }

        public int Priority { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsCompleted { get; set; }

        public int ProjectID { get; set; }

        public int? UserID { get; set; }

        public virtual Project Project { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Task> ChildrenTasks { get; set; }

        public virtual Task ParentTask { get; set; }

        public virtual User TaskOwner { get; set; }
    }
}
