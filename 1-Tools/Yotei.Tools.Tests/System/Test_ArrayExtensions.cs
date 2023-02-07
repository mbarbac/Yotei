namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_ArrayExtensions
{
    //[Enforced]
    [Fact]
    public static void Test_AdjustTail()
    {
        int[] source = new int[] { 11, 22, 33 };
        int[] target;

        target = source.ResizeAtTail(2);
        Assert.Equal(2, target.Length);
        Assert.Equal(11, target[0]);
        Assert.Equal(22, target[1]);

        target = source.ResizeAtTail(4, pad: 0);
        Assert.Equal(4, target.Length);
        Assert.Equal(11, target[0]);
        Assert.Equal(22, target[1]);
        Assert.Equal(33, target[2]);
        Assert.Equal(0, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AdjustHead()
    {
        int[] source = new int[] { 11, 22, 33 };
        int[] target;

        target = source.ResizeAtHead(2);
        Assert.Equal(2, target.Length);
        Assert.Equal(22, target[0]);
        Assert.Equal(33, target[1]);

        target = source.ResizeAtHead(4, pad: 0);
        Assert.Equal(4, target.Length);
        Assert.Equal(0, target[0]);
        Assert.Equal(11, target[1]);
        Assert.Equal(22, target[2]);
        Assert.Equal(33, target[3]);
    }
}