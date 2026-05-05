using System;
using System.Collections.Generic;

namespace EventCenter
{
    public static class EventCenter
    {
        private static readonly Dictionary<object, Delegate> events_ = new();

        public sealed class EventKey { }
        public sealed class EventKey<T> { }
        public sealed class EventKey<T1, T2> { }
        public sealed class EventKey<T1, T2, T3> { }


        #region 订阅事件
        public static void Subscribe(this object _, EventKey event_key, Action handler)
        {
            AddHandler(event_key, handler);
        }
        public static void Subscribe<T>(this object _, EventKey<T> event_key, Action<T> handle)
        {
            AddHandler(event_key, handle);
        }
        public static void Subscribe<T1, T2>(this object _, EventKey<T1, T2> event_key, Action<T1, T2> handle)
        {
            AddHandler(event_key, handle);
        }
        public static void Subscribe<T1, T2, T3>(this object _, EventKey<T1, T2, T3> event_key, Action<T1, T2, T3> handle)
        {
            AddHandler(event_key, handle);
        }
        #endregion

        #region 取消订阅
        public static void Unsubscribe(this object _, EventKey event_key, Action handler)
        {
            RemoveHandler(event_key, handler);
        }
        public static void Unsubscribe<T>(this object _, EventKey<T> event_key, Action<T> handler)
        {
            RemoveHandler(event_key, handler);
        }
        public static void Unsubscribe<T1, T2>(this object _, EventKey<T1, T2> event_key, Action<T1, T2> handler)
        {
            RemoveHandler(event_key, handler);
        }
        public static void Unsubscribe<T1, T2, T3>(this object _, EventKey<T1, T2, T3> event_key, Action<T1, T2, T3> handler)
        {
            RemoveHandler(event_key, handler);
        }
        #endregion

        #region 触发事件
        public static void Publish(this object _, EventKey event_key)
        {
            if (events_.TryGetValue(event_key, out Delegate del))
            {
                Action cb = del as Action;
                cb?.Invoke();
            }
        }
        public static void Publish<T>(this object _, EventKey<T> event_key, T arg)
        {
            if (events_.TryGetValue(event_key, out Delegate del))
            {
                Action<T> cb = del as Action<T>;
                cb?.Invoke(arg);
            }
        }
        public static void Publish<T1, T2>(this object _, EventKey<T1, T2> event_key, T1 arg1, T2 arg2)
        {
            if (events_.TryGetValue(event_key, out Delegate del))
            {
                Action<T1, T2> cb = del as Action<T1, T2>;
                cb?.Invoke(arg1, arg2);
            }
        }
        public static void Publish<T1, T2, T3>(this object _, EventKey<T1, T2, T3> event_key, T1 arg1, T2 arg2, T3 arg3)
        {
            if (events_.TryGetValue(event_key, out Delegate del))
            {
                Action<T1, T2, T3> cb = del as Action<T1, T2, T3>;
                cb?.Invoke(arg1, arg2, arg3);
            }
        }
        #endregion

        #region 辅助方法
        private static void AddHandler(object event_key, Delegate handler)
        {
            if (!events_.ContainsKey(event_key))
            {
                events_.Add(event_key, handler);
            }
            else
            {
                events_[event_key] = Delegate.Combine(events_[event_key], handler);
            }
        }
        private static void RemoveHandler(object event_key, Delegate handler)
        {
            if (events_.ContainsKey(event_key))
            {
                events_[event_key] = Delegate.Remove(events_[event_key], handler);
                if (events_[event_key] == null)
                {
                    events_.Remove(event_key);
                }
            }
        }
        #endregion
    }
}