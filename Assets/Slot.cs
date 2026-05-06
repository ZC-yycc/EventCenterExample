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

        EventBus<TestEvent>.Subscribe(OnTestEvent);
        EventBus<TestEvent_i>.Subscribe(OnTestEvent_i);
        EventBus<TestEvent_ii>.Subscribe(OnTestEvent_ii);
        EventBus<TestEvent_iii>.Subscribe(OnTestEvent_iii);
    }

    private void OnTestEvent(TestEvent e)
    {
        Debug.Log("TestEvent received from EventBus!");
    }
    private void OnTestEvent_i(TestEvent_i e)
    {
        Debug.Log($"TestEvent_i received from EventBus with arg: {e.i}");
    }
    private void OnTestEvent_ii(TestEvent_ii e)
    {
        Debug.Log($"TestEvent_ii received from EventBus with args: {e.i1}, {e.i2}");
    }
    private void OnTestEvent_iii(TestEvent_iii e)
    {
        Debug.Log($"TestEvent_iii received from EventBus with args: {e.i1}, {e.i2}, {e.i3}");
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
