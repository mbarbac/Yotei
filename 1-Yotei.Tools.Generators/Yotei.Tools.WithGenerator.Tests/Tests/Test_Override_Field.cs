using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.WithGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_Override_Field
{
    public interface IValueA { }

    public partial class TypeA
    {
        public TypeA(IValueA value) => Value = value;
        protected TypeA(TypeA source) => Value = source.Value;

        [With] public IValueA Value;
    }

    public interface IValueB : IValueA { }

    [InheritWiths]
    public partial class TypeB : TypeA
    {
        public TypeB(IValueB value) : base(value) => Value = value;
        protected TypeB(TypeB source) : base(source) => Value = source.Value;

        [With] public new IValueB Value;
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_TypeA_Has_Methods()
    {
        var type = typeof(TypeA);
        var methods = type.GetMethods().Where(x =>
            x.Name == "WithValue" &&
            x.GetParameters().Length == 1)
            .ToList();

        Assert.Single(methods);
        var method = methods[0];
        Assert.Equal("IValueA", method.GetParameters()[0].ParameterType.Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_TypeB_Has_Methods()
    {
        var type = typeof(TypeB);
        var methods = type.GetMethods().Where(x =>
            x.Name == "WithValue" &&
            x.GetParameters().Length == 1)
            .ToList();

        Assert.Equal(2, methods.Count);
        var method = methods.First(x => x.GetParameters()[0].ParameterType.Name == "IValueA");
        Assert.NotNull(method);
        method = methods.First(x => x.GetParameters()[0].ParameterType.Name == "IValueB");
        Assert.NotNull(method);
    }
}