namespace Yotei.Tools.Generators.Tests;

// ========================================================
//[Enforced]
public static class Test_BuilderOptional
{
    //[Enforced]
    [Fact]
    public static void Test_Standard()
    {
        var item = new BuilderOptional("+*")!;
        Assert.False(item.IsExclude); Assert.False(item.IsExcludeAll);
        Assert.True(item.IsInclude); Assert.True(item.IsIncludeAll);
        Assert.Equal("*", item.Name);
        Assert.True(item.IsNameAsterisk); Assert.False(item.IsNameEnforced);
        Assert.Null(item.Value);
        Assert.False(item.UseClone);

        item = new BuilderOptional("-*")!;
        Assert.True(item.IsExclude); Assert.True(item.IsExcludeAll);
        Assert.False(item.IsInclude); Assert.False(item.IsIncludeAll);
        Assert.Equal("*", item.Name);
        Assert.True(item.IsNameAsterisk); Assert.False(item.IsNameEnforced);
        Assert.Null(item.Value);
        Assert.False(item.UseClone);

        item = new BuilderOptional("+@")!;
        Assert.False(item.IsExclude); Assert.False(item.IsExcludeAll);
        Assert.True(item.IsInclude); Assert.False(item.IsIncludeAll);
        Assert.Equal("@", item.Name);
        Assert.False(item.IsNameAsterisk); Assert.True(item.IsNameEnforced);
        Assert.Null(item.Value);
        Assert.False(item.UseClone);

        item = new BuilderOptional("-name")!;
        Assert.True(item.IsExclude); Assert.False(item.IsExcludeAll);
        Assert.False(item.IsInclude); Assert.False(item.IsIncludeAll);
        Assert.Equal("name", item.Name);
        Assert.False(item.IsNameAsterisk); Assert.False(item.IsNameEnforced);
        Assert.Null(item.Value);
        Assert.False(item.UseClone);

        item = new BuilderOptional("+name")!;
        Assert.False(item.IsExclude); Assert.False(item.IsExcludeAll);
        Assert.True(item.IsInclude); Assert.False(item.IsIncludeAll);
        Assert.Equal("name", item.Name);
        Assert.False(item.IsNameAsterisk); Assert.False(item.IsNameEnforced);
        Assert.Null(item.Value);
        Assert.False(item.UseClone);

        item = new BuilderOptional("+name!")!;
        Assert.False(item.IsExclude); Assert.False(item.IsExcludeAll);
        Assert.True(item.IsInclude); Assert.False(item.IsIncludeAll);
        Assert.Equal("name", item.Name);
        Assert.False(item.IsNameAsterisk); Assert.False(item.IsNameEnforced);
        Assert.Null(item.Value);
        Assert.True(item.UseClone);

        item = new BuilderOptional("+name=value")!;
        Assert.False(item.IsExclude); Assert.False(item.IsExcludeAll);
        Assert.True(item.IsInclude); Assert.False(item.IsIncludeAll);
        Assert.Equal("name", item.Name);
        Assert.False(item.IsNameAsterisk); Assert.False(item.IsNameEnforced);
        Assert.Equal("value", item.Value);
        Assert.False(item.UseClone);

        item = new BuilderOptional("+name=value!")!;
        Assert.False(item.IsExclude); Assert.False(item.IsExcludeAll);
        Assert.True(item.IsInclude); Assert.False(item.IsIncludeAll);
        Assert.Equal("name", item.Name);
        Assert.False(item.IsNameAsterisk); Assert.False(item.IsNameEnforced);
        Assert.Equal("value", item.Value);
        Assert.True(item.UseClone);
    }

    //[Enforced]
    [Fact]
    public static void Test_Errors()
    {
        try { _ = new BuilderOptional(null!); Assert.Fail(); } catch (ArgumentNullException) { }

        try { _ = new BuilderOptional("+"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderOptional("+!"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderOptional("+="); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderOptional("+=value"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderOptional("+=value!"); Assert.Fail(); } catch (ArgumentException) { }

        try { _ = new BuilderOptional("-"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderOptional("-!"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderOptional("-="); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderOptional("-=value"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderOptional("-=value!"); Assert.Fail(); } catch (ArgumentException) { }

        try { _ = new BuilderOptional("+*!"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderOptional("+*="); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderOptional("+*=!"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderOptional("+*=value!"); Assert.Fail(); } catch (ArgumentException) { }

        try { _ = new BuilderOptional("-*!"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderOptional("-*="); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderOptional("-*=!"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderOptional("-*=value!"); Assert.Fail(); } catch (ArgumentException) { }

        try { _ = new BuilderOptional("-@"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderOptional("+@!"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderOptional("+@=value"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderOptional("+@=value!"); Assert.Fail(); } catch (ArgumentException) { }

        try { _ = new BuilderOptional("-name!"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderOptional("-name=value"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderOptional("-name=value!"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderOptional("-name=@"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderOptional("-name=@!"); Assert.Fail(); } catch (ArgumentException) { }
    }
}