namespace Experimental;

// ========================================================
public static class Test_StringBuilderExtensions
{
    [Fact]
    public static void Test_Features()
    {
        using (var sb = StringBuilder.Rent("Hello"))
        {
            Assert.False(sb.IsDisposed);

            sb.Append(" world!");
            Assert.Equal("Hello world!", sb.ToString());
        }
    }
}