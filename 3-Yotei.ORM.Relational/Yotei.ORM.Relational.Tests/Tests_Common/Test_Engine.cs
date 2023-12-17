namespace Yotei.ORM.Relational.Tests;

// ========================================================
//[Enforced]
public static class Test_Engine
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var engine = new FakeEngine();

        Assert.False(engine.CaseSensitiveNames);
        Assert.Equal("NULL", engine.NullValueLiteral);
        Assert.Equal("@", engine.ParameterPrefix);
        Assert.False(engine.PositionalParameters);
        Assert.True(engine.NativePaging);
        Assert.True(engine.UseTerminators);
        Assert.Equal('[', engine.LeftTerminator);
        Assert.Equal(']', engine.RightTerminator);

        Assert.True(ReferenceEquals(engine.Factory, SqlClientFactory.Instance));
    }
}