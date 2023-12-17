namespace Yotei.ORM.Relational.Tests;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
[WithGenerator]
public partial class FakeEngine : Code.Engine, IEngine
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