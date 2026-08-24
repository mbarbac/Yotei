namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_DbTokenIdentifier
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Chained()
    {
        var engine = new FakeEngine();
        var arg = new DbTokenArgument("x");

        var host1 = new DbTokenIdentifier(arg, new Identifier(engine, "Alpha"));
        Assert.Same(arg, host1.Host);
        Assert.Equal("x.[Alpha]", host1.ToString());
        Assert.Equal("[Alpha]", host1.Identifier.Value);

        var host2 = new DbTokenIdentifier(host1, new Identifier(engine, "Beta"));
        Assert.Same(host1, host2.Host);
        Assert.Equal("x.[Alpha].[Beta]", host2.ToString());
        Assert.Equal("[Beta]", host2.Identifier.Value);

        var host3 = new DbTokenIdentifier(host2, new Identifier(engine, "Gamma"));
        Assert.Same(host2, host3.Host);
        Assert.Equal("x.[Alpha].[Beta].[Gamma]", host3.ToString());
        Assert.Equal("[Gamma]", host3.Identifier.Value);

        var token = new DbTokenIdentifier(host3, new Identifier(engine, "Delta"));
        Assert.Same(host3, token.Host);
        Assert.Equal("x.[Alpha].[Beta].[Gamma].[Delta]", token.ToString());
        Assert.Equal("[Delta]", token.Identifier.Value);
    }
    
    //[Enforced]
    [Fact]
    public static void Test_Create_Chained_WithNulls()
    {
        var engine = new FakeEngine();
        var arg = new DbTokenArgument("x");

        var host1 = new DbTokenIdentifier(arg, new Identifier(engine));
        Assert.Same(arg, host1.Host);
        Assert.Equal("x.", host1.ToString());
        Assert.Null(host1.Identifier.Value);

        var host2 = new DbTokenIdentifier(host1, new Identifier(engine, "Beta"));
        Assert.Same(host1, host2.Host);
        Assert.Equal("x..[Beta]", host2.ToString());
        Assert.Equal("[Beta]", host2.Identifier.Value);

        var host3 = new DbTokenIdentifier(host2, new Identifier(engine));
        Assert.Same(host2, host3.Host);
        Assert.Equal("x..[Beta].", host3.ToString());
        Assert.Equal("[Beta]", host2.Identifier.Value);

        var token = new DbTokenIdentifier(host3, new Identifier(engine, "Delta"));
        Assert.Same(host3, token.Host);
        Assert.Equal("x..[Beta]..[Delta]", token.ToString());
        Assert.Equal("[Delta]", token.Identifier.Value);
    }
}