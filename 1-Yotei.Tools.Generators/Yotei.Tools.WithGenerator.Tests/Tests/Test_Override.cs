using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.WithGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_Override
{
    public interface IValueA { }

    public partial interface ITypeA
    {
        [With] IValueA Value { get; }
    }

    //public partial class TypeA : ITypeA
    //{
    //    public TypeA(IValueA value) => Value = value;
    //    protected TypeA(TypeA source) => Value = source.Value;
    //    public IValueA Value { get; set; }
    //}

    public interface IValueB : IValueA { }

    [InheritWiths]
    public partial interface ITypeB : ITypeA
    {
        [With] new IValueB Value { get; }
    }

    //public partial class TypeB : TypeA, ITypeB
    //{
    //    public TypeB(IValueB value) : base(value) { }
    //    protected TypeB(TypeB source) : base(source) { }
    //    public new IValueB Value
    //    {
    //        get => (IValueB)base.Value;
    //        set => base.Value = value;
    //    }
    //}


    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_()
    //{
    //}
}