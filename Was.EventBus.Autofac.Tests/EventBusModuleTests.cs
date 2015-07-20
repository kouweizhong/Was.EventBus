namespace Was.EventBus.Autofac.Tests
{
    using System;
    using System.Reflection;
    using ExceptionEvent;
    using global::Autofac;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NAUcrm.EventBus.Exception;
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

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void Do_AlwaysThrowable_ExceptionExpcted()
        {
            var eventProxy = this.container.Resolve<IAlwaysFatalEvent>();
            eventProxy.Do();
        }
    }
}
