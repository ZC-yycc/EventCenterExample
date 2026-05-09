# EventCenterExample — Unity 事件中心示例项目

一个用于演示和对比 **四种 C# 事件系统** 实现方案的 Unity 项目，帮助你在不同场景下选择最合适的事件通信方式。

---

## 项目概览

| 事件系统 | 目录 | 命名空间 | 实现方式 | 特点 |
|---------|------|---------|------|------|
| **EventBus** | `Assets/EventBus/` | `EventBus` | 泛型静态类 + C# `event` | 🏆 性能最佳，编译期类型安全，API 最简洁 |
| **EventBusR** | `Assets/EventBus/` | `EventBus` | 泛型静态类 + 带返回值委托 | 支持从回调收集返回值 |
| **EventCenterArray** | `Assets/EventCenter/` | `EventCenterArray` | 预分配 `Delegate[]` + `Unsafe.As` | 极低 GC，适合高频调用 |
| **EventCenterDictionary** | `Assets/EventCenterOld/` | `EventCenterDictionary` | `Dictionary<object, Delegate>` | 经典字典方案，类型安全，便于调试 |

> 项目运行后会在 Console 中输出单次调用耗时、10,000 次压力测试对比、带返回值测试结果、以及自动化的调用计数 ✓ PASS / ✗ FAIL 验证。
>
> ### 实测性能数据（10,000 次压力测试，Unity 6，Windows 11）
>
> | 事件系统 | 无参 | 1参(int) | 2参(int,int) | 3参(int,int,int) | **总计** | 单次调用(6种) |
> |---------|------|----------|-------------|-----------------|---------|-------------|
> | **EventBus** | 0.08ms | 0.08ms | 0.08ms | 0.08ms | **0.32ms** 🏆 | 0.47ms |
> | **EventCenterArray** | 0.11ms | 0.10ms | 0.11ms | 0.13ms | **0.45ms** | 0.57ms |
> | **EventCenterDictionary** | 0.50ms | 0.43ms | 0.47ms | 0.45ms | **1.85ms** | 1.92ms |
>
> * EventBus 比 EventCenterDictionary **快 5.8 倍**，比 EventCenterArray 快 1.4 倍

---

## 快速开始

1. 使用 **Unity 6**（或更高版本）打开项目
2. 打开 `Assets/Scenes/SampleScene.unity`
3. 点击 Play 运行
4. 在 Console 窗口查看日志输出和性能数据

---

## 四种事件系统详解

### 1. EventBus（泛型静态类方案）🏆 性能最佳

利用 C# 原生 `event` 关键字和泛型静态类，将 **事件类型本身作为标识**。是三种方案中性能最好的。

**核心设计：**
- 标记接口 `IEventBus` / `IEventBus<T1>` / `IEventBus<T1, T2>` / `IEventBus<T1, T2, T3>` 用于约束参数数量
- 每个事件定义为空 `struct` 并实现对应接口，作为泛型类型参数
- 直接通过 `EventBus<YourEvent>.OnEvent += handler` 订阅

**优点：**
- 编译期完全类型安全
- API 极简，不需要额外的 EventKey 定义
- struct 事件类型，零堆分配
- C# 原生 `event` 机制，运行时可直接优化为直接调用

**缺点：**
- 每个事件类型生成独立静态类，订阅关系分散
- 难以做全局事件监控/统一调试

**示例：**
```csharp
using EventBus;

// 事件类型定义（空 struct + 标记接口）
public struct TestEvent : IEventBus { }
public struct TestEvent_i : IEventBus<int> { }
public struct TestEvent_s : IEventBus<string> { }
public struct TestEvent_ss : IEventBus<string, string> { }

// 订阅
EventBus<TestEvent>.OnEvent += OnTestEvent;
EventBus<TestEvent_i, int>.OnEvent += OnTestEvent_i;
EventBus<TestEvent_s, string>.OnEvent += OnTestEvent_s;
EventBus<TestEvent_ss, string, string>.OnEvent += OnTestEvent_ss;

// 发布
EventBus<TestEvent>.Publish();
EventBus<TestEvent_i, int>.Publish(42);
EventBus<TestEvent_s, string>.Publish("hello");
EventBus<TestEvent_ss, string, string>.Publish("hello", "world");

// 取消订阅
EventBus<TestEvent>.OnEvent -= OnTestEvent;
EventBus<TestEvent_i, int>.OnEvent -= OnTestEvent_i;
```

