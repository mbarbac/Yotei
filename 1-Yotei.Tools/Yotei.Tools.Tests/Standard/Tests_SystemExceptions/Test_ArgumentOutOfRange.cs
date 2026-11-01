#if NETSTANDARD2_0
namespace Yotei.Tools.Std20.Tests;
#else
namespace Yotei.Tools;
#endif

// ========================================================
public static class Test_ArgumentOutOfRange
{
    [Fact]
    public static void Test_ThrowIfZero()
    {
        //int value = 0;
        //try { ArgumentOutOfRangeException.ThrowIfZero(value); Assert.Fail(); }
        //catch (ArgumentOutOfRangeException) { }
    }

    [Fact]
    public static void Test_ThrowIfNegative()
    {
        //int value = -1;
        //try { ArgumentOutOfRangeException.ThrowIfNegative(value); Assert.Fail(); }
        //catch (ArgumentOutOfRangeException) { }
    }

    [Fact]
    public static void ThrowIfNegativeOrZero()
    {
        //int value = 0;
        //try { ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value); Assert.Fail(); }
        //catch (ArgumentOutOfRangeException) { }

        //value = -1;
        //try { ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value); Assert.Fail(); }
        //catch (ArgumentOutOfRangeException) { }
    }

    [Fact]
    public static void ThrowIfEqual()
    {
        //int source = 1;
        //int target = 1;
        //try { ArgumentOutOfRangeException.ThrowIfEqual(source, target); Assert.Fail(); }
        //catch (ArgumentOutOfRangeException) { }

        //target = 2; ArgumentOutOfRangeException.ThrowIfEqual(source, target);
    }

    [Fact]
    public static void ThrowIfNotEqual()
    {
        //int source = 1;
        //int target = 2;
        //try { ArgumentOutOfRangeException.ThrowIfNotEqual(source, target); Assert.Fail(); }
        //catch (ArgumentOutOfRangeException) { }

        //target = 1; ArgumentOutOfRangeException.ThrowIfNotEqual(source, target);
    }

    [Fact]
    public static void ThrowIfGreaterThan()
    {
        //int source = 2;
        //int target = 1;
        //try { ArgumentOutOfRangeException.ThrowIfGreaterThan(source, target); Assert.Fail(); }
        //catch (ArgumentOutOfRangeException) { }

        //target = 2; ArgumentOutOfRangeException.ThrowIfGreaterThan(source, target);
        //target = 3; ArgumentOutOfRangeException.ThrowIfGreaterThan(source, target);
    }

    [Fact]
    public static void ThrowIfGreaterThanOrEqual()
    {
        //int source = 2;
        //int target = 2;
        //try { ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(source, target); Assert.Fail(); }
        //catch (ArgumentOutOfRangeException) { }

        //target = 1;
        //try { ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(source, target); Assert.Fail(); }
        //catch (ArgumentOutOfRangeException) { }

        //target = 3; ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(source, target);
    }

    [Fact]
    public static void ThrowIfLessThan()
    {
        //int source = 1;
        //int target = 2;
        //try { ArgumentOutOfRangeException.ThrowIfLessThan(source, target); Assert.Fail(); }
        //catch (ArgumentOutOfRangeException) { }

        //target = 1; ArgumentOutOfRangeException.ThrowIfLessThan(source, target);
        //target = 0; ArgumentOutOfRangeException.ThrowIfLessThan(source, target);
    }

    [Fact]
    public static void ThrowIfLessThanOrEqual()
    {
        //int source = 2;
        //int target = 2;
        //try { ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(source, target); Assert.Fail(); }
        //catch (ArgumentOutOfRangeException) { }

        //target = 3;
        //try { ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(source, target); Assert.Fail(); }
        //catch (ArgumentOutOfRangeException) { }

        //target = 1; ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(source, target);
    }
}