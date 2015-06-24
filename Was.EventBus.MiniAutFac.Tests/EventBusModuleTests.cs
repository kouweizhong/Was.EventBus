namespace Was.EventBus.MiniAutFac.Tests
{
    using System;
    using System.Reflection;
    using global::MiniAutFac;
    using global::MiniAutFac.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MiniAutFac;
    using ExceptionEvent;
    using SampleEvent;

    [TestClass]
    public class EventBusModuleTests
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
        public void LazyEvalution_Test()
        {
            var eventProxy = this.container.Resolve<ISomeEvent>();

            Assert.IsFalse(CheckCtorCalledEvent.CtorCalled);

            eventProxy.CallEvent();

            Assert.IsTrue(CheckCtorCalledEvent.CtorCalled);
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowingException_Test()
        {
            var eventProxy = this.container.Resolve<IEventWithException>();
            eventProxy.Invoke();
            eventProxy.InvokeFatal();
        }
    }
}
