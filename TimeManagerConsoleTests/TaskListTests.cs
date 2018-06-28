using SimpleTimeManager.Tasks;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace TimeManagerConsole.Tests
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
            _sampletasklist = new TaskList(@"testdata.json");
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
        [InlineData(0)]
        [InlineData(3)]
        [InlineData(7)]
        [InlineData(11)]
        public void ReturnRequestedTask(int taskIndex)
        {
            var requestedTask = _sampletasklist.GetTask(taskIndex);
            var taskNameNumber = taskIndex + 1;

            Assert.Equal("Sample Task " + taskNameNumber, requestedTask.Name);
        }

        [Theory]
        [InlineData(TaskState.Open)]
        [InlineData(TaskState.Closed)]
        public void ReturnRequestedTasks(TaskState state)
        {
            var requestedTasks = _sampletasklist.GetTasks(state);

            int foo = _sampletasklist.Tasks.Where(x => x.State == state).Count();

            Assert.Equal(foo, requestedTasks.Count());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(3)]
        [InlineData(7)]
        [InlineData(11)]
        public void ReturnRequestedTaskIndex(int taskIndex)
        {
            var requestedTask = _sampletasklist.GetTask(taskIndex);
            var requestedTaskIndex = _sampletasklist.GetTaskIndex(requestedTask);

            Assert.Equal(taskIndex, requestedTaskIndex);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(3)]
        [InlineData(7)]
        [InlineData(11)]
        public void ConfirmATaskExists(int taskIndex)
        {
            Assert.True(_sampletasklist.TaskExists(taskIndex));
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(12)]
        public void ConfirmATaskDoesntExist(int taskIndex)
        {
            Assert.False(_sampletasklist.TaskExists(taskIndex));
        }
    }
}
