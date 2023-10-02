namespace Experimental.Tests;

// ========================================================
//[Enforced]
public static class Test_BuilderArgument
{
    //[Enforced]
    [Fact]
    public static void Test_Empty()
    {
        try { _ = new BuilderArgument(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new BuilderArgument(" "); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Name_Asterisk()
    {
        var item = new BuilderArgument("*");
        Assert.Equal("*", item.Name);
        Assert.True(item.IsNameAsterisk);
        Assert.Null(item.Member);
        Assert.False(item.IsMemberEnforced);
        Assert.False(item.UseClone);

        try { _ = new BuilderArgument("*!"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new BuilderArgument("*=any"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Only_Name()
    {
        var item = new BuilderArgument("any");
        Assert.Equal("any", item.Name);
        Assert.False(item.IsNameAsterisk);
        Assert.Null(item.Member);
        Assert.False(item.IsMemberEnforced);
        Assert.False(item.UseClone);

        item = new BuilderArgument("any!");
        Assert.Equal("any", item.Name);
        Assert.False(item.IsNameAsterisk);
        Assert.Null(item.Member);
        Assert.False(item.IsMemberEnforced);
        Assert.True(item.UseClone);

        try { _ = new BuilderArgument(" !"); Assert.Fail(); }
        catch (EmptyException) { }

        try { _ = new BuilderArgument(" = "); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Name_Member()
    {
        var item = new BuilderArgument("any=member");
        Assert.Equal("any", item.Name);
        Assert.False(item.IsNameAsterisk);
        Assert.Equal("member", item.Member);
        Assert.False(item.IsMemberEnforced);
        Assert.False(item.UseClone);

        item = new BuilderArgument("any=member!");
        Assert.Equal("any", item.Name);
        Assert.False(item.IsNameAsterisk);
        Assert.Equal("member", item.Member);
        Assert.False(item.IsMemberEnforced);
        Assert.True(item.UseClone);

        try { _ = new BuilderArgument("any= "); Assert.Fail(); }
        catch (EmptyException) { }

        try { _ = new BuilderArgument("any= !"); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Name_Member_Enforced()
    {
        var item = new BuilderArgument("any=@");
        Assert.Equal("any", item.Name);
        Assert.False(item.IsNameAsterisk);
        Assert.Equal("@", item.Member);
        Assert.True(item.IsMemberEnforced);
        Assert.False(item.UseClone);

        item = new BuilderArgument("any=@!");
        Assert.Equal("any", item.Name);
        Assert.False(item.IsNameAsterisk);
        Assert.Equal("@", item.Member);
        Assert.True(item.IsMemberEnforced);
        Assert.True(item.UseClone);
    }

    //[Enforced]
    [Fact]
    public static void Test_Get_Member_NotEnforced()
    {
        var item = new BuilderArgument("any");
        Assert.Equal("any", item.GetMember(null));

        item = new BuilderArgument("any=other");
        Assert.Equal("other", item.GetMember(null));

        item = new BuilderArgument("any=@");
        Assert.Equal("any", item.GetMember(null));

        item = new BuilderArgument("*");
        try { item.GetMember(null); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Get_Member_Enforced()
    {
        var enforced = new EnforcedMember() { Name = "name", ValueName = "value" };

        var item = new BuilderArgument("any");
        Assert.Equal("any", item.GetMember(enforced));

        item = new BuilderArgument("any=other");
        Assert.Equal("other", item.GetMember(enforced));

        item = new BuilderArgument("any=@");
        Assert.Equal("name", item.GetMember(enforced));
    }

    //[Enforced]
    [Fact]
    public static void Test_Get_Value_NotEnforced()
    {
        var item = new BuilderArgument("any");
        Assert.Equal("any", item.GetValue(null));

        item = new BuilderArgument("any=other");
        Assert.Equal("other", item.GetValue(null));

        item = new BuilderArgument("any=@");
        Assert.Equal("any", item.GetValue(null));

        item = new BuilderArgument("*");
        try { item.GetValue(null); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Get_Value_Enforced()
    {
        var enforced = new EnforcedMember() { Name = "name", ValueName = "value" };

        var item = new BuilderArgument("any");
        Assert.Equal("any", item.GetValue(enforced));

        item = new BuilderArgument("any=other");
        Assert.Equal("other", item.GetValue(enforced));

        item = new BuilderArgument("any=@");
        Assert.Equal("value", item.GetValue(enforced));
    }
}