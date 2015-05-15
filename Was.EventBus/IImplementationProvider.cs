namespace Was.EventBus
{
    using System;
    using System.Collections.Generic;

    public interface IImplementationProvider
    {
		IEnumerable<IEvent> For(Type eventType);
    }
}
