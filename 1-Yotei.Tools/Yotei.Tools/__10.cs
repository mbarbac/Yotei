namespace Yotei.Tools;

// ========================================================
public class MyFakeType
{
    public static void MyMethod()
    {
        var span = "hello".AsSpan();

        span.Remove('c');
        span.Remove('c', false);
        span.Remove('c', char.CharComparer());
        span.Remove('c', StringComparer.Ordinal);
        span.Remove('c', StringComparison.Ordinal);

        span.Remove("s");
        span.Remove("s", false);
        span.Remove("s", char.CharComparer());
        span.Remove("s", StringComparer.Ordinal);
        span.Remove("s", StringComparison.Ordinal);
    }
}