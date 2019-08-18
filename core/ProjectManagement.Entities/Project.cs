using System;

namespace ProjectManagement.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Priority { get; set; }
        public int NoOfTasks { get; set; }
        public User Manager { get; set; }
        public bool IsSuspended { get; set; }
        public bool IsDateEnabled { get; set; }
        public string Status { get; set; }
    }
}
