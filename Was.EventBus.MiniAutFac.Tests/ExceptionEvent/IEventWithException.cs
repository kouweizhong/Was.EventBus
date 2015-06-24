namespace Was.EventBus.MiniAutFac.Tests.ExceptionEvent
{
    public interface IEventWithException : IEvent
    {
        void Invoke();
        void InvokeFatal();
    }
}
