namespace Experimental.WeakEvents;

// ========================================================
public sealed class WeakEventSource
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