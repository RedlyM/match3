namespace MatchThree.Utils
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public interface IEventBus
    {
        void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class;
        void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : class;
        void Publish<TEvent>(TEvent evt) where TEvent : class;
    }

    public sealed class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new();
        private readonly object _lock = new();

        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            lock (_lock)
            {
                var t = typeof(TEvent);
                if (!_subscribers.TryGetValue(t, out var list))
                {
                    list = new List<Delegate>();
                    _subscribers[t] = list;
                }
                list.Add(handler);
            }
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : class
        {
            if (handler == null) return;
            lock (_lock)
            {
                var t = typeof(TEvent);
                if (_subscribers.TryGetValue(t, out var list))
                {
                    list.Remove(handler);
                    if (list.Count == 0) _subscribers.Remove(t);
                }
            }
        }

        public void Publish<TEvent>(TEvent evt) where TEvent : class
        {
            if (evt == null) return;
            List<Delegate> copy = null;
            lock (_lock)
            {
                if (_subscribers.TryGetValue(typeof(TEvent), out var list))
                    copy = new List<Delegate>(list);
            }
            if (copy == null) return;
            foreach (var d in copy)
            {
                try { ((Action<TEvent>)d).Invoke(evt); }
                catch (Exception ex) { Debug.LogException(ex); }
            }
        }
    }

}