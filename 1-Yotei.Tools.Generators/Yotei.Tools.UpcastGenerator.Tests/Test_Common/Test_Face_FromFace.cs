#define GENERATE_

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
//public static class Test
//{
//    //[Enforced]
//    [Fact]
//    public static void Execute()
//    {
//    }
//}