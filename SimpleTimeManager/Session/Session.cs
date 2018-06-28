using Newtonsoft.Json;
using SimpleTimeManager.Tasks;
using System;
using System.Collections.Generic;
using System.IO;

namespace SimpleTimeManager
{
    internal class Session : SessionEnvironment
    {
        internal static void Save(List<SimpleTask> tasks, string fileName)
        {
            var saveData = JsonConvert.SerializeObject(tasks);
            TextWriter writer;
            using (writer = new StreamWriter(fileName, append: false))
            {
                writer.WriteLine(saveData);
            }
        }

        internal static List<SimpleTask> Load(string fileName)
        {
            try
            {
                string saveData = File.ReadAllText(fileName);
                return JsonConvert.DeserializeObject<List<SimpleTask>>(saveData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("info: unable to find save file");
                return null;
            }
        }
    }
}