namespace Experimental.Tests;

// ========================================================
//[Enforced]
public static class Test_BuilderArgument
{
    //[Enforced]
    [Fact]
    public static void Test_Errors()
    {
        try { _ = new BuilderArgument(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new BuilderArgument(" "); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Creation_All_Specification()
    {
        var item = new BuilderArgument("*");
        Assert.Equal("*", item.Name);
        Assert.Null(item.Member);
        Assert.False(item.UseEnforced);
        Assert.False(item.UseClone);

        try { _ = new BuilderArgument("*="); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Creation_Not_Enforced()
    {
        var item = new BuilderArgument("any");
        Assert.Equal("any", item.Name);
        Assert.Null(item.Member);
        Assert.False(item.UseEnforced);
        Assert.False(item.UseClone);

        try { _ = new BuilderArgument("any="); Assert.Fail(); }
        catch (ArgumentException) { }

        item = new BuilderArgument("any=!");
        Assert.Equal("any", item.Name);
        Assert.Null(item.Member);
        Assert.False(item.UseEnforced);
        Assert.True(item.UseClone);

        item = new BuilderArgument("any=member");
        Assert.Equal("any", item.Name);
        Assert.Equal("member", item.Member);
        Assert.False(item.UseEnforced);
        Assert.False(item.UseClone);

        item = new BuilderArgument("any=member!");
        Assert.Equal("any", item.Name);
        Assert.Equal("member", item.Member);
        Assert.False(item.UseEnforced);
        Assert.True(item.UseClone);
    }

    //[Enforced]
    [Fact]
    public static void Test_Creation_Enforced()
    {
        var item = new BuilderArgument("any=@");
        Assert.Equal("any", item.Name);
        Assert.Null(item.Member);
        Assert.True(item.UseEnforced);
        Assert.False(item.UseClone);

        item = new BuilderArgument("any=@!");
        Assert.Equal("any", item.Name);
        Assert.Null(item.Member);
        Assert.True(item.UseEnforced);
        Assert.True(item.UseClone);
    }

    //[Enforced]
    [Fact]
    public static void Test_Creation_Enforced_Symbol_Used()
    {
        var item = new BuilderArgument("any=@member");
        Assert.Equal("any", item.Name);
        Assert.Equal("@member", item.Member);
        Assert.False(item.UseEnforced);
        Assert.False(item.UseClone);

        item = new BuilderArgument("any=@member!");
        Assert.Equal("any", item.Name);
        Assert.Equal("@member", item.Member);
        Assert.False(item.UseEnforced);
        Assert.True(item.UseClone);
    }
}