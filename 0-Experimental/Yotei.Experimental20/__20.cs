namespace Yotei.Experimental20;

// ========================================================
public class FakeType
{
    public static void MyMethod()
    {
        var span = "hello".AsSpan();

        //span.ContainsAny(['c']);
        //span.ContainsAny(['c'], false);
        //span.ContainsAny(['c'], char.CharComparer());
        //span.ContainsAny(['c'], StringComparer.Ordinal);
        //span.ContainsAny(['c'], StringComparison.Ordinal);

        //span.ContainsAny(["s"]);
        //span.ContainsAny(["s"], false);
        //span.ContainsAny(["s"], char.CharComparer());
        //span.ContainsAny(["s"], StringComparer.Ordinal);
        //span.ContainsAny(["s"], StringComparison.Ordinal);
    }
}