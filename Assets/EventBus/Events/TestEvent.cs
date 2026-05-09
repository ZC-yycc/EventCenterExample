namespace EventBus
{
    // 定义事件类型 — void 发布
    public struct TestEvent : IEventBus { }

    public struct TestEvent_i : IEventBus<int> { }

    public struct TestEvent_ii : IEventBus<int, int> { }

    public struct TestEvent_iii : IEventBus<int, int, int> { }

    // 定义事件类型 — 带返回值发布
    public struct TestEventR : IEventBusR<int> { }

    public struct TestEventR_i : IEventBusR<int, int> { }

    // 定义事件类型 — string 参数
    public struct TestEvent_s : IEventBus<string> { }

    public struct TestEvent_ss : IEventBus<string, string> { }
}
