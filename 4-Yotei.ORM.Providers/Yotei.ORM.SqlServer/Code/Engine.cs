namespace Yotei.ORM.SqlServer.Code;

// ========================================================
/// <inheritdoc cref="IEngine"/>
[WithGenerator]
public partial class Engine : Relational.Code.Engine, IEngine
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public Engine() : base(SqlClientFactory.Instance)
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

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Engine(Engine source) : base(source) { }

    /// <inheritdoc/>
    public override string ToString() => "SqlServer.Engine";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public new SqlClientFactory Factory => (SqlClientFactory)base.Factory;
}