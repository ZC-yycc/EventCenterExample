using EventCenterDictionary;
using EventCenterArray;
using EventBus;
using UnityEngine;

public class Signal : MonoBehaviour
{
    void Start()
    {
        float start_time = Time.realtimeSinceStartup;
        this.Publish(EventCenterDictionary.EventName.TestEvent);
        this.Publish(EventCenterDictionary.EventName.TestEvent_i, 42);
        this.Publish(EventCenterDictionary.EventName.TestEvent_ii, 42, 84);
        this.Publish(EventCenterDictionary.EventName.TestEvent_iii, 42, 84, 168);
        float elapsed_time = (Time.realtimeSinceStartup - start_time) * 1000f; // 转换为毫秒
        Debug.Log($"EventCenter方法总耗时: {elapsed_time}ms");

        start_time = Time.realtimeSinceStartup;
        this.Publish(EventCenterArray.EventName.TestEvent);
        this.Publish(EventCenterArray.EventName.TestEvent_i, 42);
        this.Publish(EventCenterArray.EventName.TestEvent_ii, 42, 84);
        this.Publish(EventCenterArray.EventName.TestEvent_iii, 42, 84, 168);
        elapsed_time = (Time.realtimeSinceStartup - start_time) * 1000f;
        Debug.Log($"EventCenterHighPerformance方法总耗时: {elapsed_time}ms");

        start_time = Time.realtimeSinceStartup;
        EventBus<TestEvent>.Publish();
        EventBus<TestEvent_i, int>.Publish(42);
        EventBus<TestEvent_ii, int, int>.Publish(42, 84);
        EventBus<TestEvent_iii, int, int, int>.Publish(42, 84, 168);
        elapsed_time = (Time.realtimeSinceStartup - start_time) * 1000f;
        Debug.Log($"EventBus方法总耗时: {elapsed_time}ms");
    }
}
