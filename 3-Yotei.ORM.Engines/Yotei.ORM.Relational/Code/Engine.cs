namespace Yotei.ORM.Relational.Code;

// ========================================================
/// <inheritdoc cref="IEngine"/>
[WithGenerator]
public partial class Engine : ORM.Code.Engine, IEngine
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="factory"></param>
    public Engine(DbProviderFactory factory) : base()
    {
        Factory = factory.ThrowWhenNull();
        KnownTags = new KnownTags(CASESENSITIVETAGS);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Engine(Engine source) : base(source) => Factory = source.Factory;

    /// <inheritdoc/>
    public override string ToString() => "Relational.Engine";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public DbProviderFactory Factory { get; }

    /// <inheritdoc cref="ORM.IEngine.KnownTags"/>
    public new IKnownTags KnownTags
    {
        get => (IKnownTags)base.KnownTags;
        init => base.KnownTags = value;
    }
}