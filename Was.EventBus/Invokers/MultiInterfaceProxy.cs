namespace Was.EventBus.Invokers
{
    using System.Net.Mime;
    using Castle.DynamicProxy;
    using NAUcrm.EventBus.Exception;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class MultiInterfaceProxy : IInterceptor
    {
        private readonly IImplementationProvider implementationProvider;
        private readonly Type eventType;

        private readonly Lazy<IEnumerable<IEvent>> events;
        private IEnumerable<MethodInfo> eventMethods;

        private MultiInterfaceProxy(Type eventType, IImplementationProvider implementationProvider)
        {
            this.implementationProvider = implementationProvider;
            this.eventType = eventType;

            this.events = this.implementationProvider.For(this.eventType);
        }

        private IEnumerable<IEvent> Events
        {
            get { return this.events.Value; }
        }

        public static object For(Type eventType, IImplementationProvider implementationProvider)
        {
            if (!eventType.IsInterface)
            {
                throw new ArgumentException("Event must be an interface.", "eventType");
            }

            if (!typeof(IEvent).IsAssignableFrom(eventType))
            {
                throw new ArgumentException("Event must be derrived from IEvent", "eventType");
            }

            var generator = new ProxyGenerator();
            var proxtInterceptor =
                new MultiInterfaceProxy(eventType,
                                        implementationProvider);

            var instance = generator.CreateInterfaceProxyWithoutTarget(eventType, proxtInterceptor);

            return instance;
        }


        public void Intercept(IInvocation invocation)
        {
            var returnType = invocation.Method.ReturnType;
            if (returnType.IsGenericType &&
                typeof(IEnumerable<>).IsAssignableFrom(returnType.GetGenericTypeDefinition()))
            {
                var list =
                    (dynamic)
                    Activator.CreateInstance(typeof(List<>).MakeGenericType(returnType.GetGenericArguments()[0]));

                foreach (var @event in this.Events)
                {
                    try
                    {
                        dynamic localList = invocation.Method.Invoke(@event, invocation.Arguments);

                        foreach (var elem in localList.ToList())
                        {
                            list.Add(elem);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!(ex is EventFatalException))
                        {
                            continue;
                        }

                        if (ex.InnerException != null)
                        {
                            ex = ex.InnerException;
                        }

                        throw ex;
                    }
                }

                invocation.ReturnValue = list;

                return;
            }

            object result = null;

            foreach (var @event in this.Events)
            {
                try
                {
                    result = invocation.Method.Invoke(@event, invocation.Arguments);
                }
                catch (Exception ex)
                {
                    if (!(ex is TargetInvocationException))
                    {
                        continue;
                    }

                    var tex = ex.InnerException;
                    if (tex == null)
                    {
                        continue;
                    }

                    if (tex is EventFatalException)
                    {
                        if (tex.InnerException != null)
                        {
                            tex = tex.InnerException;
                        }

                        throw tex;
                    }
                }
            }

            invocation.ReturnValue = result;
        }
    }
}
