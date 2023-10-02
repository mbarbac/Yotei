namespace Experimental.Tests;

// ========================================================
//[Enforced]
public static class Test_BuilderSpecs
{
    //[Enforced]
    [Fact]
    public static void Test_Empty()
    {
        var specs = new BuilderSpecs();
        Assert.Null(specs.Name);
        Assert.Single(specs.Arguments);
        Assert.True(specs.AllArguments);
        Assert.Empty(specs.Optionals);

        specs = new BuilderSpecs("  ");
        Assert.Null(specs.Name);
        Assert.Single(specs.Arguments);
        Assert.True(specs.AllArguments);
        Assert.Empty(specs.Optionals);
    }

    //[Enforced]
    [Fact]
    public static void Test_NoName_Arguments()
    {
        var specs = new BuilderSpecs("()");
        Assert.Null(specs.Name);
        Assert.Empty(specs.Arguments);
        Assert.False(specs.AllArguments);
        Assert.Empty(specs.Optionals);

        specs = new BuilderSpecs("(*)");
        Assert.Null(specs.Name);
        Assert.Single(specs.Arguments);
        Assert.True(specs.AllArguments);
        Assert.Empty(specs.Optionals);

        specs = new BuilderSpecs("(one=member!,two=@)");
        Assert.Null(specs.Name);
        Assert.Equal(2, specs.Arguments.Count);
        Assert.Equal("one=member!", specs.Arguments[0].ToString());
        Assert.Equal("two=@", specs.Arguments[1].ToString());
        Assert.False(specs.AllArguments);
        Assert.Empty(specs.Optionals);
    }

    //[Enforced]
    [Fact]
    public static void Test_NoName_Optionals()
    {
        var specs = new BuilderSpecs("+*");
        Assert.Null(specs.Name);
        Assert.Single(specs.Arguments);
        Assert.True(specs.AllArguments);
        Assert.Single(specs.Optionals);
        Assert.Equal("+*", specs.Optionals[0].ToString());

        specs = new BuilderSpecs("-info+other");
        Assert.Null(specs.Name);
        Assert.Single(specs.Arguments);
        Assert.True(specs.AllArguments);
        Assert.Equal(2, specs.Optionals.Count);
        Assert.Equal("-info", specs.Optionals[0].ToString());
        Assert.Equal("+other", specs.Optionals[1].ToString());

        specs = new BuilderSpecs("-info+*");
        Assert.Null(specs.Name);
        Assert.Single(specs.Arguments);
        Assert.True(specs.AllArguments);
        Assert.Single(specs.Optionals);
        Assert.Equal("+*", specs.Optionals[0].ToString());

        specs = new BuilderSpecs("+other-*");
        Assert.Null(specs.Name);
        Assert.Single(specs.Arguments);
        Assert.True(specs.AllArguments);
        Assert.Empty(specs.Optionals);
    }

    //[Enforced]
    [Fact]
    public static void Test_NoName_Complete()
    {
        var specs = new BuilderSpecs("()+*");
        Assert.Null(specs.Name);
        Assert.Empty(specs.Arguments);
        Assert.False(specs.AllArguments);
        Assert.Single(specs.Optionals);
        Assert.Equal("+*", specs.Optionals[0].ToString());

        specs = new BuilderSpecs("(*)+*");
        Assert.Null(specs.Name);
        Assert.Single(specs.Arguments);
        Assert.True(specs.AllArguments);
        Assert.Single(specs.Optionals);
        Assert.Equal("+*", specs.Optionals[0].ToString());

        specs = new BuilderSpecs("(one=member!,two=@)+*-info");
        Assert.Null(specs.Name);
        Assert.Equal(2, specs.Arguments.Count);
        Assert.Equal("one=member!", specs.Arguments[0].ToString());
        Assert.Equal("two=@", specs.Arguments[1].ToString());
        Assert.Equal(2, specs.Optionals.Count);
        Assert.Equal("+*", specs.Optionals[0].ToString());
        Assert.Equal("-info", specs.Optionals[1].ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Name()
    {
        var specs = new BuilderSpecs("method");
        Assert.Equal("method", specs.Name);
        Assert.Single(specs.Arguments);
        Assert.True(specs.AllArguments);
        Assert.Empty(specs.Optionals);
    }

    //[Enforced]
    [Fact]
    public static void Test_Name_Arguments()
    {
        var specs = new BuilderSpecs("method()");
        Assert.Equal("method", specs.Name);
        Assert.Empty(specs.Arguments);
        Assert.False(specs.AllArguments);
        Assert.Empty(specs.Optionals);

        specs = new BuilderSpecs("method(*)");
        Assert.Equal("method", specs.Name);
        Assert.Single(specs.Arguments);
        Assert.True(specs.AllArguments);
        Assert.Empty(specs.Optionals);

        specs = new BuilderSpecs("method(one=member!,two=@)");
        Assert.Equal("method", specs.Name);
        Assert.Equal(2, specs.Arguments.Count);
        Assert.Equal("one=member!", specs.Arguments[0].ToString());
        Assert.Equal("two=@", specs.Arguments[1].ToString());
        Assert.False(specs.AllArguments);
        Assert.Empty(specs.Optionals);
    }

    //[Enforced]
    [Fact]
    public static void Test_Name_Optionals()
    {
        var specs = new BuilderSpecs("method+*");
        Assert.Equal("method", specs.Name);
        Assert.Single(specs.Arguments);
        Assert.True(specs.AllArguments);
        Assert.Single(specs.Optionals);
        Assert.Equal("+*", specs.Optionals[0].ToString());

        specs = new BuilderSpecs("method-info+other");
        Assert.Equal("method", specs.Name);
        Assert.Single(specs.Arguments);
        Assert.True(specs.AllArguments);
        Assert.Equal(2, specs.Optionals.Count);
        Assert.Equal("-info", specs.Optionals[0].ToString());
        Assert.Equal("+other", specs.Optionals[1].ToString());

        specs = new BuilderSpecs("method-info+*");
        Assert.Equal("method", specs.Name);
        Assert.Single(specs.Arguments);
        Assert.True(specs.AllArguments);
        Assert.Single(specs.Optionals);
        Assert.Equal("+*", specs.Optionals[0].ToString());

        specs = new BuilderSpecs("method+other-*");
        Assert.Equal("method", specs.Name);
        Assert.Single(specs.Arguments);
        Assert.True(specs.AllArguments);
        Assert.Empty(specs.Optionals);
    }

    //[Enforced]
    [Fact]
    public static void Test_Name_Complete()
    {
        var specs = new BuilderSpecs("method()+*");
        Assert.Equal("method", specs.Name);
        Assert.Empty(specs.Arguments);
        Assert.False(specs.AllArguments);
        Assert.Single(specs.Optionals);
        Assert.Equal("+*", specs.Optionals[0].ToString());

        specs = new BuilderSpecs("method(*)+*");
        Assert.Equal("method", specs.Name);
        Assert.Single(specs.Arguments);
        Assert.True(specs.AllArguments);
        Assert.Single(specs.Optionals);
        Assert.Equal("+*", specs.Optionals[0].ToString());

        specs = new BuilderSpecs("method(one=member!,two=@)+*-info");
        Assert.Equal("method", specs.Name);
        Assert.Equal(2, specs.Arguments.Count);
        Assert.Equal("one=member!", specs.Arguments[0].ToString());
        Assert.Equal("two=@", specs.Arguments[1].ToString());
        Assert.Equal(2, specs.Optionals.Count);
        Assert.Equal("+*", specs.Optionals[0].ToString());
        Assert.Equal("-info", specs.Optionals[1].ToString());
    }
}