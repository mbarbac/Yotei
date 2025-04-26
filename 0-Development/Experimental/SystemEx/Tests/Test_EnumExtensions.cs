using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Experimental.SystemEx;

// ========================================================
//[Enforced]
public static class Test_EnumExtensions
{
    [Flags]
    public enum USignedByte : byte
    {
        None = 0b0000,
        Alpha = 0b0001,
        Beta = 0b0010,
        Gamma = 0b0100,
    }

    //[Enforced]
    [Fact]
    public static void Validate_USignedByte_Single()
    {
        bool r;
        var item = USignedByte.None;
        r = item.HasFlag(USignedByte.Alpha); Assert.False(r);
        r = item.HasFlag(USignedByte.Beta); Assert.False(r);
        r = item.HasFlag(USignedByte.Gamma); Assert.False(r);

        item = item.AddFlags(USignedByte.Alpha);
        r = item.HasFlag(USignedByte.Alpha); Assert.True(r);
        r = item.HasFlag(USignedByte.Beta); Assert.False(r);
        r = item.HasFlag(USignedByte.Gamma); Assert.False(r);

        item = item.RemoveFlags(USignedByte.Alpha);
        r = item.HasFlag(USignedByte.Alpha); Assert.False(r);
        r = item.HasFlag(USignedByte.Beta); Assert.False(r);
        r = item.HasFlag(USignedByte.Gamma); Assert.False(r);
    }

    //[Enforced]
    [Fact]
    public static void Validate_USignedByte_Multiple()
    {
        bool r;
        var item = USignedByte.Alpha | USignedByte.Beta;
        r = item.HasFlag(USignedByte.Alpha); Assert.True(r);
        r = item.HasFlag(USignedByte.Beta); Assert.True(r);
        r = item.HasFlag(USignedByte.Gamma); Assert.False(r);

        item = item.AddFlags(USignedByte.Gamma);
        r = item.HasFlag(USignedByte.Alpha); Assert.True(r);
        r = item.HasFlag(USignedByte.Beta); Assert.True(r);
        r = item.HasFlag(USignedByte.Gamma); Assert.True(r);

        item = item.RemoveFlags(USignedByte.Alpha);
        r = item.HasFlag(USignedByte.Alpha); Assert.False(r);
        r = item.HasFlag(USignedByte.Beta); Assert.True(r);
        r = item.HasFlag(USignedByte.Gamma); Assert.True(r);
    }

    // ----------------------------------------------------

    [Flags]
    public enum SignedByte : sbyte
    {
        None = 0b0000,
        Alpha = 0b0001,
        Beta = 0b0010,
        Gamma = 0b0100,
    }

    //[Enforced]
    [Fact]
    public static void Validate_SignedByte_Single()
    {
        bool r;
        var item = SignedByte.None;
        r = item.HasFlag(SignedByte.Alpha); Assert.False(r);
        r = item.HasFlag(SignedByte.Beta); Assert.False(r);
        r = item.HasFlag(SignedByte.Gamma); Assert.False(r);

        item = item.AddFlags(SignedByte.Alpha);
        r = item.HasFlag(SignedByte.Alpha); Assert.True(r);
        r = item.HasFlag(SignedByte.Beta); Assert.False(r);
        r = item.HasFlag(SignedByte.Gamma); Assert.False(r);

        item = item.RemoveFlags(SignedByte.Alpha);
        r = item.HasFlag(SignedByte.Alpha); Assert.False(r);
        r = item.HasFlag(SignedByte.Beta); Assert.False(r);
        r = item.HasFlag(SignedByte.Gamma); Assert.False(r);
    }

    //[Enforced]
    [Fact]
    public static void Validate_SignedByte_Multiple()
    {
        bool r;
        var item = SignedByte.Alpha | SignedByte.Beta;
        r = item.HasFlag(SignedByte.Alpha); Assert.True(r);
        r = item.HasFlag(SignedByte.Beta); Assert.True(r);
        r = item.HasFlag(SignedByte.Gamma); Assert.False(r);

        item = item.AddFlags(SignedByte.Gamma);
        r = item.HasFlag(SignedByte.Alpha); Assert.True(r);
        r = item.HasFlag(SignedByte.Beta); Assert.True(r);
        r = item.HasFlag(SignedByte.Gamma); Assert.True(r);

        item = item.RemoveFlags(SignedByte.Alpha);
        r = item.HasFlag(SignedByte.Alpha); Assert.False(r);
        r = item.HasFlag(SignedByte.Beta); Assert.True(r);
        r = item.HasFlag(SignedByte.Gamma); Assert.True(r);
    }

