namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyType
{
    public sealed class Pepito<T> { }
    internal record SType<K, T> { }

    //[Enforced]
    [Fact]
    public static void Test()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(SType<int, Pepito<string>>);

        options = EasyTypeOptions.Empty; name = source.EasyName(options); Assert.NotNull(name);
        options = EasyTypeOptions.Default; name = source.EasyName(options); Assert.NotNull(name);
        options = EasyTypeOptions.Full; name = source.EasyName(options); Assert.NotNull(name);
    }
}