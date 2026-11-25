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
    public static void Test_OnEnum_SequenceEqual_Ints()
    {
        bool result;
        var source = Array.Empty<int>();
        var target = IntArray();
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
    public static void Test_OnEnum_SequenceEqual_Items()
    {
        bool result;
        var source = Array.Empty<Item<int>>();
        var target = ItemArray();
        result = source.SequenceEqual(target);
        result = source.SequenceEqual(target, ItemEquality()); Assert.True(result);

        source = ItemArray([1, 2]);
        result = source.SequenceEqual(target); Assert.False(result);
        result = source.SequenceEqual(target, ItemEquality()); Assert.False(result);

        target = ItemArray([1, 2]);
        result = source.SequenceEqual(target); Assert.False(result); // elements are references
        result = source.SequenceEqual(target, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    // TODO ArraEx: IndexOf enum
    // TODO ArraEx: LastIndexOf enum
    // TODO ArraEx: IndexesOf enum
    // TODO ArraEx: Contains enum
    // TODO ArraEx: StartsWith enum
    // TODO ArraEx: EndsWith enum

    // ----------------------------------------------------

    // TODO ArraEx: IndexOfAny enum
    // TODO ArraEx: LastIndexOfAny enum
    // TODO ArraEx: IndexesOfAny enum
    // TODO ArraEx: ContainsAny enum
    // TODO ArraEx: StartsWithAny enum
    // TODO ArraEx: EndsWithAny enum

    // ----------------------------------------------------

    // TODO ArraEx: IndexOfAnyExcept enum
    // TODO ArraEx: LastIndexOfAnyExcept enum
    // TODO ArraEx: IndexesOfAnyExcept enum
    // TODO ArraEx: ContainsAnyExcept enum
    // TODO ArraEx: StartsWithAnyExcept enum
    // TODO ArraEx: EndsWithAnyExcept enum

    // ----------------------------------------------------

    // TODO ArraEx: TrimStart enum
    // TODO ArraEx: TrimEnd enum
    // TODO ArraEx: Trim enum

    // ----------------------------------------------------

    // TODO ArraEx: AddRange enum
    // TODO ArraEx: InsertRange enum
    // TODO ArraEx: Remove enum
    // TODO ArraEx: RemoveLast enum
    // TODO ArraEx: RemoveAll enum
}