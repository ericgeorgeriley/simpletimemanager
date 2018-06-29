using System.Collections.Generic;
using System.Reflection;
using SimpleTimeManager.Tasks;
using Xunit.Sdk;

namespace SimpleTimeManager.Tests.TestData
{
    public class TaskIndexTestDataAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { 0 };
            yield return new object[] { 1 };
            yield return new object[] { 2 };
            yield return new object[] { 3 };
            yield return new object[] { 4 };
            yield return new object[] { 5 };
            yield return new object[] { 6 };
            yield return new object[] { 7 };
            yield return new object[] { 8 };
            yield return new object[] { 9 };
            yield return new object[] { 10 };
            yield return new object[] { 11 };
        }
    }

    public class TaskStatusTestDataAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { TaskStatus.NotStarted };
            yield return new object[] { TaskStatus.Active };
            yield return new object[] { TaskStatus.Waiting };
            yield return new object[] { TaskStatus.Complete };
            yield return new object[] { TaskStatus.Cancelled };
        }
    }
}
