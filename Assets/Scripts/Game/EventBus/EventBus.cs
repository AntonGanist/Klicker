using System;
using System.Collections.Generic;
using System.Linq;

public static class EventBus
{
    private static readonly Dictionary<Type, List<WeakReference<IBaseEventReceiver>>> _receivers
        = new Dictionary<Type, List<WeakReference<IBaseEventReceiver>>>();
    private static readonly Dictionary<string, WeakReference<IBaseEventReceiver>> _receiverHashToReference
            = new Dictionary<string, WeakReference<IBaseEventReceiver>>();

    public static void Register<T>(IEventReceiver<T> receiver) where T : struct, IEvent
    {
        Type eventType = typeof(T);
        if (!_receivers.ContainsKey(eventType))
            _receivers[eventType] = new List<WeakReference<IBaseEventReceiver>>();
        if (!_receiverHashToReference.TryGetValue(receiver.Id, out WeakReference<IBaseEventReceiver> reference))
        {
            reference = new WeakReference<IBaseEventReceiver>(receiver);
            _receiverHashToReference[receiver.Id] = reference;
        }
        _receivers[eventType].Add(reference);
    }

    public static void Unregister<T>(IEventReceiver<T> receiver) where T : struct, IEvent
    {
        Type eventType = typeof(T);
        if (!_receivers.ContainsKey(eventType) || !_receiverHashToReference.ContainsKey(receiver.Id))
            return;
        WeakReference<IBaseEventReceiver> reference = _receiverHashToReference[receiver.Id];
        _receivers[eventType].Remove(reference);
        int weakRefCount = _receivers.SelectMany(x => x.Value).Count(x => x == reference);
        if (weakRefCount == 0)
            _receiverHashToReference.Remove(receiver.Id);
    }

    public static void Raise<T>(T @event) where T : struct, IEvent
    {
        Type eventType = typeof(T);
        if (!_receivers.ContainsKey(eventType))
            return;
        List<WeakReference<IBaseEventReceiver>> references = _receivers[eventType];
        for (int i = references.Count - 1; i >= 0; i--)
        {
            if (references[i].TryGetTarget(out IBaseEventReceiver receiver))
                ((IEventReceiver<T>)receiver).OnEvent(@event);
        }
    }
}
