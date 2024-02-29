#define GENERATE

namespace Yotei.Tools.UpcastGenerator.Tests.Face_FromFace;

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
//[Enforced]
public static partial class Test
{
    public partial class Bar<K> : IBar<K>
    {
        public int Count => 0;
        public Bar<K>? Legend { get => this; set { } }
        public Bar<K>? this[int index] { get => this; set { } }
        public Bar<K>? Add(string _) => this;

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


        IFoo<K, string>? IFoo<K, string>.Legend
        {
            get => Legend;
            set => Legend = (Bar<K>?)value;
        }
        IFoo<K, string>? IFoo<K, string>.this[int index]
        {
            get => this[index];
            set => this[index] = (Bar<K>?)value;
        }
        IFoo<K, string>? IFoo<K, string>.Add(string item) => Add(item);
    }

    //[Enforced]
    [Fact]
    public static void Execute()
    {
        IBar<int> item = new Bar<int>();

        Assert.Equal(0, item.Count);
        Assert.Same(item, item.Legend);
        Assert.Same(item, item[0]);
        Assert.Same(item, item.Add(""));
    }
}