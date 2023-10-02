namespace Experimental.Tests;

// ========================================================
//[Enforced]
public static class Test_BuilderOptional
{
    //[Enforced]
    [Fact]
    public static void Test_Empty()
    {
        try { _ = new BuilderOptional(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new BuilderOptional(" "); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Include_Or_Exclude()
    {
        try { _ = new BuilderOptional("whatever"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Member_Asterisk()
    {
        var item = new BuilderOptional("+*");
        Assert.True(item.IsInclude);
        Assert.True(item.IsIncludeAll);
        Assert.False(item.IsExclude);
        Assert.False(item.IsExcludeAll);
        Assert.Equal("*", item.Member);
        Assert.True(item.IsMemberAsterisk);
        Assert.False(item.IsMemberEnforced);
        Assert.False(item.UseClone);

        try { _ = new BuilderOptional("+*!"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new BuilderOptional("+*=any"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new BuilderOptional("+*=@"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Member_Name()
    {
        var item = new BuilderOptional("+any");
        Assert.True(item.IsInclude);
        Assert.False(item.IsIncludeAll);
        Assert.False(item.IsExclude);
        Assert.False(item.IsExcludeAll);
        Assert.Equal("any", item.Member);
        Assert.False(item.IsMemberAsterisk);
        Assert.False(item.IsMemberEnforced);
        Assert.False(item.UseClone);

        item = new BuilderOptional("+any!");
        Assert.True(item.IsInclude);
        Assert.False(item.IsIncludeAll);
        Assert.False(item.IsExclude);
        Assert.False(item.IsExcludeAll);
        Assert.Equal("any", item.Member);
        Assert.False(item.IsMemberAsterisk);
        Assert.False(item.IsMemberEnforced);
        Assert.True(item.UseClone);

        try { _ = new BuilderOptional("+any="); Assert.Fail(); }
        catch (EmptyException) { }

        try { _ = new BuilderOptional("+any=!"); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Member_Enforced()
    {
        var item = new BuilderOptional("+any=@");
        Assert.True(item.IsInclude);
        Assert.False(item.IsIncludeAll);
        Assert.False(item.IsExclude);
        Assert.False(item.IsExcludeAll);
        Assert.Equal("any", item.Member);
        Assert.False(item.IsMemberAsterisk);
        Assert.True(item.IsMemberEnforced);
        Assert.False(item.UseClone);

        item = new BuilderOptional("+any=@!");
        Assert.True(item.IsInclude);
        Assert.False(item.IsIncludeAll);
        Assert.False(item.IsExclude);
        Assert.False(item.IsExcludeAll);
        Assert.Equal("any", item.Member);
        Assert.False(item.IsMemberAsterisk);
        Assert.True(item.IsMemberEnforced);
        Assert.True(item.UseClone);

        try { _ = new BuilderOptional("+any=any"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Excluide_Specials()
    {
        try { _ = new BuilderOptional("-any!"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new BuilderOptional("-any=@"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new BuilderOptional("-any=@!"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new BuilderOptional("-any=other"); Assert.Fail(); }
        catch (ArgumentException) { }
    }
}