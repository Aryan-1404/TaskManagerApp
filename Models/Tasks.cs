using System;

namespace Models
{
    public class Tasks
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public Guid AssignedUserId { get; set; }
        public DateTime DueDate { get; set; } = DateTime.Now;

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public TaskStatus Status { get; set; } = TaskStatus.Todo;
    }
}
