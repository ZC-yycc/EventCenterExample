using EventCenter;
using EventCenterHighPerformance;
using UnityEngine;

public class Signal : MonoBehaviour
{
    void Start()
    {
        float start_time = Time.realtimeSinceStartup;
        this.Publish(EventCenter.EventName.TestEvent);
        this.Publish(EventCenter.EventName.TestEvent_i, 42);
        this.Publish(EventCenter.EventName.TestEvent_ii, 42, 84);
        this.Publish(EventCenter.EventName.TestEvent_iii, 42, 84, 168);
        float elapsed_time = (Time.realtimeSinceStartup - start_time) * 1000f; // 转换为毫秒
        Debug.Log($"EventCenter方法总耗时: {elapsed_time}ms");

        start_time = Time.realtimeSinceStartup;
        this.Publish(EventCenterHighPerformance.EventName.TestEvent);
        this.Publish(EventCenterHighPerformance.EventName.TestEvent_i, 42);
        this.Publish(EventCenterHighPerformance.EventName.TestEvent_ii, 42, 84);
        this.Publish(EventCenterHighPerformance.EventName.TestEvent_iii, 42, 84, 168);
        elapsed_time = (Time.realtimeSinceStartup - start_time) * 1000f;
        Debug.Log($"EventCenterHighPerformance方法总耗时: {elapsed_time}ms");
    }
}
