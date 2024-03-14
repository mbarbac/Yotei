namespace Yotei.ORM.SqlServer.Code;

// ========================================================
/// <inheritdoc cref="IEngine"/>
[WithGenerator]
public partial class Engine : Relational.Code.Engine, IEngine
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public Engine() : base(SqlClientFactory.Instance) { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Engine(Engine source) : base(source) { }

    /// <inheritdoc/>
    public new SqlClientFactory Factory => (SqlClientFactory)base.Factory;
}