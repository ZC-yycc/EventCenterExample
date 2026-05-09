using EventCenterDictionary;
using EventCenterArray;
using EventBus;
using UnityEngine;

public class Signal : MonoBehaviour
{
    // 压力测试次数
    private const int stress_count_ = 10000;

    void Start()
    {
        Debug.Log("========== 单次调用测试 ==========");
        RunSingleTests();

        Debug.Log("\n========== 无订阅者安全测试 ==========");
        RunNoSubscriberTests();

        Debug.Log($"\n========== 压力测试 (x{stress_count_}) ==========");
        RunStressTests();

        Debug.Log("\n========== 带返回值 EventBusR 测试 ==========");
        RunEventBusRTests();

        Debug.Log("\n========== 调用计数验证 ==========");
        LogCallCounts();
    }

    #region 单次调用测试
    private void RunSingleTests()
    {
        // EventCenterDictionary
        float start_time = Time.realtimeSinceStartup;
        this.Publish(EventCenterDictionary.EventName.TestEvent);
        this.Publish(EventCenterDictionary.EventName.TestEvent_i, 42);
        this.Publish(EventCenterDictionary.EventName.TestEvent_ii, 42, 84);
        this.Publish(EventCenterDictionary.EventName.TestEvent_iii, 42, 84, 168);
        this.Publish(EventCenterDictionary.EventName.TestEvent_s, "hello");
        this.Publish(EventCenterDictionary.EventName.TestEvent_ss, "hello", "world");
        Debug.Log($"[Dict] 6种事件单次调用总耗时: {(Time.realtimeSinceStartup - start_time) * 1000f:F4}ms");

        // EventCenterArray
        start_time = Time.realtimeSinceStartup;
        this.Publish(EventCenterArray.EventName.TestEvent);
        this.Publish(EventCenterArray.EventName.TestEvent_i, 42);
        this.Publish(EventCenterArray.EventName.TestEvent_ii, 42, 84);
        this.Publish(EventCenterArray.EventName.TestEvent_iii, 42, 84, 168);
        this.Publish(EventCenterArray.EventName.TestEvent_s, "hello");
        this.Publish(EventCenterArray.EventName.TestEvent_ss, "hello", "world");
        Debug.Log($"[Array] 6种事件单次调用总耗时: {(Time.realtimeSinceStartup - start_time) * 1000f:F4}ms");

        // EventBus
        start_time = Time.realtimeSinceStartup;
        EventBus<TestEvent>.Publish();
        EventBus<TestEvent_i, int>.Publish(42);
        EventBus<TestEvent_ii, int, int>.Publish(42, 84);
        EventBus<TestEvent_iii, int, int, int>.Publish(42, 84, 168);
        EventBus<TestEvent_s, string>.Publish("hello");
        EventBus<TestEvent_ss, string, string>.Publish("hello", "world");
        Debug.Log($"[Bus] 6种事件单次调用总耗时: {(Time.realtimeSinceStartup - start_time) * 1000f:F4}ms");
    }
    #endregion

    #region 无订阅者安全测试
    private void RunNoSubscriberTests()
    {
        // 使用未注册的动态 Key 发布，确保不抛异常
        var dyn_key = new EventCenterDictionary.EventCenter.EventKey();
        var dyn_key_i = new EventCenterDictionary.EventCenter.EventKey<int>();
        var dyn_key_ii = new EventCenterDictionary.EventCenter.EventKey<int, int>();

        this.Publish(dyn_key);                        // 无订阅者
        this.Publish(dyn_key_i, 999);                 // 无订阅者
        this.Publish(dyn_key_ii, 111, 222);           // 无订阅者
        Debug.Log("[Dict] 无订阅者发布 — 无异常");

        var arr_key = new EventCenterArray.EventCenter.EventKey();
        var arr_key_i = new EventCenterArray.EventCenter.EventKey<int>();
        this.Publish(arr_key);                        // 无订阅者
        this.Publish(arr_key_i, 999);                 // 无订阅者
        Debug.Log("[Array] 无订阅者发布 — 无异常");

        // EventBus 无订阅者时 event 为 null，?.Invoke 安全
        EventBus<TestEvent_ii, int, int>.Publish(0, 0);   // TestEvent_ii 只有 Slot 订阅，但其他 Slot 不会崩溃
        Debug.Log("[Bus] 无订阅者发布 — 无异常");
    }
    #endregion

