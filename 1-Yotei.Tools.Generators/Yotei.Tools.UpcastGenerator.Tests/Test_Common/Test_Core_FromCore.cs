#define GENERATE

namespace Yotei.Tools.UpcastGenerator.Tests.Core_FromCore;

// ========================================================
public class Foo<K, T>
{
    public int Count => 0;
    public Foo<K, T>? Legend { get => this; set { } }
    public Foo<K, T>? this[int index] { get => this; set { } }
    public virtual Foo<K, T>? Add(T item) => this;
}

// ========================================================
#if GENERATE
public partial class Bar<K> : IUpcastEx<Foo<K, string>>
{ }
#else
public partial class Bar<K> : Foo<K, string>
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
#endif

// ========================================================
//[Enforced]
public static class Test
{
    //[Enforced]
    [Fact]
    public static void Execute()
    {
        Bar<int> item = new Bar<int>();

        Assert.Equal(0, item.Count);
        Assert.Same(item, item.Legend);
        Assert.Same(item, item[0]);
        Assert.Same(item, item.Add(""));
    }
}