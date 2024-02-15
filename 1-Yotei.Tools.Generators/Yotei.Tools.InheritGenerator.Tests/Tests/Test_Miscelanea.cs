namespace Yotei.Tools.InheritGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_Miscelanea
{
    public interface IFoo<T>
    {
        int Count { get; }
        IFoo<T> Legend { get; }
        IFoo<T> Add(T item);
    }

    public class Foo<T> : IFoo<T>
    {
        readonly List<T> items = [];

        public int Count => items.Count;
        public virtual Foo<T> Legend
        {
            get => _Legend;
            init => _Legend = value.ThrowWhenNull();
        }
        Foo<T> _Legend = new();
        IFoo<T> IFoo<T>.Legend => Legend;

        public virtual Foo<T> Add(T item)
        {
            items.Add(item);
            return this;
        }
        IFoo<T> IFoo<T>.Add(T item) => Add(item);
    }

    // ====================================================

    [Inherit(typeof(IFoo<>), GenericNames = ["T"])]
    public partial interface IBar<T>
    {
    }

    [Inherit(typeof(IBar<>), GenericNames = ["T"])]
    [Inherit(typeof(Foo<>), GenericNames = ["T"])]
    public partial class Bar<T>
    {
    }

    // ====================================================


    //[Enforced]
    [Fact]
    public static void Test_()
    {
    }
}