---

### 2. EventBusR（带返回值事件）

在 EventBus 基础上扩展，支持从回调中收集返回值（最后一个订阅者的返回值）。

**核心设计：**
- 标记接口 `IEventBusR<TReturn>` / `IEventBusR<TReturn, T1>` 约束返回类型和参数
- `Publish()` 返回最后一个订阅者的返回值
- 无订阅者时返回 `default(TReturn)`

**示例：**
```csharp
using EventBus;

// 事件类型定义
public struct TestEventR : IEventBusR<int> { }
public struct TestEventR_i : IEventBusR<int, int> { }

// 订阅（返回类型必须匹配）
EventBusR<TestEventR, int>.OnEvent += () => 42;
EventBusR<TestEventR_i, int, int>.OnEvent += (i) => i * 10;

// 发布并获取返回值
int result = EventBusR<TestEventR, int>.Publish();       // → 42
int result2 = EventBusR<TestEventR_i, int, int>.Publish(7); // → 70
```

---

### 3. EventCenterArray（数组方案）

通过 **预分配大数组** + **整数索引** + **`Unsafe.As` 零开销类型转换** 实现高性能。

**核心设计：**
- 所有事件注册在固定大小的 `Delegate[]` 数组中（默认 1024），O(1) 直接寻址
- `EventKey` 构造时自增分配全局唯一整数 ID，作为数组索引
- `Unsafe.As<>` 将 `Delegate` 零开销转换为具体 `Action<T>`，绕过 C# 类型检查

**优点：**
- 优秀的派发性能，几乎无 GC 压力
- 无字典查找、无装箱
- 事件 key 统一注册在数组中，便于集中管理

**注意：**
- `Unsafe.As<>` 绕过了运行时类型检查，**调用者必须保证 Subscribe 和 Publish 的类型参数一致**，否则会触发未定义行为
- 事件数量受 `MAX_EVENTS` 限制（默认 1024），超出会抛出异常

**示例：**
```csharp
using EventCenterArray;

// 订阅
this.Subscribe(EventCenterArray.EventName.TestEvent, OnTestEvent);
this.Subscribe(EventCenterArray.EventName.TestEvent_i, OnTestEvent_i);

// 发布
this.Publish(EventCenterArray.EventName.TestEvent);
this.Publish(EventCenterArray.EventName.TestEvent_i, 42);

// 取消订阅
this.Unsubscribe(EventCenterArray.EventName.TestEvent, OnTestEvent);
```

---

### 4. EventCenterDictionary（字典方案）

基于 `Dictionary<object, Delegate>` 实现的事件中心，使用 `EventKey` 对象作为事件标识 — 最慢但最易调试。

**核心设计：**
- `EventKey` 作为字典键，通过对象引用区分不同事件
- `Subscribe` / `Unsubscribe` / `Publish` 以扩展方法形式提供
- `Publish` 时通过 `as` 进行类型转换

**优点：**
- 实现简单直观，易于理解和扩展
- 类型安全的发布-订阅语义
- 可通过字典遍历实现全局事件调试/监控

**缺点：**
- 字典查找 + `as` 类型检查存在运行时开销，性能最差

**示例：**
```csharp
using EventCenterDictionary;

// 订阅
this.Subscribe(EventCenterDictionary.EventName.TestEvent, OnTestEvent);
this.Subscribe(EventCenterDictionary.EventName.TestEvent_i, OnTestEvent_i);

// 发布
this.Publish(EventCenterDictionary.EventName.TestEvent);
this.Publish(EventCenterDictionary.EventName.TestEvent_i, 42);

// 取消订阅
this.Unsubscribe(EventCenterDictionary.EventName.TestEvent, OnTestEvent);
```

