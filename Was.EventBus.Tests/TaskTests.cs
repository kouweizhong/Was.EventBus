using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniAutFac;
using MiniAutFac.Interfaces;
using Was.EventBus.MiniAutFac;
using Was.EventBus.Tests.TaskEvent;

namespace Was.EventBus.Tests
{
    [TestClass]
    public class TaskTests
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
        public async Task TestAsyncTask()
        {
            var ev = this.container.Resolve<ITestTaskEvent>();

            await ev.SomeAwaitable();

            Assert.AreEqual(2, T0.Calls);
        }
    }
}
