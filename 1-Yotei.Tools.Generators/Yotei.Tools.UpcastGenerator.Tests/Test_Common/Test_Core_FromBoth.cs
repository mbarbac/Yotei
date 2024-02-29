#define GENERATE

namespace Yotei.Tools.UpcastGenerator.Tests.Core_FromBoth;

// ========================================================
public interface IFoo<K, T>
{
    int Count { get; }
    IFoo<K, T>? Legend { get; set; }
    IFoo<K, T>? this[int index] { get; set; }
    IFoo<K, T>? Add(T item);
}

// ========================================================
#if GENERATE
public partial interface IBar<K> : IUpcastEx<IFoo<K, string>>
{ }
#else
public partial interface IBar<K> : IFoo<K, string>
{
    new IBar<K>? Legend { get; set; }
    new IBar<K>? this[int index] { get; set; }
    new IBar<K>? Add(string item);
}
#endif

// ========================================================
public class Foo<K, T> : IFoo<K, T>
{
    public int Count => 0;
    public Foo<K, T>? Legend { get => this; set { } }
    IFoo<K, T>? IFoo<K, T>.Legend
    {
        get => Legend;
        set => Legend = (Foo<K, T>?)value;
    }

    public Foo<K, T>? this[int index] { get => this; set { } }
    IFoo<K, T>? IFoo<K, T>.this[int index]
    {
        get => this[index];
        set => this[index] = (Foo<K, T>?)value;
    }

    public virtual Foo<K, T>? Add(T item) => this;
    IFoo<K, T>? IFoo<K, T>.Add(T item) => Add(item);
}

// ========================================================
#if GENERATE
public partial class Bar<K> : IUpcastEx<Foo<K, string>>, IUpcastEx<IBar<K>>
{ }
#else
public partial class Bar<K> : Foo<K, string>, IBar<K>
{
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

    IBar<K>? IBar<K>.Legend
    {
        get => Legend;
        set => Legend = (Bar<K>?)value;
    }
    IBar<K>? IBar<K>.this[int index]
    {
        get => this[index];
        set => this[index] = (Bar<K>?)value;
    }
    IBar<K>? IBar<K>.Add(string item) => Add(item);
}
#endif

// ========================================================
//[Enforced]
//public static class Test
//{
//    //[Enforced]
//    [Fact]
//    public static void Execute()
//    {
//    }
//}