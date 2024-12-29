using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;
using Runner.Builder;

namespace Runner;

// ========================================================
[Enforced]
public static class Test_Builder
{
    //[Enforced]
    [Fact]
    public static void Test_Increase_Version_Debug()
    {
        var mode = BuildMode.Debug;

        var oldversion = new SemanticVersion("1.2.3-v001");
        var newversion = Builder.incre

    /*
    //[Enforced]
    [Fact]
    public static void Test_Increase_Version_Debug()
    {
        var mode = BuildMode.Debug;

        var oldversion = new SemanticVersion("1.2.3-v001");
        var newversion = Builder.IncreaseVersion(oldversion, mode);
        Assert.Equal("1.2.3-v002", newversion);
        Assert.Equal("v002", newversion.PreRelease);

        oldversion = new SemanticVersion("1.2.3-xyz");
        newversion = Builder.IncreaseVersion(oldversion, mode);
        Assert.Equal("1.2.3-v001", newversion);
        Assert.Equal("v001", newversion.PreRelease);

        oldversion = new SemanticVersion("1.2.3");
        newversion = Builder.IncreaseVersion(oldversion, mode);
        Assert.Equal("1.2.4-v001", newversion);
        Assert.Equal(4, newversion.Patch);
        Assert.Equal("v001", newversion.PreRelease);
    }

    //[Enforced]
    [Fact]
    public static void Test_Increase_Version_Local()
    {
        var mode = BuildMode.Local;

        var oldversion = new SemanticVersion("1.2.3-v001");
        var newversion = Builder.IncreaseVersion(oldversion, mode);
        Assert.Equal("1.2.3-v001", newversion);

        oldversion = new SemanticVersion("1.2.3-xyz");
        newversion = Builder.IncreaseVersion(oldversion, mode);
        Assert.Equal("1.2.3-xyz", newversion);

        oldversion = new SemanticVersion("1.2.3");
        newversion = Builder.IncreaseVersion(oldversion, mode);
        Assert.Equal("1.2.3-v001", newversion);
    }

    //[Enforced]
    [Fact]
    public static void Test_Increase_Version_Release()
    {
        var mode = BuildMode.Release;

        var oldversion = new SemanticVersion("1.2.3-v001");
        var newversion = Builder.IncreaseVersion(oldversion, mode);
        Assert.Equal("1.2.3", newversion);
        Assert.True(newversion.PreRelease.IsEmpty);

        oldversion = new SemanticVersion("1.2.3-xyz");
        newversion = Builder.IncreaseVersion(oldversion, mode);
        Assert.Equal("1.2.3", newversion);
        Assert.True(newversion.PreRelease.IsEmpty);

        oldversion = new SemanticVersion("1.2.3");
        newversion = Builder.IncreaseVersion(oldversion, mode);
        Assert.Equal("1.2.4", newversion);
        Assert.Equal(4, newversion.Patch);
        Assert.True(newversion.PreRelease.IsEmpty);
    }
    */
}