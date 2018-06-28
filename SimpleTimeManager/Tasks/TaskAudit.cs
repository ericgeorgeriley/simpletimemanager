using System;

namespace SimpleTimeManager.Tasks
{
    public class TaskAudit
    {
        public DateTime Timestamp { get; set; }
        public TaskState State { get; set; }
        public TaskStatus Status { get; set; }
    }

}
