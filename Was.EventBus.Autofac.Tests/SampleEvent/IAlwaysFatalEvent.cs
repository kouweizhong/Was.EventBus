namespace Was.EventBus.Autofac.Tests.SampleEvent
{
    public interface IAlwaysFatalEvent : IEvent
    {
        void Do();
    }
}
