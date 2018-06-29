using SimpleTimeManager.Tasks;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using SimpleTimeManager.Tests.TestData;

namespace SimpleTimeManager.Tests
{
    [Trait("Category", "TaskWorkflow")]
    public class TaskListShould
    {
        private readonly TaskList _sampletasklist;
        private readonly TaskList _emptytasklist;

        private readonly ITestOutputHelper _output;

        public TaskListShould(ITestOutputHelper output)
        {
            _output = output;
            _sampletasklist = new TaskList(@"TestData\testdata.json");
            _output.WriteLine(string.Format("Created an sample task list"));
            _emptytasklist = new TaskList();
            _output.WriteLine(string.Format("Created an empty task list"));

            _output.WriteLine("Leaving constructor - starting test");
        }

        [Fact]
        public void HaveNoTasksWhenNew()
        {
            Assert.Empty(_emptytasklist.Tasks);
        }

        [Fact]
        public void HaveTasksWhenLoaded()
        {
            Assert.NotEmpty(_sampletasklist.Tasks);
            Assert.Equal(12, _sampletasklist.Tasks.Count());
        }

        [Fact]
        public void PersistTasksWhenSaving()
        {
            var saveTaskList = new TaskList(@"TestData\savedatatest.json");
            saveTaskList.AddTask("Persisted Task");
            saveTaskList.Save();

            var tasksCount = saveTaskList.Tasks.Count();
            var savedTask = saveTaskList.GetTask(saveTaskList.GetLatestTaskIndex());

            var loadedTaskList = new TaskList(@"TestData\savedatatest.json");
            var persistedTask = loadedTaskList.GetTask(saveTaskList.GetLatestTaskIndex());

            Assert.NotEmpty(loadedTaskList.Tasks);
            Assert.Equal(tasksCount, loadedTaskList.Tasks.Count());
            Assert.Equal(savedTask.Name, persistedTask.Name);
            Assert.Equal(savedTask.State, persistedTask.State);
            Assert.Equal(savedTask.Status, persistedTask.Status);
            Assert.Equal(savedTask.CreatedDate, persistedTask.CreatedDate);
            Assert.Equal(savedTask.Duration, persistedTask.Duration);
            Assert.Equal(savedTask.ActiveStarted, persistedTask.ActiveStarted);
            Assert.Equal(savedTask.AuditTracker, persistedTask.AuditTracker);
        }

        [Fact]
        public void AddTaskToTheList()
        {
            var initialTaskCount = _sampletasklist.Tasks.Count();

            _sampletasklist.AddTask("New Task");

            Assert.Equal(initialTaskCount + 1, _sampletasklist.Tasks.Count());
        }

        [Fact]
        public void ReturnLatestTaskIndex()
        {
            Assert.Equal(11, _sampletasklist.GetLatestTaskIndex());
        }

        [Theory]
        [TaskIndexTestData]
        public void ReturnRequestedTask(int taskIndex)
        {
            var requestedTask = _sampletasklist.GetTask(taskIndex);
            var taskNameNumber = taskIndex + 1;

            Assert.Equal("Sample Task " + taskNameNumber, requestedTask.Name);
        }

        [Theory]
        [InlineData(TaskState.Open)]
        [InlineData(TaskState.Closed)]
        public void ReturnTasksForRequestedState(TaskState state)
        {
            var requestedTasks = _sampletasklist.GetTasks(state);

            int expectedTaskCount = _sampletasklist.Tasks.Where(x => x.State == state).Count();

            Assert.Equal(expectedTaskCount, requestedTasks.Count());
        }

        [Theory]
        [TaskIndexTestData]
        public void ReturnRequestedTaskIndex(int taskIndex)
        {
            var requestedTask = _sampletasklist.GetTask(taskIndex);
            var requestedTaskIndex = _sampletasklist.GetTaskIndex(requestedTask);

            Assert.Equal(taskIndex, requestedTaskIndex);
        }

        [Theory]
        [TaskIndexTestData]
        public void ConfirmATaskExists(int taskIndex)
        {
            Assert.True(_sampletasklist.TaskExists(taskIndex));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(12)]
        public void ConfirmATaskDoesntExist(int taskIndex)
        {
            Assert.False(_sampletasklist.TaskExists(taskIndex));
        }
    }
}
