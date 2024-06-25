namespace Experimental.SystemEx;

// =========================================================
public sealed class WeakEvent<TArgs>
{
    readonly List<WeakReference<EventHandler<TArgs>>> Handlers = [];
    readonly ConditionalWeakTable<object, List<object>> KeepAlive = [];

    public void Suscribe(EventHandler<TArgs> handler)
    {
        handler.ThrowWhenNull();
        Handlers.Add(new(handler));

        if (handler.Target != null)
        {
            var delegatelist = KeepAlive.GetOrCreateValue(handler.Target);
            delegatelist.Add(handler);
        }
    }

    public void Unsubscribe(EventHandler<TArgs> handler)
    {
        handler.ThrowWhenNull();

        Handlers.RemoveAll(x =>
            x.TryGetTarget(out var existingHandler) &&
            existingHandler == handler);

        if (handler.Target != null &&
            KeepAlive.TryGetValue(handler.Target, out var delegatelist))
        {
            delegatelist.Remove(handler);
        }
    }

    public void Raise(object sender, TArgs e)
    {
        foreach (var weak in Handlers.ToList())
        {
            if (weak.TryGetTarget(out var handler))
            {
                handler(sender, e);
            }
            else
            {
                Handlers.Remove(weak);
            }
        }
    }
}

// ========================================================
public sealed class EventSource
{
    readonly WeakEvent<EventArgs> CustomEvent = new();

    public void Suscribe(EventHandler<EventArgs> handler)
    {
        CustomEvent.Suscribe(handler);
    }
    public void Unsubscribe(EventHandler<EventArgs> handler)
    {
        CustomEvent.Unsubscribe(handler);
    }
    public void TriggerEvent() => CustomEvent.Raise(this, EventArgs.Empty);
}

public sealed class EventListener
{
    public void OnCustomEvent(object? sender, EventArgs e)
    {
        Console.WriteLine("Event received.");
    }
}