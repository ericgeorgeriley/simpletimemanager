using SimpleTimeManager;
using System;
using System.Linq;
using Xunit;

namespace TimeManagerConsoleTests
{
    public class SimpleTaskShould
    {
        [Fact]
        public void GetCreatedWithDefaults()
        {
            SimpleTask task = new SimpleTask("task name");

            Assert.Equal("task name", task.Name);
            Assert.Equal(new TimeSpan(), task.Duration);
            Assert.Equal(TaskState.Open, task.State);
            Assert.Equal(TaskStatus.NotStarted, task.Status);
            Assert.Empty(task.AuditTracker);
        }

        [Fact]
        public void TrackDurationWhenActive()
        {
            SimpleTask task = new SimpleTask("");

            task.Active();

            Assert.NotNull(task.ActiveStarted);

            Assert.Equal(new TimeSpan(), task.Duration);
            Assert.Equal(TaskState.Open, task.State);
            Assert.Equal(TaskStatus.NotStarted, task.Status);



        }
    }
}
