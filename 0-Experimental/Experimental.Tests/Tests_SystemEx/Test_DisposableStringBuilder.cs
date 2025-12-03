namespace Experimental.Tests;

// ========================================================
//[Enforced]
public static class Test_DisposableStringBuilder
{
    //[Enforced]
    [Fact]
    public static void Test_1()
    {
        using var builder = new DisposableStringBuilder();
        builder.Append("Hello world!");
    }
}