namespace Experimental.SystemEx;

// =========================================================
//[Enforced]
public static class Test_WeakEvents
{
    //[Enforced]
    [Fact]
    public static void Test()
    {
        var source = new EventSource();
        var listener = new EventListener();

        source.Suscribe(listener.OnCustomEvent);
        source.TriggerEvent();
        source.Unsubscribe(listener.OnCustomEvent);
    }
}