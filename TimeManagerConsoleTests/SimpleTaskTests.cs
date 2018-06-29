using SimpleTimeManager.Tasks;
using SimpleTimeManager.Tests.TestData;
using System;
using Xunit;
using Xunit.Abstractions;

namespace SimpleTimeManager.Tests
{
    [Trait("Category", "TaskWorkflow")]
    public class SimpleTaskShould
    {
        private readonly SimpleTask _task;
        private readonly ITestOutputHelper _output;

        public SimpleTaskShould(ITestOutputHelper output)
        {
            _output = output;
            _task = new SimpleTask("Sample Task");
            _output.WriteLine(string.Format("Created a simple task called : \"{0}\"", _task.Name));

            _output.WriteLine("Leaving constructor - starting test");
        }

        [Fact]
        public void GetCreatedWithDefaults()
        {
            Assert.Equal(TaskState.Open, _task.State);
            Assert.Equal(TaskStatus.NotStarted, _task.Status);
            Assert.Equal("Sample Task", _task.Name);
            Assert.Equal(new TimeSpan(), _task.Duration);
            Assert.Empty(_task.AuditTracker);
        }

        [Fact]
        public void TrackDurationWhenActive()
        {
            _task.Active();

            Assert.Equal(TaskState.Open, _task.State);
            Assert.Equal(TaskStatus.Active, _task.Status);
            Assert.NotNull(_task.ActiveStarted);
            Assert.Equal(new TimeSpan(), _task.Duration);
        }

        [Fact]
        public void ShowDynamicDurationWhenActive()
        {
            _task.Active();
            var startingDuration = _task.GetDuration();

            Assert.Equal(TaskState.Open, _task.State);
            Assert.Equal(TaskStatus.Active, _task.Status);
            Assert.True(_task.GetDuration() > startingDuration);
        }

        [Fact]
        public void ShowStaticDurationWhenInactive()
        {
            _task.Active();
            _task.Wait();
            var startingDuration = _task.GetDuration();

            Assert.Equal(TaskState.Open, _task.State);
            Assert.Equal(TaskStatus.Waiting, _task.Status);
            Assert.Equal(startingDuration, _task.GetDuration());
        }

        [Fact]
        public void UpdateDurationWhenWaiting()
        {
            var startingDuration = _task.GetDuration();
            _task.Active();
            _task.Wait();

            Assert.Equal(TaskState.Open, _task.State);
            Assert.Equal(TaskStatus.Waiting, _task.Status);
            Assert.Null(_task.ActiveStarted);
            Assert.True(_task.Duration > startingDuration);
        }

        [Theory]
        [TaskStatusTestData]
        public void UpdateAuditTrackerWhenChangingStatus(TaskStatus status)
        {
            int expectedTrackerCount = 1;

            switch (status)
            {
                case TaskStatus.NotStarted:
                    expectedTrackerCount = 0;
                    break;

                case TaskStatus.Waiting:
                    _task.Active();
                    _task.Wait();
                    expectedTrackerCount = 2;
                    break;

                case TaskStatus.Active:
                    _task.Active();
                    break;

                case TaskStatus.Complete:
                    _task.Complete();
                    break;

                case TaskStatus.Cancelled:
                    _task.Cancel();
                    break;

                default:
                    break;
            }

            Assert.Equal(expectedTrackerCount, _task.AuditTracker.Count);
        }

        [Fact]
        public void CloseWhenCompleted()
        {
            _task.Complete();

            Assert.Equal(TaskState.Closed, _task.State);
            Assert.Equal(TaskStatus.Complete, _task.Status);
        }

        [Fact]
        public void CloseWhenCancelled()
        {
            _task.Cancel();

            Assert.Equal(TaskState.Closed, _task.State);
            Assert.Equal(TaskStatus.Cancelled, _task.Status);
        }

        [Theory]
        [TaskStatusTestData]
        public void MaintainCurrentStatus(TaskStatus status)
        {
            switch (status)
            {
                case TaskStatus.NotStarted:
                    break;
                case TaskStatus.Waiting:
                    _task.Active();
                    _task.Wait();
                    break;
                case TaskStatus.Active:
                    _task.Active();
                    break;
                case TaskStatus.Complete:
                    _task.Complete();
                    break;
                case TaskStatus.Cancelled:
                    _task.Cancel();
                    break;
                default:
                    break;
            }

            Assert.Equal(status, _task.Status);
        }

        [Fact]
        public void BeOpenAndWaitingWhenReopened()
        {
            _task.Cancel();
            _task.Reopen();

            Assert.Equal(TaskState.Open, _task.State);
            Assert.Equal(TaskStatus.Waiting, _task.Status);
        }
    }
}
