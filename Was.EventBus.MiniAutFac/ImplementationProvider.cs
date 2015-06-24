namespace Was.EventBus.MiniAutFac
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class ImplementationProvider : IImplementationProvider
    {
        private global::MiniAutFac.Context.ActivationContext ctx;
        private readonly object key;

        public ImplementationProvider(global::MiniAutFac.Context.ActivationContext ctx, object key)
        {
            this.ctx = ctx;
            this.key = key;
        }
        public Lazy<IEnumerable<IEvent>> For(Type eventType)
        {
            var allImplementationType = typeof(IEnumerable<>).MakeGenericType(eventType);
            var lazyType = typeof(Lazy<>).MakeGenericType(allImplementationType);

            var allImplementationsLazy = this.key != null
                                             ? this.ctx.CurrentLifetimeScope.ResolveKeyed(lazyType, key)
                                             : this.ctx.CurrentLifetimeScope.Resolve(lazyType);

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
