namespace OpenTeamProject.EventBus.AutofacIntegration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;

    public class ComponentContextProvider : IImplementationProvider
    {
        private readonly IComponentContext componentContext;
        private readonly object keyedService;

        public ComponentContextProvider(IComponentContext componentContext, object keyedService = null)
        {
            this.componentContext = componentContext;
            this.keyedService = keyedService;
        }

        public IEnumerable<IEvent> For(Type eventType)
        {
            var allImplementationType = typeof(IEnumerable<>).MakeGenericType(eventType);

            IEnumerable allImplementations;
            if (keyedService != null)
            {
                allImplementations =
                    (IEnumerable)this.componentContext.ResolveKeyed(this.keyedService, allImplementationType);
            }
            else
            {
                allImplementations =
                    (IEnumerable)this.componentContext.Resolve(allImplementationType);

            }

            return allImplementations.Cast<IEvent>();
        }
    }
}
