namespace Yotei.ORM.Relational.Code;

// ========================================================
/// <inheritdoc cref="IEngine"/>
[Cloneable]
[InheritWiths]
public partial class Engine : ORM.Code.Engine, IEngine
{
    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    /// <param name="factory"></param>
    public Engine(DbProviderFactory factory) : base()
    {
        KnownTags = new KnownTags(CASESENSITIVETAGS);
        Factory = factory;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Engine(Engine source) : base(source) => Factory = source.Factory;

    /// <inheritdoc/>
    public override string ToString() => "Relational.Engine";

    // ----------------------------------------------------

    // ----------------------------------------------------

    /// <inheritdoc/>
    public new IKnownTags KnownTags
    {
        get => (IKnownTags)base.KnownTags;
        init => base.KnownTags = value;
    }

    /// <inheritdoc/>
    public DbProviderFactory Factory
    {
        get => _Factory;
        init => _Factory = value.ThrowWhenNull();
    }
    DbProviderFactory _Factory = default!;
}