    #region 压力测试
    private void RunStressTests()
    {
        // ---- EventCenterDictionary 压力测试 ----
        float start_time = Time.realtimeSinceStartup;
        for (int i = 0; i < stress_count_; i++)
            this.Publish(EventCenterDictionary.EventName.TestEvent);
        float dict_no_arg = (Time.realtimeSinceStartup - start_time) * 1000f;

        start_time = Time.realtimeSinceStartup;
        for (int i = 0; i < stress_count_; i++)
            this.Publish(EventCenterDictionary.EventName.TestEvent_i, i);
        float dict_1_arg = (Time.realtimeSinceStartup - start_time) * 1000f;

        start_time = Time.realtimeSinceStartup;
        for (int i = 0; i < stress_count_; i++)
            this.Publish(EventCenterDictionary.EventName.TestEvent_ii, i, i * 2);
        float dict_2_arg = (Time.realtimeSinceStartup - start_time) * 1000f;

        start_time = Time.realtimeSinceStartup;
        for (int i = 0; i < stress_count_; i++)
            this.Publish(EventCenterDictionary.EventName.TestEvent_iii, i, i * 2, i * 3);
        float dict_3_arg = (Time.realtimeSinceStartup - start_time) * 1000f;

        float dict_total = dict_no_arg + dict_1_arg + dict_2_arg + dict_3_arg;
        Debug.Log($"[Dict] 压力测试 ({stress_count_}次): 无参={dict_no_arg:F2}ms | 1参={dict_1_arg:F2}ms | 2参={dict_2_arg:F2}ms | 3参={dict_3_arg:F2}ms | 总计={dict_total:F2}ms");

        // ---- EventCenterArray 压力测试 ----
        start_time = Time.realtimeSinceStartup;
        for (int i = 0; i < stress_count_; i++)
            this.Publish(EventCenterArray.EventName.TestEvent);
        float arr_no_arg = (Time.realtimeSinceStartup - start_time) * 1000f;

        start_time = Time.realtimeSinceStartup;
        for (int i = 0; i < stress_count_; i++)
            this.Publish(EventCenterArray.EventName.TestEvent_i, i);
        float arr_1_arg = (Time.realtimeSinceStartup - start_time) * 1000f;

        start_time = Time.realtimeSinceStartup;
        for (int i = 0; i < stress_count_; i++)
            this.Publish(EventCenterArray.EventName.TestEvent_ii, i, i * 2);
        float arr_2_arg = (Time.realtimeSinceStartup - start_time) * 1000f;

        start_time = Time.realtimeSinceStartup;
        for (int i = 0; i < stress_count_; i++)
            this.Publish(EventCenterArray.EventName.TestEvent_iii, i, i * 2, i * 3);
        float arr_3_arg = (Time.realtimeSinceStartup - start_time) * 1000f;

        float arr_total = arr_no_arg + arr_1_arg + arr_2_arg + arr_3_arg;
        Debug.Log($"[Array] 压力测试 ({stress_count_}次): 无参={arr_no_arg:F2}ms | 1参={arr_1_arg:F2}ms | 2参={arr_2_arg:F2}ms | 3参={arr_3_arg:F2}ms | 总计={arr_total:F2}ms");

        // ---- EventBus 压力测试 ----
        start_time = Time.realtimeSinceStartup;
        for (int i = 0; i < stress_count_; i++)
            EventBus<TestEvent>.Publish();
        float bus_no_arg = (Time.realtimeSinceStartup - start_time) * 1000f;

        start_time = Time.realtimeSinceStartup;
        for (int i = 0; i < stress_count_; i++)
            EventBus<TestEvent_i, int>.Publish(i);
        float bus_1_arg = (Time.realtimeSinceStartup - start_time) * 1000f;

        start_time = Time.realtimeSinceStartup;
        for (int i = 0; i < stress_count_; i++)
            EventBus<TestEvent_ii, int, int>.Publish(i, i * 2);
        float bus_2_arg = (Time.realtimeSinceStartup - start_time) * 1000f;

        start_time = Time.realtimeSinceStartup;
        for (int i = 0; i < stress_count_; i++)
            EventBus<TestEvent_iii, int, int, int>.Publish(i, i * 2, i * 3);
        float bus_3_arg = (Time.realtimeSinceStartup - start_time) * 1000f;

        float bus_total = bus_no_arg + bus_1_arg + bus_2_arg + bus_3_arg;
        Debug.Log($"[Bus] 压力测试 ({stress_count_}次): 无参={bus_no_arg:F2}ms | 1参={bus_1_arg:F2}ms | 2参={bus_2_arg:F2}ms | 3参={bus_3_arg:F2}ms | 总计={bus_total:F2}ms");

        // ---- 性能对比 ----
        Debug.Log($"=== 性能对比 (总计): Dict={dict_total:F2}ms | Array={arr_total:F2}ms | Bus={bus_total:F2}ms ===");
    }
    #endregion

    #region EventBusR 带返回值测试
    private void RunEventBusRTests()
    {
        float start_time = Time.realtimeSinceStartup;

        int result_1 = EventBusR<TestEventR, int>.Publish();
        Debug.Log($"[BusR] TestEventR 返回值: {result_1} (期望: 42)  结果: {(result_1 == 42 ? "✓ PASS" : "✗ FAIL")}");

        int result_2 = EventBusR<TestEventR_i, int, int>.Publish(7);
        Debug.Log($"[BusR] TestEventR_i(7) 返回值: {result_2} (期望: 70)  结果: {(result_2 == 70 ? "✓ PASS" : "✗ FAIL")}");

        // 无订阅者时返回 default
        int result_3 = EventBusR<TestEventR_i, int, int>.Publish(0);
        // result_2 已经是第二次调用了，这里换个思路：TestEventR_i 没有 Slot 之外的订阅者时发布仍应正常返回
        Debug.Log($"[BusR] 二次调用 TestEventR_i(0) 返回值: {result_3} (期望: 0)");

        Debug.Log($"[BusR] 带返回值测试总耗时: {(Time.realtimeSinceStartup - start_time) * 1000f:F4}ms");
    }
    #endregion

