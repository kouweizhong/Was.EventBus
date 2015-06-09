namespace Was.EventBus.Autofac.Tests.ExceptionEvent
{
    using System;
    using NAUcrm.EventBus.Exception;

    public class ThrowableEvent : IEventWithException
    {
        public void Invoke()
        {
            throw new InvalidOperationException();
        }

        public void InvokeFatal()
        {
            throw new EventFatalException("tst", new AggregateException());
        }
    }
}
