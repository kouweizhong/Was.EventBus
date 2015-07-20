namespace Was.EventBus.Autofac.Tests.SampleEvent
{
    using System;
    using Attributes;

    [AlwaysFatalEvent]
    public class AlwaysFatalEventImpl : IAlwaysFatalEvent
    {
        public void Do()
        {
            throw new InvalidOperationException();
        }
    }
}