---

## 测试体系

### 测试覆盖的事件类型

| 参数类型 | 无参 | 1参(int) | 2参(int,int) | 3参(int,int,int) | 1参(string) | 2参(string,string) | 带返回值 |
|---------|------|----------|-------------|-----------------|-------------|-------------------|---------|
| EventCenterDictionary | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | — |
| EventCenterArray | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | — |
| EventBus | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | — |
| EventBusR | ✓ | ✓ | — | — | — | — | ✓ |

### 测试流程（`Signal.Start()` 依次执行）

1. **单次调用测试** — 三种事件系统各发布 6 种事件，输出单次调用总耗时
2. **无订阅者安全测试** — 使用动态创建的 EventKey 和空 EventBus 发布，验证不会抛异常
3. **压力测试** — 每种事件循环发布 10,000 次，按参数数量分组计时并汇总对比
4. **EventBusR 带返回值测试** — 验证返回值正确性，输出 ✓ PASS / ✗ FAIL
5. **调用计数验证** — 读取 `Slot` 中静态计数器（`dict_testEvent_count_` 等），自动对比期望值

### 验证机制

`Slot.cs` 中维护了 20 个静态计数器，每个回调仅做计数 + 微小计算（不打印日志）。测试结束后 `Signal.cs` 自动读取所有计数器并与期望值比对，输出类似：

```
  ✓ Dict.TestEvent: 10001 (符合预期)
  ✗ Bus.TestEvent_ii: 实际=10002, 期望=10001 — 不匹配！
```


## 项目文件结构

```
Assets/
├── EventBus/                           # EventBus + EventBusR 泛型静态类方案
│   ├── EventBus.cs                     # 核心实现（支持 0~3 参数 + 带返回值）
│   └── Events/
│       └── TestEvent.cs                # 测试用事件类型定义（6 种普通 + 2 种带返回值）
├── EventCenter/                        # EventCenterArray 高性能方案
│   ├── EventCenter.cs                  # 核心实现（数组 + Unsafe.As）
│   └── EventName.cs                    # 事件 key 定义（6 种参数类型）
├── EventCenterOld/                     # EventCenterDictionary 经典方案
│   └── EventCenter/
│       ├── EventCenter.cs              # 核心实现（字典方案）
│       └── EventName.cs                # 事件 key 定义（6 种参数类型）
├── Signal.cs                           # 事件发布者 — 完整的测试套件
├── Slot.cs                             # 事件订阅者 — 回调计数 + 自动验证
└── Scenes/                             # Unity 场景
```

### 关键文件说明

| 文件 | 角色 | 职责 |
|------|------|------|
| `Signal.cs` | 发布者 + 测试驱动 | 依次执行单次测试、压力测试、EventBusR 测试、自动计数验证 |
| `Slot.cs` | 订阅者 + 验证支撑 | 订阅全部 20 种事件回调，维护静态计数器，回调中仅做微小计算 |
| `TestEvent.cs` | 事件类型定义 | 定义 6 种普通 EventBus 事件 + 2 种带返回值 EventBusR 事件 |

---

## 选型建议

| 场景 | 推荐方案 |
|------|---------|
| 游戏核心玩法事件（战斗、AI、物理），对性能极度敏感 | **EventBus**（🏆 性能最佳） |
| 需要从回调收集返回值（如查询、投票） | **EventBusR** |
| 需要集中管理事件 key，兼顾性能与可控性 | **EventCenterArray** |
| 需要全局事件监控、动态调试、运行时诊断的项目 | **EventCenterDictionary** |
| 原型快速开发、迭代 | **EventBus**（API 最简，零额外定义） |

---

## 环境要求

- Unity 6.0+
- .NET Standard 2.1+
- C# 9.0+

---

## 许可证

本项目为示例项目，可随意参考、学习和使用。