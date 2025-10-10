namespace Experimental.Tests.WeakEvents;

// ========================================================
//[Enforced]
public static class Test_WeakEvent
{
    public sealed class EventListener
    {
        public void OnCustomEvent(object? sender, EventArgs e)
        {
            Console.WriteLine("Event received.");
        }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test()
    {
        var source = new WeakEventSource();
        var listener = new EventListener();

        source.Suscribe(listener.OnCustomEvent);
        source.TriggerEvent();
        source.Unsubscribe(listener.OnCustomEvent);
    }
}