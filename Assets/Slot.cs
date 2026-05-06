using EventCenter;
using EventCenterHighPerformance;
using UnityEngine;

public class Slot : MonoBehaviour
{
    void Awake()
    {
        this.Subscribe(EventCenter.EventName.TestEvent, OnTestEvent);
        this.Subscribe(EventCenter.EventName.TestEvent_i, OnTestEvent_i);
        this.Subscribe(EventCenter.EventName.TestEvent_ii, OnTestEvent_ii);
        this.Subscribe(EventCenter.EventName.TestEvent_iii, OnTestEvent_iii);

        this.Subscribe(EventCenterHighPerformance.EventName.TestEvent, OnTestEvent);
        this.Subscribe(EventCenterHighPerformance.EventName.TestEvent_i, OnTestEvent_i);
        this.Subscribe(EventCenterHighPerformance.EventName.TestEvent_ii, OnTestEvent_ii);
        this.Subscribe(EventCenterHighPerformance.EventName.TestEvent_iii, OnTestEvent_iii);

        EventBus<TestEvent>.OnEvent += OnTestEvent;
        EventBus<TestEvent_i, int>.OnEvent += OnTestEvent_i;
        EventBus<TestEvent_ii, int, int>.OnEvent += OnTestEvent_ii;
        EventBus<TestEvent_iii, int, int, int>.OnEvent += OnTestEvent_iii;
    }

    private void OnTestEvent()
    {
        Debug.Log("TestEvent received!");
    }
    private void OnTestEvent_i(int i)
    {
        Debug.Log($"TestEvent_i received with arg: {i}");
    }
    private void OnTestEvent_ii(int i1, int i2)
    {
        Debug.Log($"TestEvent_ii received with args: {i1}, {i2}");
    }
    private void OnTestEvent_iii(int i1, int i2, int i3)
    {
        Debug.Log($"TestEvent_iii received with args: {i1}, {i2}, {i3}");
    }
}
