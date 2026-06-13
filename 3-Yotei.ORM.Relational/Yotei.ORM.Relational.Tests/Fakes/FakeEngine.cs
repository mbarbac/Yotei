namespace Yotei.ORM.Relational.Tests;

// ========================================================
[Cloneable(ReturnType = typeof(IEngine))]
[InheritsWith(ReturnType = typeof(IEngine))]
public partial class FakeEngine : ORM.Relational.Code.Engine
{
    public FakeEngine() : base(SqlClientFactory.Instance)
    {
        IgnoreCase = true;
        NullValueLiteral = "NULL";
        PositionalParameters = false;
        ParameterPrefix = "@";
        NativePaging = true;
        UseTerminators = true;
        LeftTerminator = '[';
        RightTerminator = ']';
        KnownTags = new FakeKnownTags();
    }

    protected FakeEngine(FakeEngine source) : base(source) { }

    public override string ToString() => "FakeSqlEngine";
}