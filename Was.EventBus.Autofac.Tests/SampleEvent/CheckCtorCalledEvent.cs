namespace Was.EventBus.Autofac.Tests.SampleEvent
{
    public class CheckCtorCalledEvent : ISomeEvent
    {
        public static bool CtorCalled { get; set; }
        public bool EventCalled { get; private set; }

        public CheckCtorCalledEvent()
        {
            CtorCalled = true;
        }

        public void CallEvent()
        {
            this.EventCalled = true;
        }
    }
}
