namespace EventCenterHighPerformance
{
    using static EventCenter;

    public static class EventName
    {
        public static readonly EventKey TestEvent = new EventKey();
        public static readonly EventKey<int> TestEvent_i = new EventKey<int>();
        public static readonly EventKey<int, int> TestEvent_ii = new EventKey<int, int>();
        public static readonly EventKey<int, int, int> TestEvent_iii = new EventKey<int, int, int>();
    }
}