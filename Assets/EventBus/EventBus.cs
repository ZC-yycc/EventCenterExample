using UnityEngine;

public interface IEventBus
{
    
}

public static class EventBus<T> where T : IEventBus
{
    public delegate void EventHandler(T event_data);
    private static event EventHandler OnEvent;

    public static void Subscribe(EventHandler handler)
    {
        OnEvent += handler;
    }

    public static void Unsubscribe(EventHandler handler)
    {
        OnEvent -= handler;
    }

    public static void Publish(in T event_data)
    {
        OnEvent?.Invoke(event_data);
    }
}
