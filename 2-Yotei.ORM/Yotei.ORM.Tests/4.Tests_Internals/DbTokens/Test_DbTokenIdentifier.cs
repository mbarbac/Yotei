using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_DbTokenIdentifier
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        IEngine engine = new FakeEngine();
        IIdentifierPart id = new IdentifierPart(engine);
        var arg = new DbTokenArgument("x");
        var token = new DbTokenIdentifier(arg, id);

        Assert.Same(token.Identifier, id);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        IEngine engine = new FakeEngine();
        IIdentifierPart id = new IdentifierPart(engine, "Alpha");
        var arg = new DbTokenArgument("x");
        var token = new DbTokenIdentifier(arg, id);

        Assert.Same(token.Identifier, id);
        Assert.Equal("[Alpha]", token.Identifier.Value);
        Assert.IsType<DbTokenArgument>(token.Host);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Multi()
    {
        IEngine engine = new FakeEngine();
        IIdentifier id = new IdentifierChain(engine, "Alpha.Beta.Gamma");
        var arg = new DbTokenArgument("x");
        
        var token = DbTokenIdentifier.Create(arg, id);
        Assert.Equal("[Gamma]", token.Value);
        token = Assert.IsType<DbTokenIdentifier>(token.Host);
        Assert.Equal("[Beta]", token.Value);
        token = Assert.IsType<DbTokenIdentifier>(token.Host);
        Assert.Equal("[Alpha]", token.Value);
        Assert.IsType<DbTokenArgument>(token.Host);
    }
}