namespace Yotei.ORM.Relational.Tests;

// ========================================================
[WithGenerator]
public partial class FakeEngine : Code.Engine
{
    public FakeEngine() : base(SqlClientFactory.Instance)
    {
        CaseSensitiveNames = false;
        NullValueLiteral = "NULL";
        NativePaging = true;
        ParameterPrefix = "@";
        PositionalParameters = false;
        UseTerminators = true;
        LeftTerminator = '[';
        RightTerminator = ']';
    }
    protected FakeEngine(FakeEngine source) : base(source) { }
}