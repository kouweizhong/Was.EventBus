namespace Was.EventBus.Tests
{
    using EnumerableEvent;
    using Extensions;
    using global::MiniAutFac;
    using global::MiniAutFac.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MiniAutFac;
    using System.Linq;
    using System.Reflection;

    [TestClass]
    public class EnumerableTest
    {
        private ILifetimeScope container;

        [TestInitialize]
        public void Up()
        {
            var bld = new ContainerBuilder();

            bld.RegisterModule(new EventBusModule(new[] { Assembly.GetExecutingAssembly() }));

            this.container = bld.Build();
        }

        [TestMethod]
        public void InvokingTests()
        {
            var enumEventProxy = this.container.Resolve<IEnumEvent>();
            var coll = enumEventProxy.GetStrings().ToList();

            Assert.AreEqual(2, coll.Count);
        }

        [TestMethod]
        public void Test_GettingAllImplementations()
        {
            var enumEventProxy = this.container.Resolve<IEnumEvent>();
            var registeredEvents = enumEventProxy.GetUnderlyingInstances();

            Assert.AreEqual(2, registeredEvents.Count());
        }
    }
}
