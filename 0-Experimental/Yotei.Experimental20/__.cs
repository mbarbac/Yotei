namespace Yotei.Experimental20;

// ========================================================
public class FakeType
{
    static void MyMethod()
    {
        var span = "hello".AsSpan();
        span.StartsWith('x');
    }
}
