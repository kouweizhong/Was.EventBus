namespace Was.EventBus.Providers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class SingleTypeProvider : IImplementationProvider
    {
        private readonly IEnumerable<IEvent> events;
        private readonly Type eventType;

        public SingleTypeProvider(IEnumerable events, Type eventType)
        {
            this.events = events.Cast<IEvent>().ToList();
            this.eventType = eventType;
        }

        public Lazy<IEnumerable<IEvent>> For(Type eventType)
        {
            if (this.eventType != eventType)
            {
                throw new NotSupportedException("Only one type is supported.");
            }

            return new Lazy<IEnumerable<IEvent>>(() => this.events);
        }
    }
}
