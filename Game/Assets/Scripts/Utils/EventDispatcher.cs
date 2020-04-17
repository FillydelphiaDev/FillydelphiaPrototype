using System;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace Utils
{
    public class EventDispatcher : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(EventDispatcher));

        // Can be null
        private readonly ILog instanceLog;

        private readonly IDictionary<Type, OrderedListenerList> listeners;

        public EventDispatcher() : this(null)
        {
        }

        public EventDispatcher(ILog instanceLogger)
        {
            instanceLog = instanceLogger;
            listeners = new Dictionary<Type, OrderedListenerList>();
        }

        /// <summary>
        /// Register a new event listener with a given priority (small - first).
        /// </summary>
        /// <exception cref="ArgumentException">If listener is already registered.</exception>
        public void AddListener<TEvent>(int order, Func<TEvent, TEvent> listener)
            where TEvent : struct
        {
            if (!listeners.TryGetValue(typeof(TEvent),
                out OrderedListenerList eventListeners))
            {
                eventListeners = new OrderedListenerList();
                listeners[typeof(TEvent)] = eventListeners;
            }

            try
            {
                eventListeners.Add(order, listener);
                instanceLog?.Debug()
                    ?.Call($"Added listener {ListenerToString(listener)} for {typeof(TEvent)}");
            }
            catch (ArgumentException)
            {
                instanceLog?.Debug()?.Call(
                    $"Tried to add listener {ListenerToString(listener)} " +
                    $"for {typeof(TEvent)} but it's already registered");
                throw;
            }
        }

        /// <summary>
        /// Remove a registered listener.
        /// </summary>
        /// <param name="listener">Listener to remove.</param>
        /// <exception cref="ArgumentException">If listener is not registered.</exception>
        public void RemoveModifier<TEvent>(Func<TEvent, TEvent> listener) where TEvent : struct
        {
            listeners.TryGetValue(typeof(TEvent), out OrderedListenerList eventListeners);

            try
            {
                if (eventListeners == null)
                {
                    throw new ArgumentException("Event doesn't have any listeners");
                }
                eventListeners.Remove(listener);
                instanceLog?.Debug()
                    ?.Call($"Removed listener {ListenerToString(listener)} for {typeof(TEvent)}");
            }
            catch (ArgumentException)
            {
                instanceLog?.Debug()?.Call(
                    $"Tried to remove listener {ListenerToString(listener)} " +
                    $"for {typeof(TEvent)} but there's no such listener");
                throw;
            }

            // Clear from dict if all listeners were removed
            if (eventListeners.Count == 0)
            {
                listeners.Remove(typeof(TEvent));
                Log.Debug()?.Call($"{typeof(TEvent)} removed from dict: it had no listeners");
            }
        }

        public void Dispatch<TEvent>(ref TEvent @event) where TEvent : struct
        {
            if (listeners.TryGetValue(typeof(TEvent), out OrderedListenerList eventListeners))
            {
                eventListeners.Dispatch(ref @event);
                instanceLog?.Debug()
                    ?.Call(
                        $"Event {typeof(TEvent)} dispatched to {eventListeners.Count} listeners");
            }
        }

        /// <summary>
        /// Get listeners of an event.
        /// </summary>
        /// <returns>
        /// List with event listeners in order.
        /// Empty if trait doesn't have any listeners.
        /// </returns>
        public List<(int order, Func<TEvent, TEvent> listener)> GetEventListeners<TEvent>()
            where TEvent : struct
        {
            if (listeners.TryGetValue(typeof(TEvent), out OrderedListenerList eventListeners))
            {
                return eventListeners.CopyList<TEvent>();
            }
            return new List<(int, Func<TEvent, TEvent>)>();
        }

        /// <summary>
        /// Get all event listeners.
        /// </summary>
        /// <returns>A dict of event types and lists of their listeners in order.</returns>
        public Dictionary<Type, List<(int order, Delegate listener)>> GetAllListeners()
        {
            Dictionary<Type, List<(int, Delegate)>> result =
                new Dictionary<Type, List<(int, Delegate)>>(listeners.Count);
            foreach (KeyValuePair<Type, OrderedListenerList> eventEntry in listeners)
            {
                if (eventEntry.Value.Count > 0)
                {
                    result[eventEntry.Key] = eventEntry.Value.CopyList();
                }
            }
            return result;
        }

        /// <summary>
        /// Is there any active event listeners.
        /// </summary>
        public bool IsEmpty()
        {
            return listeners.Count == 0;
        }

        public void Dispose()
        {
            // Send warning if someone forgot to unregister listener
            ILog log = instanceLog ?? Log;
            if (log.IsWarnEnabled && !IsEmpty())
            {
                Dictionary<Type, List<(int order, Delegate listener)>> leftListeners =
                    GetAllListeners();
                foreach (KeyValuePair<Type, List<(int order, Delegate listener)>> eventEntry in
                    leftListeners)
                {
                    string eventListeners = string.Join(", ",
                        eventEntry.Value.Select(l => ListenerToString(l.listener)));
                    log.Warn($"Event {eventEntry.Key} has active listeners " +
                             $"upon dispatcher finalization: {eventListeners}");
                }
            }
        }

        /// <summary>
        /// Get listener as string for debug.
        /// Global methods: global#MethodName
        /// Static methods: Namespace.ClassType#MethodName
        /// Instance methods: Namespace.ClassType#MethodName@InstanceHashCode
        /// </summary>
        private static string ListenerToString(Delegate listener)
        {
            string result = "";
            Type classType = listener.Method.DeclaringType;
            result += classType != null ? classType.FullName + "#" : "global#";
            result += listener.Method.Name;
            if (listener.Target != null)
            {
                result += "@" + listener.Target.GetHashCode();
            }
            return result;
        }

        /// <summary>
        /// List wrapper util to keep listeners ordered.
        /// </summary>
        private class OrderedListenerList
        {
            private static readonly Comparison<(int order, Delegate listener)> OrderComparison =
                (a, b) => a.order.CompareTo(b.order);

            private readonly List<(int order, Delegate listener)> list;

            public int Count => list.Count;

            public OrderedListenerList()
            {
                list = new List<(int, Delegate)>();
            }

            public void Add(int order, Delegate listener)
            {
                if (Contains(listener))
                {
                    throw new ArgumentException("Listener is already registered");
                }
                list.Add((order, listener));
                list.Sort(OrderComparison);
            }

            public void Remove(Delegate listener)
            {
                // [Optimization Target] O(n)
                for (int i = 0; i < list.Count; i++)
                {
                    // Delegate overrides ==
                    if (listener == list[i].listener)
                    {
                        list.RemoveAt(i);
                        return;
                    }
                }
                throw new ArgumentException("Listener is not registered");
            }

            public bool Contains(Delegate listener)
            {
                // [Optimization Target] O(n)
                for (int i = 0; i < list.Count; i++)
                {
                    // Delegate overrides ==
                    if (listener == list[i].listener)
                    {
                        return true;
                    }
                }
                return false;
            }

            public void Dispatch<TEvent>(ref TEvent @event) where TEvent : struct
            {
                // No enumerator allocations. Nice-e.
                for (int i = 0; i < list.Count; i++)
                {
                    // Getting invalid cast exception here will be very embarrassing
                    @event = ((Func<TEvent, TEvent>) list[i].listener).Invoke(@event);
                }
            }

            public List<(int order, Delegate listener)> CopyList()
            {
                List<(int, Delegate)> copy = new List<(int, Delegate)>(list.Count);
                for (int i = 0; i < list.Count; i++)
                {
                    copy.Add(list[i]);
                }
                return copy;
            }

            public List<(int order, Func<TEvent, TEvent> listener)> CopyList<TEvent>()
                where TEvent : struct
            {
                List<(int, Func<TEvent, TEvent>)> copy =
                    new List<(int, Func<TEvent, TEvent>)>(list.Count);
                for (int i = 0; i < list.Count; i++)
                {
                    copy.Add(((int, Func<TEvent, TEvent>)) list[i]);
                }
                return copy;
            }
        }
    }
}
