
namespace NAUcrm.EventBus.Exception
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class EventFatalException : Exception
    {
        public EventFatalException() { }
        public EventFatalException(string message) : base(message) { }
        public EventFatalException(string message, Exception inner) : base(message, inner) { }

        protected EventFatalException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }
    }
}
