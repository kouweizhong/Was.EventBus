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


            // ReSharper disable once SuspiciousTypeConversion.Global
            var interceptors = ((IProxyTargetAccessor) @event).GetInterceptors().ToList();
            if (interceptors.Count != 1) throw new ArgumentException("Invalid No of interceptors.");

            var interceptor = interceptors.Single();
            var interceptorType = interceptor.GetType();
            if (interceptorType != typeof(MultiInterfaceProxy))
                throw new ArgumentException("Proxy not created by Was.EventBus.", "event");

            var eventProp = interceptorType.GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic);
            var events = (IEnumerable<IEvent>)eventProp.GetValue(interceptor, null);

            return new ReadOnlyCollection<TEvent>(events.Cast<TEvent>().ToList());
        }
    }
}
