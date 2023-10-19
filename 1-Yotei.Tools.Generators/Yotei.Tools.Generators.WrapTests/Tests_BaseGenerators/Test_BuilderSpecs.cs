namespace Yotei.Tools.Generators.Tests;

// ========================================================
//[Enforced]
public static class Test_BuilderSpecs
{
    //[Enforced]
    [Fact]
    public static void Test_Standar()
    {
        var specs = new BuilderSpecs();
        Assert.Null(specs.Name);
        Assert.Single(specs.Arguments); Assert.True(specs.AllArguments);
        Assert.Single(specs.Optionals); Assert.True(specs.Optionals[0].IsIncludeAll);

        specs = new BuilderSpecs("(*)");
        Assert.Null(specs.Name);
        Assert.Single(specs.Arguments); Assert.True(specs.AllArguments);
        Assert.Single(specs.Optionals); Assert.True(specs.Optionals[0].IsIncludeAll);

        specs = new BuilderSpecs("()");
        Assert.Null(specs.Name);
        Assert.Empty(specs.Arguments); Assert.False(specs.AllArguments);
        Assert.Single(specs.Optionals); Assert.True(specs.Optionals[0].IsIncludeAll);

        specs = new BuilderSpecs("name");
        Assert.Equal("name", specs.Name);
        Assert.Single(specs.Arguments); Assert.True(specs.AllArguments);
        Assert.Single(specs.Optionals); Assert.True(specs.Optionals[0].IsIncludeAll);

        specs = new BuilderSpecs("name(*)");
        Assert.Equal("name", specs.Name);
        Assert.Single(specs.Arguments); Assert.True(specs.AllArguments);
        Assert.Single(specs.Optionals); Assert.True(specs.Optionals[0].IsIncludeAll);

        specs = new BuilderSpecs("name()");
        Assert.Equal("name", specs.Name);
        Assert.Empty(specs.Arguments); Assert.False(specs.AllArguments);
        Assert.Single(specs.Optionals); Assert.True(specs.Optionals[0].IsIncludeAll);

        specs = new BuilderSpecs("+*");
        Assert.Null(specs.Name);
        Assert.Single(specs.Arguments); Assert.True(specs.AllArguments);
        Assert.Single(specs.Optionals); Assert.True(specs.Optionals[0].IsIncludeAll);

        specs = new BuilderSpecs("-*");
        Assert.Null(specs.Name);
        Assert.Single(specs.Arguments); Assert.True(specs.AllArguments);
        Assert.Empty(specs.Optionals);

        specs = new BuilderSpecs("-name");
        Assert.Null(specs.Name);
        Assert.Single(specs.Arguments); Assert.True(specs.AllArguments);
        Assert.Single(specs.Optionals);
        Assert.Equal("-name", specs.Optionals[0].ToString());

        specs = new BuilderSpecs("+name");
        Assert.Null(specs.Name);
        Assert.Single(specs.Arguments); Assert.True(specs.AllArguments);
        Assert.Single(specs.Optionals);
        Assert.Equal("+name", specs.Optionals[0].ToString());

        specs = new BuilderSpecs("+name!");
        Assert.Null(specs.Name);
        Assert.Single(specs.Arguments); Assert.True(specs.AllArguments);
        Assert.Single(specs.Optionals);
        Assert.Equal("+name!", specs.Optionals[0].ToString());

        specs = new BuilderSpecs("+name!-one+two");
        Assert.Null(specs.Name);
        Assert.Single(specs.Arguments); Assert.True(specs.AllArguments);
        Assert.Equal(3, specs.Optionals.Count);
        Assert.Equal("+name!", specs.Optionals[0].ToString());
        Assert.Equal("-one", specs.Optionals[1].ToString());
        Assert.Equal("+two", specs.Optionals[2].ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Errors()
    {
        try { _ = new BuilderSpecs("("); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderSpecs(")"); Assert.Fail(); } catch (ArgumentException) { }

        try { _ = new BuilderSpecs("name("); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderSpecs("name)"); Assert.Fail(); } catch (ArgumentException) { }

        try { _ = new BuilderSpecs("--"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderSpecs("+*-"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderSpecs("+one-"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderSpecs("+one--"); Assert.Fail(); } catch (ArgumentException) { }

        try { _ = new BuilderSpecs("-*!"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderSpecs("+*!"); Assert.Fail(); } catch (ArgumentException) { }

        try { _ = new BuilderSpecs("-*="); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderSpecs("-*=value"); Assert.Fail(); } catch (ArgumentException) { }

        try { _ = new BuilderSpecs("+*="); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderSpecs("+*=!"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderSpecs("+*=value"); Assert.Fail(); } catch (ArgumentException) { }

        try { _ = new BuilderSpecs("-name!"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderSpecs("-name="); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderSpecs("-name=!"); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new BuilderSpecs("-name=value"); Assert.Fail(); } catch (ArgumentException) { }
    }
}