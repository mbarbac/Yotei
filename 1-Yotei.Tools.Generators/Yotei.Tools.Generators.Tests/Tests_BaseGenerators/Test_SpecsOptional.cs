using static Yotei.Tools.Diagnostics.ConsoleWrapper;
using static System.ConsoleColor;

namespace Yotei.Tools.Generators.Tests;

// ========================================================
//[Enforced]
public static class Test_SpecsOptional
{
    //[Enforced]
    [Fact]
    public static void Test_Empty()
    {
        try { _ = new SpecsOptional(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new SpecsOptional(""); Assert.Fail(); }
        catch (Shared.EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_No_Prefix()
    {
        try { _ = new SpecsOptional("x"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_All_Asterisk()
    {
        var item = new SpecsOptional("-*");
        Assert.True(item.IsExclude);
        Assert.False(item.IsInclude);
        Assert.Equal("*", item.Member);
        Assert.True(item.IsMemberAsterisk);
        Assert.True(item.IsExcludeAll);
        Assert.False(item.IsIncludeAll);
        Assert.False(item.IsMemberEnforced);
        Assert.False(item.UseClone);
        
        item = new SpecsOptional("+*");
        Assert.False(item.IsExclude);
        Assert.True(item.IsInclude);
        Assert.Equal("*", item.Member);
        Assert.True(item.IsMemberAsterisk);
        Assert.False(item.IsExcludeAll);
        Assert.True(item.IsIncludeAll);
        Assert.False(item.IsMemberEnforced);
        Assert.False(item.UseClone);

        try { _ = new SpecsOptional("-*abc"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new SpecsOptional("-*=@"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new SpecsOptional("-*=!"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new SpecsOptional("-*=@!"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Member_Name()
    {
        var item = new SpecsOptional("-name");
        Assert.True(item.IsExclude);
        Assert.False(item.IsInclude);
        Assert.Equal("name", item.Member);
        Assert.False(item.IsMemberAsterisk);
        Assert.False(item.IsExcludeAll);
        Assert.False(item.IsIncludeAll);
        Assert.False(item.IsMemberEnforced);
        Assert.False(item.UseClone);

        item = new SpecsOptional("+name!");
        Assert.True(item.IsInclude);
        Assert.Equal("name", item.Member);
        Assert.False(item.IsMemberEnforced);
        Assert.True(item.UseClone);
    }

    //[Enforced]
    [Fact]
    public static void Test_Member_Name_Enforced()
    {
        var item = new SpecsOptional("-name=@");
        Assert.True(item.IsExclude);
        Assert.False(item.IsInclude);
        Assert.Equal("name", item.Member);
        Assert.False(item.IsMemberAsterisk);
        Assert.False(item.IsExcludeAll);
        Assert.False(item.IsIncludeAll);
        Assert.True(item.IsMemberEnforced);
        Assert.False(item.UseClone);

        try { _ = new SpecsOptional("+name=@!"); Assert.Fail(); }
        catch (ArgumentException) { }
    }
}