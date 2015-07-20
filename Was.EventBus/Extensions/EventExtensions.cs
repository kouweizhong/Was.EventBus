namespace Was.EventBus.Extensions
{
    using System.Linq;
    using Attributes;

    internal static class EventExtensions
    {
        public static bool IsAlwaysFatal(this IEvent @event)
        {
            return @event.GetType().GetCustomAttributes(typeof(AlwaysFatalEventAttribute), true).Any();
        }
    }
}
