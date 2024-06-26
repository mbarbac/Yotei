namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_EasyType
{
    public class Foo<T, K>
    {
        public class Bar
        {
            public class Other<S> { }
        }
    }

    //[Enforced]
    [Fact]
    public static void Test()
    {
        Type type;
        string name;
        var options = EasyTypeOptions.True;

        type = typeof(Foo<,>.Bar.Other<>); name = type.EasyName(options);

        type = typeof(Foo<int, long>); name = type.EasyName(options);
        type = typeof(Foo<int, long>.Bar.Other<byte>); name = type.EasyName(options);
    }
}