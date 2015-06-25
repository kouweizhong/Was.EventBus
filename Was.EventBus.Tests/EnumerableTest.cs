namespace Was.EventBus.Tests
{
    using System.Linq;
    using EnumerableEvent;
    using global::MiniAutFac;
    using global::MiniAutFac.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Reflection;
    using MiniAutFac;

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
        public void TestMethod1()
        {
            var enumEventProxy = this.container.Resolve<IEnumEvent>();
            var coll = enumEventProxy.GetStrings().ToList();

            Assert.AreEqual(2, coll.Count);
        }
    }
}
