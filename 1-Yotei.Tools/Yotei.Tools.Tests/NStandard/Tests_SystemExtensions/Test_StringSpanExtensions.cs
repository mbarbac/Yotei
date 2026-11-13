namespace Yotei.Tools.Tests;

// ========================================================
public static partial class Test_StringSpanExtensions
{
    [SuppressMessage("", "IDE0057")]
    [Fact]
    public static void Inherit_Remove_FromIndex()
    {
        string target;
        var source = "abcd";
        
        target = source.Remove(0); Assert.Empty(target);
        target = source.Remove(1); Assert.Equal("a", target);
        target = source.Remove(2); Assert.Equal("ab", target);
        target = source.Remove(3); Assert.Equal("abc", target);

#if !NETSTANDARD2_0
        target = source.Remove(4); Assert.Equal("abcd", target);
#else
        try { target = source.Remove(4); Assert.Fail(""); } catch (ArgumentOutOfRangeException) { }
#endif

        try { target = source.Remove(-1); Assert.Fail(""); } catch (ArgumentOutOfRangeException) { }
        try { target = source.Remove(5); Assert.Fail(""); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    [SuppressMessage("", "IDE0057")]
    [Fact]
    public static void Inherit_Remove_Range()
    {
        string target;
        var source = "abcd";

        target = source.Remove(0, 0); Assert.Equal("abcd", target);
        target = source.Remove(0, 2); Assert.Equal("cd", target);
        target = source.Remove(0, 4); Assert.Empty(target);

        target = source.Remove(1, 0); Assert.Equal("abcd", target);
        target = source.Remove(1, 2); Assert.Equal("ad", target);
        target = source.Remove(1, 3); Assert.Equal("a", target);

        try { target = source.Remove(-1, 0); Assert.Fail(""); } catch (ArgumentOutOfRangeException) { }
        try { target = source.Remove(0, -1); Assert.Fail(""); } catch (ArgumentOutOfRangeException) { }
        try { target = source.Remove(1, 4); Assert.Fail(""); } catch (ArgumentOutOfRangeException) { }
    }
}