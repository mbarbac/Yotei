#if NETSTANDARD2_0
namespace Yotei.Tools.Std20.Tests;
#else
namespace Yotei.Tools;
#endif

// ========================================================
public static class Test_ReadOnlySpanExtensions
{
    [Fact]
    public static void Test_SequenceEquals()
    {
        Item<int>[] nums;
        bool same;

        nums = [new(1), new(2)]; same = nums.AsSpan().SequenceEqual([]); Assert.True(same);
    }
}