# EventCenterExample — Unity 事件中心示例项目

一个用于演示和对比 **三种 C# 事件系统** 实现方案的 Unity 项目，帮助你在不同场景下选择最合适的事件通信方式。

---

## 项目概览

| 事件系统 | 目录 | 命名空间 | 实现方式 | 调用耗时 | 特点 |
|---------|------|---------|------|------|------|
| **EventBus** | `Assets/EventBus/` | `EventBus` | 泛型静态类 + C# `event` | **~0.1ms** | 🏆 性能最佳，编译期类型安全，API 最简洁 |
| **EventCenterArray** | `Assets/EventCenter/` | `EventCenterArray` | 预分配 `Delegate[]` + `Unsafe.As` | **~0.15ms** | 极低 GC，适合高频调用 |
| **EventCenterDictionary** | `Assets/EventCenterOld/` | `EventCenterDictionary` | `Dictionary<object, Delegate>` | **~0.5ms** | 经典字典方案，类型安全，便于调试 |

> 项目运行后会在 Console 中输出三种方案的耗时对比，直观展示性能差异。以上数据为四次 Publish 调用的总耗时。

---

## 快速开始

1. 使用 **Unity 6**（或更高版本）打开项目
2. 打开 `Assets/Scenes/SampleScene.unity`
3. 点击 Play 运行
4. 在 Console 窗口查看日志输出和性能数据

---

## 三种事件系统详解

### 1. EventBus（泛型静态类方案）🏆 性能最佳

利用 C# 原生 `event` 关键字和泛型静态类，将 **事件类型本身作为标识**。实测四次 Publish 调用总耗时仅 **~0.1ms**，是三种方案中性能最好的。

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

// 订阅
EventBus<TestEvent>.OnEvent += OnTestEvent;
EventBus<TestEvent_i, int>.OnEvent += OnTestEvent_i;

// 发布
EventBus<TestEvent>.Publish();
EventBus<TestEvent_i, int>.Publish(42);

// 取消订阅
EventBus<TestEvent>.OnEvent -= OnTestEvent;
EventBus<TestEvent_i, int>.OnEvent -= OnTestEvent_i;
```

---

### 2. EventCenterArray（数组方案）

通过 **预分配大数组** + **整数索引** + **`Unsafe.As` 零开销类型转换** 实现高性能，实测耗时约 **~0.15ms**。

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

### 3. EventCenterDictionary（字典方案）

基于 `Dictionary<object, Delegate>` 实现的事件中心，使用 `EventKey` 对象作为事件标识，实测耗时约 **~0.5ms** — 最慢但最易调试。

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

## 项目文件结构

```
Assets/
├── EventBus/                           # EventBus 泛型静态类方案
│   ├── EventBus.cs                     # 核心实现（支持 0~3 参数）
│   └── Events/
│       └── TestEvent.cs                # 测试用事件类型定义
├── EventCenter/                        # EventCenterArray 高性能方案
│   ├── EventCenter.cs                  # 核心实现（数组 + Unsafe.As）
│   └── EventName.cs                    # 事件 key 定义
├── EventCenterOld/                     # EventCenterDictionary 经典方案
│   └── EventCenter/
│       ├── EventCenter.cs              # 核心实现（字典方案）
│       └── EventName.cs                # 事件 key 定义
├── Signal.cs                           # 事件发布者（发布三种系统的事件并输出耗时）
├── Slot.cs                             # 事件订阅者（同时订阅三种系统的事件）
└── Scenes/                             # Unity 场景
```

### 关键文件说明

| 文件 | 角色 | 职责 |
|------|------|------|
| `Signal.cs` | 发布者 | 在 `Start()` 中依次通过三种系统发布事件，并输出各自耗时 |
| `Slot.cs` | 订阅者 | 在 `Awake()` 中同时向三种系统注册回调，收到事件时打印日志 |

---

## 选型建议

| 场景 | 推荐方案 |
|------|---------|
| 游戏核心玩法事件（战斗、AI、物理），对性能极度敏感 | **EventBus**（🏆 性能最佳，~0.1ms） |
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