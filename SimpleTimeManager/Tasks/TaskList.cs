using System.Collections.Generic;
using System.Linq;

namespace SimpleTimeManager.Tasks
{
    public enum TaskState
    {
        Open, Closed
    }

    public enum TaskStatus
    {
        NotStarted = 0,
        Waiting = 1,
        Active = 2,
        Complete = 3,
        Cancelled = 4
    }

    public class TaskList
    {
        private string FileName { get; set; }

        public List<SimpleTask> Tasks { get; set; }
        public TaskList(string fromFile = null)
        {
            FileName = string.IsNullOrEmpty(fromFile) ? @"savedata.json" : fromFile;        
            Tasks = Load() ?? new List<SimpleTask>();
        }

        private List<SimpleTask> Load()
        {

            return Session.Load(FileName);
        }

        internal void Save()
        {
            Session.Save(Tasks, FileName);
        }

        public bool AddTask(string taskName)
        {
            Tasks.Add(new SimpleTask(taskName));
            return true;
        }

        public int GetLatestTaskIndex()
        {
            return Tasks.IndexOf(Tasks.Last());
        }

        public SimpleTask GetTask(int taskIndex)
        {
            try
            {
                return Tasks.ElementAt(taskIndex);
            }
            catch { return null; }
        }

        public IEnumerable<SimpleTask> GetTasks(TaskState state)
        {
            return Tasks.Where(x => x.State == state);
        }

        public int GetTaskIndex(SimpleTask task)
        {
            return Tasks.IndexOf(task);
        }

        public bool TaskExists(int intIndex)
        {
            if (intIndex < 0)
                return false;
            return Tasks.Count() >= intIndex + 1;
        }
    }
}
