namespace Was.EventBus.Tests.EnumerableEvent
{
    using System.Collections.Generic;

    public interface IEnumEvent : IEvent
    {
        IEnumerable<string> GetStrings();
    }

    public class SomeEventEvent : IEnumEvent
    {
        public IEnumerable<string> GetStrings()
        {
            yield return "1";
        }
    }

    public class SomeOtherEventEvent : IEnumEvent
    {
        public IEnumerable<string> GetStrings()
        {
            yield return "2";
        }
    }
}
