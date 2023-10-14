using static Yotei.Tools.Diagnostics.ConsoleWrapper;
using static System.ConsoleColor;

namespace Yotei.Tools.Generators.Tests;

// ========================================================
//[Enforced]
public static class Test_SpecsArgument
{
    //[Enforced]
    [Fact]
    public static void Test_Empty()
    {
        try { _ = new SpecsArgument(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new SpecsArgument(""); Assert.Fail(); }
        catch (Shared.EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Plain_Name()
    {
        var item = new SpecsArgument("name");
        Assert.Equal("name", item.Name);
        Assert.False(item.IsNameAsterisk);
        Assert.Null(item.Member);
        Assert.False(item.IsEnforcedMember);
        Assert.False(item.UseClone);

        try { _ = new SpecsArgument("name="); Assert.Fail(); }
        catch (Shared.EmptyException) { }

        try { _ = new SpecsArgument("name=!"); Assert.Fail(); }
        catch (Shared.EmptyException) { }

        try { _ = new SpecsArgument("name!"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Name_Asterisk()
    {
        var item = new SpecsArgument("*");
        Assert.Equal("*", item.Name);
        Assert.True(item.IsNameAsterisk);
        Assert.Null(item.Member);
        Assert.False(item.IsEnforcedMember);
        Assert.False(item.UseClone);

        try { _ = new SpecsArgument("*xyx"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new SpecsArgument("*="); Assert.Fail(); }
        catch (Shared.EmptyException) { }

        try { _ = new SpecsArgument("*=!"); Assert.Fail(); }
        catch (Shared.EmptyException) { }

        try { _ = new SpecsArgument("*=member"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Source()
    {
        var item = new SpecsArgument("name=member");
        Assert.Equal("name", item.Name);
        Assert.False(item.IsNameAsterisk);
        Assert.Equal("member", item.Member);
        Assert.False(item.IsEnforcedMember);
        Assert.False(item.UseClone);

        item = new SpecsArgument("name=member!");
        Assert.Equal("name", item.Name);
        Assert.False(item.IsNameAsterisk);
        Assert.Equal("member", item.Member);
        Assert.False(item.IsEnforcedMember);
        Assert.True(item.UseClone);

        item = new SpecsArgument("name=@");
        Assert.Equal("name", item.Name);
        Assert.False(item.IsNameAsterisk);
        Assert.Equal("@", item.Member);
        Assert.True(item.IsEnforcedMember);
        Assert.False(item.UseClone);

        item = new SpecsArgument("name=@!");
        Assert.Equal("name", item.Name);
        Assert.False(item.IsNameAsterisk);
        Assert.Equal("@", item.Member);
        Assert.True(item.IsEnforcedMember);
        Assert.True(item.UseClone);
    }
}