namespace Was.EventBus.Extensions
{
    using Attributes;
    using Castle.DynamicProxy;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

    public static class EventExtensions
    {
        public static bool IsAlwaysFatal(this IEvent @event)
        {
            return @event.GetType().GetCustomAttributes(typeof(AlwaysFatalEventAttribute), true).Any();
        }

        public static IEnumerable<IEvent> GetUnderlyingInstances(this IEvent @event)
        {
            if (@event == null) throw new ArgumentNullException("@event");
            if (!ProxyUtil.IsProxy(@event)) throw new ArgumentException("Event is concrete implementation, not a proxy.", "@event");

            var interceptor = ((IProxyTargetAccessor)@event).GetInterceptors().Single();

            var type = interceptor.GetType();
            var eventProp = type.GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic);
            var events = (IEnumerable<IEvent>)eventProp.GetValue(interceptor, null);

            return new ReadOnlyCollection<IEvent>(events as List<IEvent> ?? events.ToList());
        }
    }
}
