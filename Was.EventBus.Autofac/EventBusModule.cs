namespace Was.EventBus.Autofac
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using global::Autofac;
    using EventBus;
    using Invokers;

    public class EventBusModule : global::Autofac.Module
    {
        private readonly Assembly[] assemblies;

        public EventBusModule(Assembly[] assemblies)
        {
            this.assemblies = assemblies;
        }

        private IEnumerable<Type> GetAllEventTypes()
        {
            return
                this.assemblies.SelectMany(
                                           asm =>
                                           asm.GetTypes()
                                              .Where(type => (typeof(IEvent).IsAssignableFrom(type)))
                                              .Where(type => type.IsInterface)
                                              .Where(type => type != typeof(IEvent))).Distinct();
        }

        protected override void Load(ContainerBuilder builder)
        {
            foreach (var eventType in this.GetAllEventTypes())
            {
                var localEventType = eventType;
                var eventTypeKey = new object();

                builder.RegisterAssemblyTypes(this.assemblies)
                       .AssignableTo(localEventType)
                       .As(localEventType)
                       .InstancePerLifetimeScope()
                       .Keyed(eventTypeKey, eventType);

                builder.Register(ctx => MultiInterfaceProxy.For(localEventType,
                                                                new ComponentContextProvider(ctx, eventTypeKey)))
                       .As(localEventType).InstancePerLifetimeScope();
            }
        }
    }
}
