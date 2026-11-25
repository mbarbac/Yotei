#pragma warning disable IDE0079
#pragma warning disable CA1859

#if NETSTANDARD2_0
using System.Net.Sockets;

namespace Yotei.Tools.NStd20.Tests;
#else
namespace Yotei.Tools.Tests;
#endif

// ========================================================
public static partial class Test_ArrayExtensions
{
    [Fact]
    public static void Test_OnSpan_SequenceEqual_Ints()
    {
        bool result;
        var source = Array.Empty<int>();
        var target = IntSpan();
        result = source.SequenceEqual(target); Assert.True(result);
        result = source.SequenceEqual(target, IntEquality()); Assert.True(result);

        source = [1, 2];
        result = source.SequenceEqual(target); Assert.False(result);
        result = source.SequenceEqual(target, IntEquality()); Assert.False(result);

        target = [1, 2];
        result = source.SequenceEqual(target); Assert.True(result);
        result = source.SequenceEqual(target, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_OnSpan_SequenceEqual_Items()
    {
        bool result;
        var source = Array.Empty<Item<int>>();
        var target = ItemSpan();
        result = source.SequenceEqual(target); Assert.True(result);
        result = source.SequenceEqual(target, ItemEquality()); Assert.True(result);

        source = ItemArray([1, 2]);
        result = source.SequenceEqual(target); Assert.False(result);
        result = source.SequenceEqual(target, ItemEquality()); Assert.False(result);

        target = ItemSpan([1, 2]);
        result = source.SequenceEqual(target); Assert.False(result); // elements are references
        result = source.SequenceEqual(target, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    // TODO ArraEx: IndexOf span
    // TODO ArraEx: LastIndexOf span
    // TODO ArraEx: IndexesOf span
    // TODO ArraEx: Contains span
    // TODO ArraEx: StartsWith span
    // TODO ArraEx: EndsWith span

    // ----------------------------------------------------

    // TODO ArraEx: IndexOfAny span
    // TODO ArraEx: LastIndexOfAny span
    // TODO ArraEx: IndexesOfAny span
    // TODO ArraEx: ContainsAny span
    // TODO ArraEx: StartsWithAny span
    // TODO ArraEx: EndsWithAny span

    // ----------------------------------------------------

    // TODO ArraEx: IndexOfAnyExcept span
    // TODO ArraEx: LastIndexOfAnyExcept span
    // TODO ArraEx: IndexesOfAnyExcept span
    // TODO ArraEx: ContainsAnyExcept span
    // TODO ArraEx: StartsWithAnyExcept span
    // TODO ArraEx: EndsWithAnyExcept span

    // ----------------------------------------------------

    // TODO ArraEx: TrimStart span
    // TODO ArraEx: TrimEnd span
    // TODO ArraEx: Trim span

    // ----------------------------------------------------

    // TODO ArraEx: AddRange span
    // TODO ArraEx: InsertRange span
    // TODO ArraEx: Remove span
    // TODO ArraEx: RemoveLast span
    // TODO ArraEx: RemoveAll span
}