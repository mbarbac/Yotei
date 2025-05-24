using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Relational.Tests.Core;

// ========================================================
//[Enforced]
public static class Test_Engine
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var engine = new FakeEngine();

        Assert.IsType<SqlClientFactory>(engine.Factory);
        Assert.IsAssignableFrom<IKnownTags>(engine.KnownTags);

        Assert.False(engine.CaseSensitiveNames);
        Assert.Equal("NULL", engine.NullValueLiteral);
        Assert.False(engine.PositionalParameters);
        Assert.Equal("@", engine.ParameterPrefix);
        Assert.True(engine.SupportsNativePaging);
        Assert.True(engine.UseTerminators);
        Assert.Equal('[', engine.LeftTerminator);
        Assert.Equal(']', engine.RightTerminator);

        var tags = engine.KnownTags;
        Assert.Equal(3, tags.IdentifierTags.Count);
        Assert.Equal("BaseSchemaName", tags.IdentifierTags[0].Default);
        Assert.Equal("BaseTableName", tags.IdentifierTags[1].Default);
        Assert.Equal("BaseColumnName", tags.IdentifierTags[2].Default);

        Assert.Equal("IsKey", tags.PrimaryKeyTag!.Default);
        Assert.Equal("IsUnique", tags.UniqueValuedTag!.Default);
        Assert.Equal("IsReadOnly", tags.ReadOnlyTag!.Default);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new FakeEngine();
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.True(source.Equals(target));
    }
}
