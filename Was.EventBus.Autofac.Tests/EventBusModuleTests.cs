namespace Was.EventBus.Autofac.Tests
{
    using System.Reflection;
    using global::Autofac;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SampleEvent;

    [TestClass]
    public class EventBusModu
    {
        private IContainer container;

        [TestInitialize]
        public void Up()
        {
            var bld = new ContainerBuilder();

            bld.RegisterModule(new EventBusModule(new[] { Assembly.GetExecutingAssembly() }));

            this.container = bld.Build();
        }

        [TestMethod]
        public void SomeTest()
        {
            var eventProxy = this.container.Resolve<ISomeEvent>();

            Assert.IsFalse(CheckCtorCalledEvent.CtorCalled);

            eventProxy.CallEvent();

            Assert.IsTrue(CheckCtorCalledEvent.CtorCalled);
        }
    }
}
