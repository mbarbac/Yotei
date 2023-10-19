namespace Yotei.Tools.Generators.Tests;

// ========================================================
//[Enforced]
public static class Test_BuilderArgument
{
    //[Enforced]
    [Fact]
    public static void Test_Standard()
    {
        var item = new BuilderArgument("*")!;
        Assert.Equal("*", item.Name);
        Assert.True(item.IsNameAsterisk);
        Assert.Null(item.Value);
        Assert.False(item.IsValueEnforced);
        Assert.False(item.IsValueThis);
        Assert.False(item.UseClone);

        item = new BuilderArgument("name")!;
        Assert.Equal("name", item.Name);
        Assert.False(item.IsNameAsterisk);
        Assert.Null(item.Value);
        Assert.False(item.IsValueEnforced);
        Assert.False(item.IsValueThis);
        Assert.False(item.UseClone);

        item = new BuilderArgument("name!")!;
        Assert.Equal("name", item.Name);
        Assert.False(item.IsNameAsterisk);
        Assert.Null(item.Value);
        Assert.False(item.IsValueEnforced);
        Assert.False(item.IsValueThis);
        Assert.True(item.UseClone);

        item = new BuilderArgument("name=value")!;
        Assert.Equal("name", item.Name);
        Assert.False(item.IsNameAsterisk);
        Assert.Equal("value", item.Value);
        Assert.False(item.IsValueEnforced);
        Assert.False(item.IsValueThis);
        Assert.False(item.UseClone);

        item = new BuilderArgument("name=value!")!;
        Assert.Equal("name", item.Name);
        Assert.False(item.IsNameAsterisk);
        Assert.Equal("value", item.Value);
        Assert.False(item.IsValueEnforced);
        Assert.False(item.IsValueThis);
        Assert.True(item.UseClone);

        item = new BuilderArgument("name=@")!;
        Assert.Equal("name", item.Name);
        Assert.False(item.IsNameAsterisk);
        Assert.Equal("@", item.Value);
        Assert.True(item.IsValueEnforced);
        Assert.False(item.IsValueThis);
        Assert.False(item.UseClone);

        item = new BuilderArgument("name=@!")!;
        Assert.Equal("name", item.Name);
        Assert.False(item.IsNameAsterisk);
        Assert.Equal("@", item.Value);
        Assert.True(item.IsValueEnforced);
        Assert.False(item.IsValueThis);
        Assert.True(item.UseClone);

        item = new BuilderArgument("name=this")!;
        Assert.Equal("name", item.Name);
        Assert.False(item.IsNameAsterisk);
        Assert.Equal("this", item.Value);
        Assert.False(item.IsValueEnforced);
        Assert.True(item.IsValueThis);
        Assert.False(item.UseClone);

        item = new BuilderArgument("name=this!")!;
        Assert.Equal("name", item.Name);
        Assert.False(item.IsNameAsterisk);
        Assert.Equal("this", item.Value);
        Assert.False(item.IsValueEnforced);
        Assert.True(item.IsValueThis);
        Assert.True(item.UseClone);
    }

    //[Enforced]
    [Fact]
    public static void Test_Errors()
    {
        try { _ = new BuilderArgument(null!); Assert.Fail(); } catch (ArgumentNullException) { }

        try { _ = new BuilderArgument("!"); Assert.Fail(); } catch (ArgumentException) { }

        try { _ = new BuilderArgument("*!"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderArgument("*="); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderArgument("*=!"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderArgument("*=any!"); Assert.Fail(); } catch (ArgumentException) { }

        try { _ = new BuilderArgument("name="); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderArgument("name=*"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderArgument("name=!"); Assert.Fail(); } catch (ArgumentException) { }
    }
}