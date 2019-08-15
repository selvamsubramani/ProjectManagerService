using System;

namespace ProjectManagement.Entities
{
    public class Task
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Priority { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsParent { get; set; }
        public Project Project { get; set; }
        public User Owner { get; set; }
        public Task Parent { get; set; }

    }
}
