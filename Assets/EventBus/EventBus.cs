namespace EventBus
{
    public interface IEventBus { }
    public interface IEventBus<T1> { }
    public interface IEventBus<T1, T2> { }
    public interface IEventBus<T1, T2, T3> { }

    public static class EventBus<EventType> where EventType : IEventBus
    {
        public delegate void EventHandler();
        public static event EventHandler OnEvent;
        public static void Publish()
        {
            OnEvent?.Invoke();
        }
    }

    public static class EventBus<EventType, T1> where EventType : IEventBus<T1>
    {
        public delegate void EventHandler(T1 arg1);
        public static event EventHandler OnEvent;
        public static void Publish(in T1 arg1)
        {
            OnEvent?.Invoke(arg1);
        }
    }

    public static class EventBus<EventType, T1, T2> where EventType : IEventBus<T1, T2>
    {
        public delegate void EventHandler(T1 arg1, T2 arg2);
        public static event EventHandler OnEvent;
        public static void Publish(in T1 arg1, in T2 arg2)
        {
            OnEvent?.Invoke(arg1, arg2);
        }
    }

    public static class EventBus<EventType, T1, T2, T3> where EventType : IEventBus<T1, T2, T3>
    {
        public delegate void EventHandler(T1 arg1, T2 arg2, T3 arg3);
        public static event EventHandler OnEvent;
        public static void Publish(in T1 arg1, in T2 arg2, in T3 arg3)
        {
            OnEvent?.Invoke(arg1, arg2, arg3);
        }
    }

    // ===== Version with return value =====

    public interface IEventBusR<TResult> { }
    public interface IEventBusR<T1, TResult> { }
    public interface IEventBusR<T1, T2, TResult> { }
    public interface IEventBusR<T1, T2, T3, TResult> { }

    /// <summary>
    /// EventBus with return value. Multicast delegate returns the last subscriber's result.
    /// </summary>
    public static class EventBusR<EventType, TResult> where EventType : IEventBusR<TResult>
    {
        public delegate TResult EventHandler();
        public static event EventHandler OnEvent;
        public static TResult Publish()
        {
            if (OnEvent != null) return OnEvent();
            return default;
        }
    }

    public static class EventBusR<EventType, T1, TResult> where EventType : IEventBusR<T1, TResult>
    {
        public delegate TResult EventHandler(T1 arg1);
        public static event EventHandler OnEvent;
        public static TResult Publish(in T1 arg1)
        {
            if (OnEvent != null) return OnEvent(arg1);
            return default;
        }
    }

    public static class EventBusR<EventType, T1, T2, TResult> where EventType : IEventBusR<T1, T2, TResult>
    {
        public delegate TResult EventHandler(T1 arg1, T2 arg2);
        public static event EventHandler OnEvent;
        public static TResult Publish(in T1 arg1, in T2 arg2)
        {
            if (OnEvent != null) return OnEvent(arg1, arg2);
            return default;
        }
    }

    public static class EventBusR<EventType, T1, T2, T3, TResult> where EventType : IEventBusR<T1, T2, T3, TResult>
    {
        public delegate TResult EventHandler(T1 arg1, T2 arg2, T3 arg3);
        public static event EventHandler OnEvent;
        public static TResult Publish(in T1 arg1, in T2 arg2, in T3 arg3)
        {
            if (OnEvent != null) return OnEvent(arg1, arg2, arg3);
            return default;
        }
    }
}
