namespace Experimental.Tests;

// ========================================================
//[Enforced]
public class QuickAndDirty
{
    //[Enforced]
    [Fact]
    public void Test()
    {
    }

    public static bool Example(
        ReadOnlySpan<char> source, int ini, char head,
        out ReadOnlySpan<char> span)
    {
        source = source.Slice(ini);
        if (source.StartsWith(head))
        {
            span = source.Slice(head);
            return true;
        }

        span = ReadOnlySpan<char>.Empty;
        return false;
    }
}