    // ----------------------------------------------------

    [Flags]
    public enum USignedInt : uint
    {
        None = 0b0000,
        Alpha = 0b0001,
        Beta = 0b0010,
        Gamma = 0b0100,
    }

    //[Enforced]
    [Fact]
    public static void Validate_USignedInt_Single()
    {
        bool r;
        var item = USignedInt.None;
        r = item.HasFlag(USignedInt.Alpha); Assert.False(r);
        r = item.HasFlag(USignedInt.Beta); Assert.False(r);
        r = item.HasFlag(USignedInt.Gamma); Assert.False(r);

        item = item.AddFlags(USignedInt.Alpha);
        r = item.HasFlag(USignedInt.Alpha); Assert.True(r);
        r = item.HasFlag(USignedInt.Beta); Assert.False(r);
        r = item.HasFlag(USignedInt.Gamma); Assert.False(r);

        item = item.RemoveFlags(USignedInt.Alpha);
        r = item.HasFlag(USignedInt.Alpha); Assert.False(r);
        r = item.HasFlag(USignedInt.Beta); Assert.False(r);
        r = item.HasFlag(USignedInt.Gamma); Assert.False(r);
    }

    //[Enforced]
    [Fact]
    public static void Validate_USignedInt_Multiple()
    {
        bool r;
        var item = USignedInt.Alpha | USignedInt.Beta;
        r = item.HasFlag(USignedInt.Alpha); Assert.True(r);
        r = item.HasFlag(USignedInt.Beta); Assert.True(r);
        r = item.HasFlag(USignedInt.Gamma); Assert.False(r);

        item = item.AddFlags(USignedInt.Gamma);
        r = item.HasFlag(USignedInt.Alpha); Assert.True(r);
        r = item.HasFlag(USignedInt.Beta); Assert.True(r);
        r = item.HasFlag(USignedInt.Gamma); Assert.True(r);

        item = item.RemoveFlags(USignedInt.Alpha);
        r = item.HasFlag(USignedInt.Alpha); Assert.False(r);
        r = item.HasFlag(USignedInt.Beta); Assert.True(r);
        r = item.HasFlag(USignedInt.Gamma); Assert.True(r);
    }

    // ----------------------------------------------------

    [Flags]
    public enum SignedInt
    {
        None = 0b0000,
        Alpha = 0b0001,
        Beta = 0b0010,
        Gamma = 0b0100,
    }

    //[Enforced]
    [Fact]
    public static void Validate_SignedInt_Single()
    {
        bool r;
        var item = SignedInt.None;
        r = item.HasFlag(SignedInt.Alpha); Assert.False(r);
        r = item.HasFlag(SignedInt.Beta); Assert.False(r);
        r = item.HasFlag(SignedInt.Gamma); Assert.False(r);

        item = item.AddFlags(SignedInt.Alpha);
        r = item.HasFlag(SignedInt.Alpha); Assert.True(r);
        r = item.HasFlag(SignedInt.Beta); Assert.False(r);
        r = item.HasFlag(SignedInt.Gamma); Assert.False(r);

        item = item.RemoveFlags(SignedInt.Alpha);
        r = item.HasFlag(SignedInt.Alpha); Assert.False(r);
        r = item.HasFlag(SignedInt.Beta); Assert.False(r);
        r = item.HasFlag(SignedInt.Gamma); Assert.False(r);
    }

    //[Enforced]
    [Fact]
    public static void Validate_SignedInt_Multiple()
    {
        bool r;
        var item = SignedInt.Alpha | SignedInt.Beta;
        r = item.HasFlag(SignedInt.Alpha); Assert.True(r);
        r = item.HasFlag(SignedInt.Beta); Assert.True(r);
        r = item.HasFlag(SignedInt.Gamma); Assert.False(r);

        item = item.AddFlags(SignedInt.Gamma);
        r = item.HasFlag(SignedInt.Alpha); Assert.True(r);
        r = item.HasFlag(SignedInt.Beta); Assert.True(r);
        r = item.HasFlag(SignedInt.Gamma); Assert.True(r);

        item = item.RemoveFlags(SignedInt.Alpha);
        r = item.HasFlag(SignedInt.Alpha); Assert.False(r);
        r = item.HasFlag(SignedInt.Beta); Assert.True(r);
        r = item.HasFlag(SignedInt.Gamma); Assert.True(r);
    }
}