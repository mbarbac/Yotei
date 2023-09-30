namespace Experimental.Tests;

// ========================================================
//[Enforced]
public static class Test_BuilderSpecs
{
    //[Enforced]
    [Fact]
    public static void Test_Empty()
    {
        var item = new BuilderSpecs(null);
        Assert.Equal("(*)+*", item.ToString());

        item = new BuilderSpecs(" ");
        Assert.Equal("(*)+*", item.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_NoName_Arguments()
    {
        var item = new BuilderSpecs("()");
        Assert.Equal("()+*", item.ToString());

        item = new BuilderSpecs("(one)");
        Assert.Equal("(one)+*", item.ToString());

        item = new BuilderSpecs("(one,two=@!)");
        Assert.Equal("(one,two=@!)+*", item.ToString());

        item = new BuilderSpecs("(*)");
        Assert.Equal("(*)+*", item.ToString());

        try { _ = new BuilderSpecs("(one,*)"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new BuilderSpecs("(*,one)"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Name_Arguments()
    {
        var item = new BuilderSpecs("name");
        Assert.Equal("name(*)+*", item.ToString());

        item = new BuilderSpecs("name()");
        Assert.Equal("name()+*", item.ToString());

        item = new BuilderSpecs("name(one)");
        Assert.Equal("name(one)+*", item.ToString());

        item = new BuilderSpecs("name(one,two=@!)");
        Assert.Equal("name(one,two=@!)+*", item.ToString());

        item = new BuilderSpecs("name(*)");
        Assert.Equal("name(*)+*", item.ToString());

        try { _ = new BuilderSpecs("name(one,*)"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new BuilderSpecs("name(*,one)"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_NoName_Optionals()
    {
        var item = new BuilderSpecs("()+*");
        Assert.Equal("()+*", item.ToString());

        item = new BuilderSpecs("()-*");
        Assert.Equal("()", item.ToString());

        item = new BuilderSpecs("()+one");
        Assert.Equal("()+one", item.ToString());

        item = new BuilderSpecs("()+one=@!+two");
        Assert.Equal("()+one=@!+two", item.ToString());

        item = new BuilderSpecs("()+one=@!+two-*");
        Assert.Equal("()", item.ToString());

        item = new BuilderSpecs("()+one=@!+two+*");
        Assert.Equal("()+*", item.ToString());

        try { _ = new BuilderSpecs("()+one+"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new BuilderSpecs("()++"); Assert.Fail(); }
        catch (ArgumentException) { }
    }
}