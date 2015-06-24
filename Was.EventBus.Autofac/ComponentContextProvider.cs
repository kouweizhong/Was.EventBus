namespace Was.EventBus.Autofac
{
    using System.Collections;
    using System.Linq;
    using System.Reflection;
    using EventBus;
    using global::Autofac;
    using System;
    using System.Collections.Generic;

    public class ComponentContextProvider : IImplementationProvider
    {
        private readonly IComponentContext componentContext;
        private readonly object keyedService;

        public ComponentContextProvider(IComponentContext componentContext, object keyedService = null)
        {
            this.componentContext = componentContext;
            this.keyedService = keyedService;
        }

        public Lazy<IEnumerable<IEvent>> For(Type eventType)
        {
            var allImplementationType = typeof(IEnumerable<>).MakeGenericType(eventType);
            var lazyType = typeof(Lazy<>).MakeGenericType(allImplementationType);

            var allImplementationsLazy = this.keyedService != null
                                             ? this.componentContext.ResolveKeyed(this.keyedService, lazyType)
                                             : this.componentContext.Resolve(lazyType);

            return new Lazy<IEnumerable<IEvent>>(() =>
                                                 {
                                                     var allImpl =
                                                         (IEnumerable)allImplementationsLazy.GetType()
                                                                               .GetProperty("Value",
                                                                                            BindingFlags.Instance |
                                                                                            BindingFlags.Public)
                                                                               .GetValue(allImplementationsLazy,
                                                                                         new object[0]);

                                                     return allImpl.Cast<IEvent>();
                                                 });
        }
    }
}
