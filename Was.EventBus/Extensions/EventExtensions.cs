namespace Was.EventBus.Extensions
{
    using Attributes;
    using Castle.DynamicProxy;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using Invokers;

    public static class EventExtensions
    {
        public static bool IsAlwaysFatal(this IEvent @event)
        {
            return @event.GetType().GetCustomAttributes(typeof(AlwaysFatalEventAttribute), true).Any();
        }

        public static IEnumerable<TEvent> GetUnderlyingInstances<TEvent>(this TEvent @event)
            where TEvent : class, IEvent
        {
            if (@event == null) throw new ArgumentNullException("event");
            if (!ProxyUtil.IsProxy(@event)) throw new ArgumentException("Event is concrete implementation, not a proxy.", "event");

            var interceptor = ((IProxyTargetAccessor)@event).GetInterceptors().Single();
            var type = interceptor.GetType();
            if (type != typeof(MultiInterfaceProxy))
                throw new ArgumentException("Proxy not created by Was.EventBus.", "event");

            var eventProp = type.GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic);
            var events = (IEnumerable<IEvent>)eventProp.GetValue(interceptor, null);

            return new ReadOnlyCollection<TEvent>(events.Cast<TEvent>().ToList());
        }
    }
}
