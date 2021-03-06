﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleTimeManager.Tasks
{
    public class SimpleTask
    {
        public TaskState State { get; set; }
        public TaskStatus Status { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public TimeSpan Duration { get; set; }

        public List<TaskAudit> AuditTracker { get; set; }

        public DateTime? ActiveStarted { get; set; }

        public SimpleTask(string taskName)
        {
            State = TaskState.Open;
            Status = TaskStatus.NotStarted;
            Name = taskName;
            CreatedDate = DateTime.Now;
            Duration = new TimeSpan(0, 0, 0);
            AuditTracker = new List<TaskAudit>();
        }

        public void Active()
        {
            if (Status == TaskStatus.Active)
                return;

            ActiveStarted = DateTime.Now;
            Status = TaskStatus.Active;
            UpdateAuditTracking();

        }

        public void Wait()
        {
            if (Status != TaskStatus.Active)
                return;

            Duration += (DateTime.Now - (DateTime)ActiveStarted);
            ActiveStarted = null;
            Status = TaskStatus.Waiting;
            UpdateAuditTracking();
        }

        public void Complete()
        {
            Status = TaskStatus.Complete;
            State = TaskState.Closed;
            UpdateAuditTracking();
        }

        public void Cancel()
        {
            Status = TaskStatus.Cancelled;
            State = TaskState.Closed;
            UpdateAuditTracking();
        }

        public void Reopen()
        {
            State = TaskState.Open;
            Status = TaskStatus.Waiting;
            UpdateAuditTracking();
        }

        public TimeSpan GetDuration()
        {
            if (ActiveStarted != null)
                return (Duration + (DateTime.Now - (DateTime)ActiveStarted));

            return Duration;
        }

        private void UpdateAuditTracking()
        {
            AuditTracker.Add(new TaskAudit
            {
                Timestamp = DateTime.Now,
                Status = Status,
                State = State
            });
        }

    }

}
