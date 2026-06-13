namespace Yotei.ORM.Relational.Tests;

// ========================================================
//[Enforced]
public static partial class Test_Engine
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var engine = new FakeEngine();

        Assert.IsType<SqlClientFactory>(engine.DbFactory);
        Assert.True(engine.IgnoreCase);
        Assert.Equal("NULL", engine.NullValueLiteral);
        Assert.False(engine.PositionalParameters);
        Assert.Equal("@", engine.ParameterPrefix);
        Assert.True(engine.NativePaging);
        Assert.True(engine.UseTerminators);
        Assert.Equal('[', engine.LeftTerminator);
        Assert.Equal(']', engine.RightTerminator);

        Assert.True(engine.KnownTags.IgnoreCase);
        Assert.Equal("BaseSchemaName", engine.KnownTags.IdentifierTags!.Value[0].Default);
        Assert.Equal("BaseTableName", engine.KnownTags.IdentifierTags!.Value[1].Default);
        Assert.Equal("BaseColumnName", engine.KnownTags.IdentifierTags!.Value[2].Default);
        Assert.Equal("IsKey", engine.KnownTags.PrimaryKeyTag!.Default);
        Assert.Equal("IsUnique", engine.KnownTags.UniqueValuedTag!.Default);
        Assert.Equal("IsReadOnly", engine.KnownTags.ReadOnlyTag!.Default);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new FakeEngine()
        {
            IgnoreCase = false,
            NullValueLiteral = "ANY",
            PositionalParameters = true,
            ParameterPrefix = "!",
            NativePaging = false,
            UseTerminators = false,
            LeftTerminator = '(',
            RightTerminator = ')',
        };

        var target = source.Clone();
        Assert.Equal(source.DbFactory.GetType(), target.DbFactory.GetType());
        Assert.Equal(source.IgnoreCase, target.IgnoreCase);
        Assert.Equal(source.NullValueLiteral, target.NullValueLiteral);
        Assert.Equal(source.PositionalParameters, target.PositionalParameters);
        Assert.Equal(source.ParameterPrefix, target.ParameterPrefix);
        Assert.Equal(source.NativePaging, target.NativePaging);
        Assert.Equal(source.UseTerminators, target.UseTerminators);
        Assert.Equal(source.LeftTerminator, target.LeftTerminator);
        Assert.Equal(source.RightTerminator, target.RightTerminator);
        Assert.Equal(source.KnownTags, target.KnownTags);
    }
}