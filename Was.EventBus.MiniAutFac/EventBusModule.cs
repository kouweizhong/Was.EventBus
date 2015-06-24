namespace Was.EventBus.MiniAutFac
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using global::MiniAutFac;
    using Invokers;

    public class EventBusModule : global::MiniAutFac.Modules.Module
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

        public override void Registration(ContainerBuilder builder)
        {
            foreach (var eventType in this.GetAllEventTypes())
            {
                var eventTypeKey = new object();

                foreach (var asmType in this.GetTypesForEvent(eventType))
                {
                    builder.Register(asmType).PerLifetimeScope().Keyed(eventTypeKey).As(eventType);
                }

                var type = eventType;
                builder.Register(ctx => MultiInterfaceProxy.For(type, new ImplementationProvider(ctx, eventTypeKey)))
                       .PerLifetimeScope()
                       .As(eventType);
            }
        }

        private IEnumerable<Type> GetTypesForEvent(Type eventType)
        {
            return
                this.assemblies.SelectMany(asm => asm.GetTypes().Where(eventType.IsAssignableFrom))
                    .Where(t => t.IsClass && !t.IsAbstract);
        }
    }
}
