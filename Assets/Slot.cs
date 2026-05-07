using EventCenterDictionary;
using EventCenterArray;
using EventBus;
using UnityEngine;

public class Slot : MonoBehaviour
{
    void Awake()
    {
        this.Subscribe(EventCenterDictionary.EventName.TestEvent, OnTestEvent);
        this.Subscribe(EventCenterDictionary.EventName.TestEvent_i, OnTestEvent_i);
        this.Subscribe(EventCenterDictionary.EventName.TestEvent_ii, OnTestEvent_ii);
        this.Subscribe(EventCenterDictionary.EventName.TestEvent_iii, OnTestEvent_iii);

        this.Subscribe(EventCenterArray.EventName.TestEvent, OnTestEvent);
        this.Subscribe(EventCenterArray.EventName.TestEvent_i, OnTestEvent_i);
        this.Subscribe(EventCenterArray.EventName.TestEvent_ii, OnTestEvent_ii);
        this.Subscribe(EventCenterArray.EventName.TestEvent_iii, OnTestEvent_iii);

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
