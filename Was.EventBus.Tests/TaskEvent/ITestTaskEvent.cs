using System;
using System.Threading.Tasks;

namespace Was.EventBus.Tests.TaskEvent
{
    public interface ITestTaskEvent : IEvent
    {
        Task SomeAwaitable();
    }

    public class T0
    {
        public static int Calls;
    }

    public class T1 : ITestTaskEvent
    {
        public async Task SomeAwaitable()
        {
            await Task.Factory.StartNew(() => T0.Calls += 1);
        }
    }

    public class T2 : ITestTaskEvent
    {
        public async Task SomeAwaitable()
        { 
            await Task.Factory.StartNew(() => T0.Calls += 1);
        }
    }
}
