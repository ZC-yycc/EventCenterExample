# EventCenterExample — Unity 事件中心示例项目

一个用于演示和对比 **三种 C# 事件系统** 实现方案的 Unity 项目，帮助你在不同场景下选择最合适的事件通信方式。

---

## 项目概览

| 事件系统 | 目录 | 实现方式 | 特点 |
|---------|------|---------|------|
| **EventCenter** | `Assets/EventCenterOld/` | `Dictionary<object, Delegate>` | 经典字典方案，类型安全，便于扩展 |
| **EventCenterHighPerformance** | `Assets/EventCenter/` | 预分配 `Delegate[]` + `Unsafe.As` | **极致性能**，零 GC，适合高频调用 |
| **EventBus** | `Assets/EventBus/` | 泛型静态类 + C# `event` | 编译期类型安全，API 最简洁 |

> 项目运行后会在 Console 中输出三种方案的耗时对比，直观展示性能差异。

---

## 快速开始

1. 使用 **Unity 6**（或更高版本）打开项目
2. 打开 `Assets/Scenes/SampleScene.unity`
3. 点击 Play 运行
4. 在 Console 窗口查看日志输出和性能数据

---

## 三种事件系统详解

### 1. EventCenter（经典字典方案）

基于 `Dictionary<object, Delegate>` 实现的事件中心，使用 `EventKey` 对象作为事件标识。

**优点：**
- 使用简单直观，`Subscribe` / `Unsubscribe` / `Publish` 作为扩展方法
- 类型安全的发布-订阅语义

**缺点：**
- 字典查找 + `is` 类型检查存在额外开销
- 每次 `Publish` 涉及装箱/类型转换

**示例：**
```csharp
// 定义事件 key
public static readonly EventKey TestEvent = new EventKey();
public static readonly EventKey<int> TestEvent_i = new EventKey<int>();

// 订阅
this.Subscribe(EventName.TestEvent, OnTestEvent);
this.Subscribe(EventName.TestEvent_i, OnTestEvent_i);

// 发布
this.Publish(EventName.TestEvent);
this.Publish(EventName.TestEvent_i, 42);
```

---

### 2. EventCenterHighPerformance（高性能方案）

通过 **预分配大数组** + **整数索引** + **`Unsafe.As` 零开销类型转换** 实现极致性能。

**核心设计：**
- 所有事件注册在固定大小的 `Delegate[]` 数组中，O(1) 直接寻址
- 使用 `Unsafe.As<>` 绕过 C# 类型安全检查，将 `Delegate` 零开销转换为具体 `Action<T>`
- 事件 key 在构造时自动分配全局唯一整数 ID

**优点：**
- 极致的派发性能，几乎无 GC 压力
- 无字典查找、无类型检查、无装箱

**注意：**
- `Unsafe.As<>` 绕过了运行时类型检查，调用者需要保证 Subscribe 和 Publish 的类型一致
- 事件数量受 `MAX_EVENTS` 限制（默认 1024）

**示例：**
```csharp
// 定义事件 key（自动分配整数 ID）
public static readonly EventKey TestEvent = new EventKey();
public static readonly EventKey<int> TestEvent_i = new EventKey<int>();

// 订阅和发布的 API 与 EventCenter 一致
this.Subscribe(EventHighPerformance.EventName.TestEvent, OnTestEvent);
this.Publish(EventHighPerformance.EventName.TestEvent);
```

---

### 3. EventBus（泛型静态类方案）

利用 C# 原生 `event` 关键字和泛型静态类，将 **事件类型本身作为标识**。

**核心设计：**
- 标记接口 `IEventBus` / `IEventBus<T1>` / `IEventBus<T1, T2>` / `IEventBus<T1, T2, T3>` 约束参数数量
- 每个事件定义一个空 `struct` 并实现对应接口，作为泛型类型参数
- 直接通过 `EventBus<YourEvent>.OnEvent += handler` 订阅

**优点：**
- API 极简，编译期完全类型安全
- 不需要额外的 EventKey 定义
- 使用 struct 作为事件类型，零分配

**缺点：**
- 每个事件类型生成一个静态类，订阅关系分散在各静态字段中
- 难以做全局事件监控/调试

**示例：**
```csharp
// 1. 定义事件类型（空 struct + 标记接口）
public struct TestEvent : IEventBus { }
public struct TestEvent_i : IEventBus<int> { }

// 2. 订阅
EventBus<TestEvent>.OnEvent += OnTestEvent;
EventBus<TestEvent_i, int>.OnEvent += OnTestEvent_i;

// 3. 发布
EventBus<TestEvent>.Publish();
EventBus<TestEvent_i, int>.Publish(42);

// 4. 取消订阅
EventBus<TestEvent>.OnEvent -= OnTestEvent;
```

---

## 项目文件结构

```
Assets/
├── EventBus/                        # EventBus 泛型静态类方案
│   ├── EventBus.cs                  # 核心实现（0~3 参数版本）
│   └── Events/
│       └── TestEvent.cs             # 测试用事件类型定义
├── EventCenter/                     # 高性能 EventCenter
│   ├── EventCenter.cs               # 核心实现（数组 + Unsafe.As）
│   └── EventName.cs                 # 事件 key 定义
├── EventCenterOld/                  # 经典 EventCenter
│   └── EventCenter/
│       ├── EventCenter.cs           # 核心实现（Dictionary 方案）
│       └── EventName.cs             # 事件 key 定义
├── Signal.cs                        # 事件发布者（含性能计时）
├── Slot.cs                          # 事件订阅者
└── Scenes/                          # Unity 场景
```

---

## 选型建议

| 场景 | 推荐方案 |
|------|---------|
| 游戏玩法事件（战斗、AI、UI），需极致性能 | **EventCenterHighPerformance** |
| 通用项目，追求简洁与安全 | **EventBus** |
| 需要动态事件名、全局管理、调试支持 | **EventCenter**（经典方案） |
| 原型快速开发 | **EventBus**（API 最简） |

---

## 环境要求

- Unity 6.0+
- .NET Standard 2.1+
- C# 9.0+

---

## 许可证

可随意参考、学习和使用。