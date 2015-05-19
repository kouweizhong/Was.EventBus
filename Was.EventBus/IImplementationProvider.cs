namespace Was.EventBus
{
    using System;
    using System.Collections.Generic;

    public interface IImplementationProvider
    {
        Lazy<IEnumerable<IEvent>> For(Type eventType);
    }
}
