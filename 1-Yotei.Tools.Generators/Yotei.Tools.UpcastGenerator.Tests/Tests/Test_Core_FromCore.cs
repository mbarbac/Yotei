#define GENERATE

namespace Yotei.Tools.UpcastGenerator.Tests.Core_FromCore;

// ========================================================
//[Enforced]
public static partial class Test
{
    public class Foo<K, T>
    {
        public int Count => 0;
        public Foo<K, T>? Legend { get => this; set { } }
        public Foo<K, T>? this[int index] { get => this; set { } }
        public virtual Foo<K, T>? Add(T item) => this;
    }

#if !GENERATE
    partial class Bar<K>
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
    }
#else
    [Upcast(ChangeProperties = true)]
#endif
    public partial class Bar<K> : Foo<K, string> { }

    // ====================================================

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