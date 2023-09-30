namespace Experimental.Tests;

// ========================================================
//[Enforced]
public static class Test_BuilderOptional
{
    //[Enforced]
    [Fact]
    public static void Test_Errors()
    {
        try { _ = new BuilderOptional(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new BuilderOptional(" "); Assert.Fail(); }
        catch (EmptyException) { }

        try { _ = new BuilderOptional("any"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Creation_All_Specification()
    {
        var item = new BuilderOptional("+*");
        Assert.True(item.IsInclude);
        Assert.Equal("*", item.Name);
        Assert.Null(item.Member);
        Assert.False(item.UseEnforced);
        Assert.False(item.UseClone);

        item = new BuilderOptional("-*");
        Assert.False(item.IsInclude);
        Assert.Equal("*", item.Name);
        Assert.Null(item.Member);
        Assert.False(item.UseEnforced);
        Assert.False(item.UseClone);

        try { _ = new BuilderOptional("+*="); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Creation_Not_Enforced()
    {
        var item = new BuilderOptional("+any");
        Assert.True(item.IsInclude);
        Assert.Equal("any", item.Name);
        Assert.Null(item.Member);
        Assert.False(item.UseEnforced);
        Assert.False(item.UseClone);

        item = new BuilderOptional("-any");
        Assert.False(item.IsInclude);
        Assert.Equal("any", item.Name);
        Assert.Null(item.Member);
        Assert.False(item.UseEnforced);
        Assert.False(item.UseClone);

        try { _ = new BuilderOptional("+any="); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new BuilderOptional("-any="); Assert.Fail(); }
        catch (ArgumentException) { }

        item = new BuilderOptional("+any=!");
        Assert.True(item.IsInclude);
        Assert.Equal("any", item.Name);
        Assert.Null(item.Member);
        Assert.False(item.UseEnforced);
        Assert.True(item.UseClone);

        try { _ = new BuilderOptional("-any=!"); Assert.Fail(); }
        catch (ArgumentException) { }

        item = new BuilderOptional("+any=member");
        Assert.True(item.IsInclude);
        Assert.Equal("any", item.Name);
        Assert.Equal("member", item.Member);
        Assert.False(item.UseEnforced);
        Assert.False(item.UseClone);

        try { _ = new BuilderOptional("-any=member"); Assert.Fail(); }
        catch (ArgumentException) { }

        item = new BuilderOptional("+any=member!");
        Assert.True(item.IsInclude);
        Assert.Equal("any", item.Name);
        Assert.Equal("member", item.Member);
        Assert.False(item.UseEnforced);
        Assert.True(item.UseClone);

        try { _ = new BuilderOptional("-any=member!"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Creation_Enforced()
    {
        var item = new BuilderOptional("+any=@");
        Assert.True(item.IsInclude);
        Assert.Equal("any", item.Name);
        Assert.Null(item.Member);
        Assert.True(item.UseEnforced);
        Assert.False(item.UseClone);

        try { _ = new BuilderOptional("-any=@"); Assert.Fail(); }
        catch (ArgumentException) { }

        item = new BuilderOptional("+any=@!");
        Assert.True(item.IsInclude);
        Assert.Equal("any", item.Name);
        Assert.Null(item.Member);
        Assert.True(item.UseEnforced);
        Assert.True(item.UseClone);

        try { _ = new BuilderOptional("-any=@!"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Creation_Enforced_Symbol_Used()
    {
        var item = new BuilderOptional("+any=@member");
        Assert.True(item.IsInclude);
        Assert.Equal("any", item.Name);
        Assert.Equal("@member", item.Member);
        Assert.False(item.UseEnforced);
        Assert.False(item.UseClone);

        try { _ = new BuilderOptional("-any=@member"); Assert.Fail(); }
        catch (ArgumentException) { }

        item = new BuilderOptional("+any=@member!");
        Assert.True(item.IsInclude);
        Assert.Equal("any", item.Name);
        Assert.Equal("@member", item.Member);
        Assert.False(item.UseEnforced);
        Assert.True(item.UseClone);

        try { _ = new BuilderOptional("-any=@member!"); Assert.Fail(); }
        catch (ArgumentException) { }
    }
}