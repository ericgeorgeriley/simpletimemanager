using System;

namespace SimpleTimeManager
{
    public class TimeManagerConsole
    {
        internal static TaskManager taskManager;
        internal static bool exitState;

        internal static bool Run()
        {
            taskManager = new TaskManager();

            while (!exitState)
            {
                taskManager.DisplayTaskList();

                var selected = Session.GetInput();

                if (Char.IsNumber(selected))
                    OpenTaskHandler(selected);
                else
                    MenuHandler(selected);
            }
            return false;
        }

        private static void MenuHandler(char selected)
        {
            var option = selected.ToString().ToUpper();
            Session.ApplyMutedForeground();
            Console.Write(option);
            Session.ApplyDefaultForeground();

            switch (option)
            {
                case "A":
                    taskManager.NewTask();
                    break;
                case "S":
                    taskManager.ViewContext = taskManager.ViewContext == TaskState.Open ? TaskState.Closed : TaskState.Open;
                    break;
                case "F":
                    exitState = true;
                    break;
                default:
                    Session.DisplayError("that wasn't an option");
                    break;
            }
        }

        private static void OpenTaskHandler(char selected)
        {
            var taskIndex = selected.ToString();
            var remainingRequiredDigits = Math.Floor(Math.Log10(taskManager.TaskList.Tasks.Count));

            Session.ApplyInvertForeground();
            Console.Write("".PadLeft(2));
            Session.ApplyHighlightBackground();
            Console.Write("Task: " + taskIndex);

            for (int i = 0; i < remainingRequiredDigits; i++)
                taskIndex += GetNextInput(taskIndex);
            Session.ApplyDefaultForeground();
            Session.ApplyDefaultBackground();


            int intIndex;
            try
            {
                intIndex = Convert.ToInt32(taskIndex);
            }
            catch (Exception)
            {
                Session.DisplayError("that's not a number");
                return;
            }

            taskManager.DisplayTask(intIndex);
        }

        private static string GetNextInput(string taskIndex)
        {
            var nextChar = Session.GetInput();
            Console.Write(nextChar);

            taskIndex += nextChar;
            return nextChar.ToString();
        }
    }

}