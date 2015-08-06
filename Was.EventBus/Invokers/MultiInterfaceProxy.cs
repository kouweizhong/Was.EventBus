namespace Was.EventBus.Invokers
{
    using Castle.DynamicProxy;
    using Extensions;
    using NAUcrm.EventBus.Exception;
    using System;
    using System.Collections;
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
                this.InvokeWithEnumerableReturn(invocation, returnType);
            }
            else
            {
                this.InvokeVoid(invocation);
            }
        }

        private void InvokeVoid(IInvocation invocation)
        {
            object result = null;

            foreach (var @event in this.Events)
            {
                try
                {
                    result = invocation.Method.Invoke(@event, invocation.Arguments);
                }
                catch (Exception ex)
                {
                    this.HandleException(ex, @event);
                }
            }

            invocation.ReturnValue = result;
        }

        private void InvokeWithEnumerableReturn(IInvocation invocation, Type returnType)
        {
            var list =
                (IList)
                Activator.CreateInstance(typeof(List<>).MakeGenericType(returnType.GetGenericArguments()[0]));

            foreach (var @event in this.Events)
            {
                try
                {
                    var enumerable = (IEnumerable)invocation.Method.Invoke(@event, invocation.Arguments);
                    if (enumerable == null)
                    {
                        continue;
                    }

                    var enumerator = enumerable.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        list.Add(enumerator.Current);
                    }
                }
                catch (Exception ex)
                {
                    this.HandleException(ex, @event);
                }
            }

            invocation.ReturnValue = list;
        }

        private void HandleException(Exception ex, IEvent proxiedEvent)
        {
            if (!(ex is TargetInvocationException))
            {
                throw ex;
            }

            var tex = ex.InnerException;
            if (tex == null)
            {
                throw ex;
            }

            if (!(tex is EventFatalException) && !proxiedEvent.IsAlwaysFatal())
            {
                return;
            }

            if (tex.InnerException != null)
            {
                tex = tex.InnerException;
            }

            throw tex;
        }
    }
}
