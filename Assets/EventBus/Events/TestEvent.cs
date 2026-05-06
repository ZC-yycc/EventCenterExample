using UnityEngine;

public struct TestEvent : IEventBus
{
    
}

public struct TestEvent_i : IEventBus
{
    public int i;
}

public struct TestEvent_ii : IEventBus
{
    public int i1;
    public int i2;
}

public struct TestEvent_iii : IEventBus
{
    public int i1;
    public int i2;
    public int i3;
}
