using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimpleTimeManager
{
    public class TaskManager
    {
        public TaskList TaskList;
        public TaskState ViewContext;

        private SimpleTask _taskContext;

        public TaskManager()
        {
            TaskList = new TaskList();
            ViewContext = TaskState.Open;
        }

        public void NewTask()
        {
            Console.Clear();
            Session.LineBreak();
            Console.Write("  Task Name: ");
            var taskName = Console.ReadLine();

            TaskList.Tasks.Add(new SimpleTask(taskName));
            TaskList.Save();
            DisplayTask(TaskList.Tasks.IndexOf(TaskList.Tasks.Last()));
        }

        internal void DisplayTask(int taskIndex)
        {
            if (!TaskExists(taskIndex))
            {
                Session.DisplayError("that task doesn't exist");
                return;
            }

            _taskContext = TaskList.Tasks.ElementAt(taskIndex);

            Console.Clear();
            Session.LineBreak();
            Session.ApplyMutedForeground();
            Console.Write(string.Format("{0}| ", "Name".PadLeft(10)));
            Session.ApplyDefaultForeground();
            Console.WriteLine(_taskContext.Name);

            Session.LineBreak();
            Session.ApplyMutedForeground();
            Console.Write(string.Format("{0}| ", "Status".PadLeft(10)));
            Session.ApplyStatusForeground(_taskContext.Status);
            Console.WriteLine("" + _taskContext.Status.ToString());

            Session.ApplyMutedForeground();
            Console.WriteLine(string.Format("{0}| {1}", "Duration".PadLeft(10), GetTaskDuration(_taskContext.GetDuration())));

            Session.ApplyMutedForeground();
            Console.WriteLine(string.Format("{0}| {1} ({2})", "Created".PadLeft(10), _taskContext.CreatedDate, GetTaskAge(_taskContext.CreatedDate)));
            Session.LineBreak();

            foreach (var log in _taskContext.AuditTracker)
            {
                string timestamp = string.Format("{0} {1}", log.Timestamp.ToShortDateString(), log.Timestamp.ToShortTimeString() );
                string state = log.State.ToString();
                string status = log.Status.ToString();

                Console.WriteLine(string.Format("".PadLeft(10) + "° {0} task was {1} {2}", timestamp, state, status));
            }

            if (_taskContext.AuditTracker.Count != 0)
                Session.LineBreak();

            TaskMenuHandler();
        }

        private string GetTaskDuration(TimeSpan duration)
        {
            var durationString = "";

            if (duration > TimeSpan.FromDays(1))
                durationString += string.Format("{0} Days ", duration.Days);

            if (duration > TimeSpan.FromHours(1))
                durationString += string.Format("{0} Hours ", duration.Hours);

            if (duration > TimeSpan.FromMinutes(1))
                durationString += string.Format("{0} Minutes ", duration.Minutes);

            if (duration > TimeSpan.FromSeconds(1))
                durationString += string.Format("{0} Seconds ", duration.Seconds);

            return string.IsNullOrEmpty(durationString) ? "N/A" : durationString;
        }

        public void DisplayTaskList()
        {
            IEnumerable<SimpleTask> tasks;

            if (ViewContext == TaskState.Open)
                tasks = TaskList.Tasks.Where(x => x.State == TaskState.Open);
            else
                tasks = TaskList.Tasks.Where(x => x.State == TaskState.Closed);

            tasks = tasks.OrderByDescending(x => x.Status);
            Console.Clear();
            Session.LineBreak();

            Session.ApplyMutedForeground();
            Console.WriteLine(string.Format("     ┌  {0} ┬  {1} ┐", ViewContext + " Tasks".PadRight(39 - ViewContext.ToString().Length), "Age".PadRight(7)));
            Console.WriteLine(string.Format("┌────┼{0}┼{1}┼──┐", "".PadRight(42, '─'), "".PadRight(10, '─')));


            foreach (var task in tasks)
            {
                Session.ApplyMutedForeground();
                Console.Write("│" + TaskList.Tasks.IndexOf(task).ToString().PadLeft(3, ' ').PadRight(4));

                Session.ApplyDefaultForeground();
                Console.Write(string.Format("├ {1} ┼ {2} ┤",
                    TaskList.Tasks.IndexOf(task).ToString().PadLeft(3, ' ').PadRight(4),
                    task.Name.Length > 40 ? string.Concat(task.Name.Substring(0, 37), "...") : task.Name.PadRight(40),
                    GetTaskAge(task.CreatedDate).PadRight(8).PadLeft(8)
                    ));

                Session.ApplyStatusForeground(task.Status);
                Console.Write("██");
                Session.ApplyMutedForeground();
                Console.WriteLine("│");
                Session.ApplyDefaultForeground();
            }
            Console.WriteLine(string.Format("└────┴{0}┴{1}┴──┘", "".PadRight(42, '─'), "".PadRight(10, '─')));

            // Session.LineBreak();

            Session.LineBreak();
            Session.DisplayOptions(new[] { "New Task", ViewContext == TaskState.Open ? "View Closed" : "View Opened", "Stats", "Exit" });
            Session.LineBreak();
        }

        private void TaskMenuHandler()
        {
            string stateOption;
            if (_taskContext.State == TaskState.Closed)
                stateOption = "Reopen";
            else
                stateOption = _taskContext.Status == TaskStatus.Active ? "Stop" : "Start";
            Session.LineBreak();
            Session.DisplayOptions(new[] { "Back", stateOption, "Complete", "Remove" });
            Session.LineBreak();

            bool selectionRequired = true;
            while (selectionRequired)
            {
                Console.Write("  ");

                var option = Session.GetInput().ToString().ToUpper();
                selectionRequired = false;

                Session.ApplyMutedForeground();
                Console.Write(option);
                Session.ApplyDefaultForeground();

                switch (option)
                {
                    case "A":
                        _taskContext = null;
                        DisplayTaskList();
                        break;

                    case "S":
                        if (_taskContext.State == TaskState.Closed)
                            _taskContext.Reopen();
                        else if (_taskContext.Status == TaskStatus.Active)
                            _taskContext.Wait();
                        else
                            _taskContext.Active();
                        break;
                    case "D":
                        _taskContext.Complete();
                        break;
                    case "F":
                        _taskContext.Cancel();
                        break;
                    default:
                        Session.DisplayError("that wasn't an option");
                        selectionRequired = true;
                        break;
                }
            }
            TaskList.Save();
            DisplayTaskList();
        }

        private static string GetTaskAge(DateTime createdDate)
        {
            var age = DateTime.Now - createdDate;

            if (age < TimeSpan.FromMinutes(1))
                return "New";

            if (age < TimeSpan.FromHours(1))
                return string.Format("{0} Mins", age.Minutes.ToString().PadRight(2));

            if (age < TimeSpan.FromDays(1))
                return string.Format("{0} Hours", age.Hours.ToString().PadRight(2));

            if (age < TimeSpan.FromDays(30))
                return string.Format("{0} Days", age.Days.ToString().PadRight(2));

            return ("Old");

        }

        internal bool TaskExists(int intIndex)
        {
            return TaskList.Tasks.Count() >= intIndex + 1;
        }


    }
}