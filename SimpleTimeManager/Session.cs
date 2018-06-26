﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace SimpleTimeManager
{
    internal class Session
    {

        internal static void Save(List<SimpleTask> tasks)
        {
            var saveData = JsonConvert.SerializeObject(tasks);
            TextWriter writer;
            using (writer = new StreamWriter(@"savedata.json", append: false))
            {
                writer.WriteLine(saveData);
            }
        }

        internal static List<SimpleTask> Load()
        {
            try
            {
                string saveData = File.ReadAllText(@"savedata.json");
                return JsonConvert.DeserializeObject<List<SimpleTask>>(saveData);
            }
            catch (Exception)
            {
                Console.WriteLine("info: unable to find save file");
                return null;
            }
        }

        internal static char GetInput()
        {
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    return Convert.ToChar(key);
                }
            }
        }


        internal static void DisplayError(string v)
        {
            ApplyErrorForeground();
            Console.WriteLine(string.Format("{0}   {1}", "nope.".PadLeft(10), v));
            ApplyDefaultForeground();
        }

        internal static void DisplayOptions(string[] optionHeadings)
        {
            var controls = new string[] { "A", "S", "D", "F" };
            Console.Write("".PadLeft(2));

            for (int i = 0; i < controls.Length; i++)
            {
                if (optionHeadings.Length >= i + 1)
                {

                    if (!String.IsNullOrWhiteSpace(optionHeadings[i]))
                    {
                        ApplyDefaultForeground();
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.Write(string.Format("[{0}]", controls[i]));


                        ApplyInvertForeground();
                        ApplyHighlightBackground();
                        Console.Write(string.Format(" {0} ", optionHeadings[i].PadRight(8)));

                        ApplyDefaultBackground();
                        Console.Write("".PadLeft(2));
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write("".PadLeft(15));
                    }

                    ApplyDefaultForeground();
                }
            }
        }


        internal static void ApplyErrorForeground()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
        }

        internal static void ApplyMutedForeground()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }

        internal static void ApplyInvertForeground()
        {
            Console.ForegroundColor = ConsoleColor.Black;
        }

        internal static void ApplyDefaultForeground()
        {
            Console.ForegroundColor = ConsoleColor.White;
        }
        internal static void ApplyStatusForeground(TaskStatus status)
        {
            switch (status)
            {
                case TaskStatus.NotStarted:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case TaskStatus.Active:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case TaskStatus.Waiting:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case TaskStatus.Complete:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case TaskStatus.Cancelled:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                default:
                    ApplyDefaultForeground();
                    break;
            }
        }

        internal static void LineBreak(int breaks = 1)
        {
            for (int i = 0; i < breaks; i++)
            {
                Console.WriteLine("".PadRight(70, ' '));
            }
        }


        internal static void ApplyDefaultBackground()
        {
            Console.BackgroundColor = ConsoleColor.Black;
        }

        internal static void ApplyHighlightBackground()
        {
            Console.BackgroundColor = ConsoleColor.Gray;
        }
    }
}