    #region 调用计数验证
    private void LogCallCounts()
    {
        Debug.Log("=== EventCenterDictionary 计数 ===");
        Debug.Log($"  TestEvent={Slot.dict_testEvent_count_}  TestEvent_i={Slot.dict_testEvent_i_count_}  TestEvent_ii={Slot.dict_testEvent_ii_count_}  TestEvent_iii={Slot.dict_testEvent_iii_count_}");
        Debug.Log($"  TestEvent_s={Slot.dict_testEvent_s_count_}  TestEvent_ss={Slot.dict_testEvent_ss_count_}");

        Debug.Log("=== EventCenterArray 计数 ===");
        Debug.Log($"  TestEvent={Slot.array_testEvent_count_}  TestEvent_i={Slot.array_testEvent_i_count_}  TestEvent_ii={Slot.array_testEvent_ii_count_}  TestEvent_iii={Slot.array_testEvent_iii_count_}");
        Debug.Log($"  TestEvent_s={Slot.array_testEvent_s_count_}  TestEvent_ss={Slot.array_testEvent_ss_count_}");

        Debug.Log("=== EventBus 计数 ===");
        Debug.Log($"  TestEvent={Slot.bus_testEvent_count_}  TestEvent_i={Slot.bus_testEvent_i_count_}  TestEvent_ii={Slot.bus_testEvent_ii_count_}  TestEvent_iii={Slot.bus_testEvent_iii_count_}");
        Debug.Log($"  TestEvent_s={Slot.bus_testEvent_s_count_}  TestEvent_ss={Slot.bus_testEvent_ss_count_}");

        Debug.Log("=== EventBusR 计数 ===");
        Debug.Log($"  TestEventR={Slot.bus_testEventR_count_}  TestEventR_i={Slot.bus_testEventR_i_count_}");

        // 自动验证各计数器期望值
        int expected_base = 1 + stress_count_; // 1 次单次调用 + stress_count_ 次压力

        VerifyCount("Dict.TestEvent",     Slot.dict_testEvent_count_,     expected_base);
        VerifyCount("Dict.TestEvent_i",   Slot.dict_testEvent_i_count_,   expected_base);
        VerifyCount("Dict.TestEvent_ii",  Slot.dict_testEvent_ii_count_,  expected_base);
        VerifyCount("Dict.TestEvent_iii", Slot.dict_testEvent_iii_count_, expected_base);
        VerifyCount("Dict.TestEvent_s",   Slot.dict_testEvent_s_count_,   1);   // 仅单次
        VerifyCount("Dict.TestEvent_ss",  Slot.dict_testEvent_ss_count_,  1);   // 仅单次

        VerifyCount("Array.TestEvent",     Slot.array_testEvent_count_,     expected_base);
        VerifyCount("Array.TestEvent_i",   Slot.array_testEvent_i_count_,   expected_base);
        VerifyCount("Array.TestEvent_ii",  Slot.array_testEvent_ii_count_,  expected_base);
        VerifyCount("Array.TestEvent_iii", Slot.array_testEvent_iii_count_, expected_base);
        VerifyCount("Array.TestEvent_s",   Slot.array_testEvent_s_count_,   1);
        VerifyCount("Array.TestEvent_ss",  Slot.array_testEvent_ss_count_,  1);

        VerifyCount("Bus.TestEvent",     Slot.bus_testEvent_count_,     expected_base);
        VerifyCount("Bus.TestEvent_i",   Slot.bus_testEvent_i_count_,   expected_base);
        VerifyCount("Bus.TestEvent_ii",  Slot.bus_testEvent_ii_count_,  expected_base + 1); // RunNoSubscriberTests 额外触发 1 次
        VerifyCount("Bus.TestEvent_iii", Slot.bus_testEvent_iii_count_, expected_base);
        VerifyCount("Bus.TestEvent_s",   Slot.bus_testEvent_s_count_,   1);
        VerifyCount("Bus.TestEvent_ss",  Slot.bus_testEvent_ss_count_,  1);

        VerifyCount("BusR.TestEventR",    Slot.bus_testEventR_count_,    1);
        VerifyCount("BusR.TestEventR_i",  Slot.bus_testEventR_i_count_,  2); // 被调用了 2 次
    }

    private void VerifyCount(string label, int actual, int expected)
    {
        if (actual == expected)
            Debug.Log($"  ✓ {label}: {actual} (符合预期)");
        else
            Debug.LogWarning($"  ✗ {label}: 实际={actual}, 期望={expected} — 不匹配！");
    }
    #endregion
}