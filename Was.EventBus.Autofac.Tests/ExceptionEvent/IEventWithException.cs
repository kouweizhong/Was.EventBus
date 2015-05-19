namespace Was.EventBus.Autofac.Tests.ExceptionEvent
{
    public interface IEventWithException : IEvent
    {
        void Invoke();
        void InvokeFatal();
    }
}
