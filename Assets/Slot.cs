using EventCenterDictionary;
using EventCenterArray;
using EventBus;
using UnityEngine;

public class Slot : MonoBehaviour
{
    // ===== 调用计数器，用于验证回调是否真正被触发 =====
    public static int dict_testEvent_count_;
    public static int dict_testEvent_i_count_;
    public static int dict_testEvent_ii_count_;
    public static int dict_testEvent_iii_count_;
    public static int dict_testEvent_s_count_;
    public static int dict_testEvent_ss_count_;

    public static int array_testEvent_count_;
    public static int array_testEvent_i_count_;
    public static int array_testEvent_ii_count_;
    public static int array_testEvent_iii_count_;
    public static int array_testEvent_s_count_;
    public static int array_testEvent_ss_count_;

    public static int bus_testEvent_count_;
    public static int bus_testEvent_i_count_;
    public static int bus_testEvent_ii_count_;
    public static int bus_testEvent_iii_count_;
    public static int bus_testEvent_s_count_;
    public static int bus_testEvent_ss_count_;

    public static int bus_testEventR_count_;
    public static int bus_testEventR_i_count_;

    void Awake()
    {
        // ===== EventCenterDictionary (旧版 Dictionary 实现) =====
        this.Subscribe(EventCenterDictionary.EventName.TestEvent, OnDictTestEvent);
        this.Subscribe(EventCenterDictionary.EventName.TestEvent_i, OnDictTestEvent_i);
        this.Subscribe(EventCenterDictionary.EventName.TestEvent_ii, OnDictTestEvent_ii);
        this.Subscribe(EventCenterDictionary.EventName.TestEvent_iii, OnDictTestEvent_iii);
        this.Subscribe(EventCenterDictionary.EventName.TestEvent_s, OnDictTestEvent_s);
        this.Subscribe(EventCenterDictionary.EventName.TestEvent_ss, OnDictTestEvent_ss);

        // ===== EventCenterArray (高性能数组版) =====
        this.Subscribe(EventCenterArray.EventName.TestEvent, OnArrayTestEvent);
        this.Subscribe(EventCenterArray.EventName.TestEvent_i, OnArrayTestEvent_i);
        this.Subscribe(EventCenterArray.EventName.TestEvent_ii, OnArrayTestEvent_ii);
        this.Subscribe(EventCenterArray.EventName.TestEvent_iii, OnArrayTestEvent_iii);
        this.Subscribe(EventCenterArray.EventName.TestEvent_s, OnArrayTestEvent_s);
        this.Subscribe(EventCenterArray.EventName.TestEvent_ss, OnArrayTestEvent_ss);

        // ===== EventBus (泛型静态类版) =====
        EventBus<TestEvent>.OnEvent += OnBusTestEvent;
        EventBus<TestEvent_i, int>.OnEvent += OnBusTestEvent_i;
        EventBus<TestEvent_ii, int, int>.OnEvent += OnBusTestEvent_ii;
        EventBus<TestEvent_iii, int, int, int>.OnEvent += OnBusTestEvent_iii;
        EventBus<TestEvent_s, string>.OnEvent += OnBusTestEvent_s;
        EventBus<TestEvent_ss, string, string>.OnEvent += OnBusTestEvent_ss;

        // ===== EventBusR (带返回值版) =====
        EventBusR<TestEventR, int>.OnEvent += OnBusTestEventR;
        EventBusR<TestEventR_i, int, int>.OnEvent += OnBusTestEventR_i;
    }

    // ========== EventCenterDictionary 回调（仅计数 + 微小计算） ==========
    private void OnDictTestEvent()                     { dict_testEvent_count_++;   int _ = 1 + 1; }
    private void OnDictTestEvent_i(int i)              { dict_testEvent_i_count_++; int _ = i + 1; }
    private void OnDictTestEvent_ii(int i_1, int i_2)    { dict_testEvent_ii_count_++; int _ = i_1 + i_2; }
    private void OnDictTestEvent_iii(int i_1, int i_2, int i_3) { dict_testEvent_iii_count_++; int _ = i_1 + i_2 + i_3; }
    private void OnDictTestEvent_s(string s)           { dict_testEvent_s_count_++;  int _ = s.Length; }
    private void OnDictTestEvent_ss(string s_1, string s_2) { dict_testEvent_ss_count_++; int _ = s_1.Length + s_2.Length; }

    // ========== EventCenterArray 回调（仅计数 + 微小计算） ==========
    private void OnArrayTestEvent()                     { array_testEvent_count_++;   int _ = 1 + 1; }
    private void OnArrayTestEvent_i(int i)              { array_testEvent_i_count_++; int _ = i + 1; }
    private void OnArrayTestEvent_ii(int i_1, int i_2)    { array_testEvent_ii_count_++; int _ = i_1 + i_2; }
    private void OnArrayTestEvent_iii(int i_1, int i_2, int i_3) { array_testEvent_iii_count_++; int _ = i_1 + i_2 + i_3; }
    private void OnArrayTestEvent_s(string s)           { array_testEvent_s_count_++;  int _ = s.Length; }
    private void OnArrayTestEvent_ss(string s_1, string s_2) { array_testEvent_ss_count_++; int _ = s_1.Length + s_2.Length; }

    // ========== EventBus 回调（仅计数 + 微小计算） ==========
    private void OnBusTestEvent()                     { bus_testEvent_count_++;   int _ = 1 + 1; }
    private void OnBusTestEvent_i(int i)              { bus_testEvent_i_count_++; int _ = i + 1; }
    private void OnBusTestEvent_ii(int i_1, int i_2)    { bus_testEvent_ii_count_++; int _ = i_1 + i_2; }
    private void OnBusTestEvent_iii(int i_1, int i_2, int i_3) { bus_testEvent_iii_count_++; int _ = i_1 + i_2 + i_3; }
    private void OnBusTestEvent_s(string s)           { bus_testEvent_s_count_++;  int _ = s.Length; }
    private void OnBusTestEvent_ss(string s_1, string s_2) { bus_testEvent_ss_count_++; int _ = s_1.Length + s_2.Length; }

    // ========== EventBusR 回调（带返回值，仅计数） ==========
    private int OnBusTestEventR()     { bus_testEventR_count_++;  return 42; }
    private int OnBusTestEventR_i(int i) { bus_testEventR_i_count_++; return i * 10; }
}