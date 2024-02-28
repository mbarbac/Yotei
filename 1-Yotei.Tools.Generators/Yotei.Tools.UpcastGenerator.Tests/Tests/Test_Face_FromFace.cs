#define GENERATE

namespace Yotei.Tools.UpcastGenerator.Tests.Face_FromFace;

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
    public partial interface IBar<K> : IFoo<K, string>
    { }

    // ====================================================

    public class Bar<K> : IBar<K>
    {
        public int Count => 0;

        public IBar<K>? Legend => this;
        IFoo<K, string>? IFoo<K, string>.Legend => Legend;

        public IBar<K>? this[int index] => this;
        IFoo<K, string>? IFoo<K, string>.this[int index] => this[index];

        public IBar<K>? Add(string item) => this;
        IFoo<K, string>? IFoo<K, string>.Add(string item) => Add(item);
    }

    //[Enforced]
    [Fact]
    public static void Test_Execute()
    {
        var bar = new Bar<int>();
        Assert.Same(bar, bar.Legend);
        Assert.Same(bar, bar[0]);
        Assert.Same(bar, bar.Add("any"));
    }
}