namespace OpenTeamProject.EventBus.AutofacIntegration
{
    using Autofac;
    using Invokers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Module = Autofac.Module;

    public class EventBusModule : Module
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
                       .InstancePerRequest()
                       .Keyed(eventTypeKey, eventType);

                builder.Register(ctx => MultiInterfaceProxy.For(localEventType,
                                                                new ComponentContextProvider(ctx, eventTypeKey),
                                                                resolveEventsImmediately: true))
                       .As(localEventType)
                       .InstancePerRequest();
            }
        }
    }
}
