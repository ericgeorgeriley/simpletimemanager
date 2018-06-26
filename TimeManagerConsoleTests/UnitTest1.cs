using SimpleTimeManager;
using System;
using Xunit;

namespace TimeManagerConsoleTests
{
    public class SimpleTaskShould
    {
        [Fact]
        public void GetCreatedWithDefaults()
        {
            SimpleTask task = new SimpleTask("task name");

            Assert.True(task.Name == "task name");
        }
    }
}
