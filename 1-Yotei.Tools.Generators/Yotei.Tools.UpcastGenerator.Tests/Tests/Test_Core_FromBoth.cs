#define GENERATE

namespace Yotei.Tools.UpcastGenerator.Tests.Core_FromBoth;

// ========================================================
//[Enforced]
public static partial class Test
{
    public interface IFoo<K, T>
    {
        int Count { get; }
        IFoo<K, T>? Legend { get; }
        IFoo<K, T>? this[int index] { get; }
        IFoo<K, T>? Add(T item);
    }

#if !GENERATE
    partial interface IBar<K>
    {
        new IBar<K>? Legend { get; }
        new IBar<K>? this[int index] { get; }
        new IBar<K>? Add(string item);
    }
#else
    [Upcast(ChangeProperties = true)]
#endif
    public partial interface IBar<K> : IFoo<K, string> { }

    // ====================================================

    public class Foo<K, T> : IFoo<K, T>
    {
        public int Count => 0;

        public Foo<K, T>? Legend { get => this; set { } }
        IFoo<K, T>? IFoo<K, T>.Legend => Legend;

        public Foo<K, T>? this[int index] { get => this; set { } }
        IFoo<K, T>? IFoo<K, T>.this[int index] => this[index];

        public virtual Foo<K, T>? Add(T item) => this;
        IFoo<K, T>? IFoo<K, T>.Add(T item) => Add(item);
    }

#if !GENERATE
    partial class Bar<K>
    {
        // Elements straight from the base Foo class...
        public new Bar<K>? Legend
        {
            get => (Bar<K>?)base.Legend;
            set => base.Legend = value;
        }

        public new Bar<K>? this[int index]
        {
            get => (Bar<K>?)base[index];
            set => base[index] = value;
        }

        public override Bar<K>? Add(string item) => (Bar<K>?)base.Add(item);

        // IBar<K> appears in the inheritance chain...
        IBar<K>? IBar<K>.Legend => Legend;
        IBar<K>? IBar<K>.this[int index] => this[index];
        IBar<K>? IBar<K>.Add(string item) => Add(item);
    }
#else
    [Upcast(ChangeProperties = true)]
#endif
    public partial class Bar<K> : Foo<K, string>, IBar<K> { }

    // ====================================================

    //[Enforced]
    [Fact]
    public static void Test_Execute()
    {
        var bar = new Bar<int>();
        Assert.Same(bar, bar.Legend);
        Assert.Same(bar, bar[0]);
        Assert.Same(bar, bar.Add("any"));

        var ibar = (IBar<int>)bar;
        ibar.Add("any");
    }
}