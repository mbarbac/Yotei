namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IEngine"/>
[WithGenerator]
public partial class Engine : ORM.Code.Engine, IEngine
{
    public const bool CASESENSITIVETAGS = false;

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public Engine() { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Engine(Engine source) : base(source) => KnownTags = source.KnownTags;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IEngine? other)
    {
        if (other is null) return false;

        if (!base.Equals(other)) return false;
        if (!KnownTags.Equals(other.KnownTags)) return false;
        
        return true;
    }
    public override bool Equals(object? obj) => Equals(obj as IEngine);
    public static bool operator ==(Engine x, IEngine y) => x is not null && x.Equals(y);
    public static bool operator !=(Engine x, IEngine y) => !(x == y);
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), KnownTags);

    /// <inheritdoc/>
    public override bool Equals(ORM.IEngine? other) => Equals(other as IEngine);

    // ----------------------------------------------------


    /// <inheritdoc/>
    public IKnownTags KnownTags
    {
        get => _KnownTags;
        init => _KnownTags = value.ThrowWhenNull();
    }
    IKnownTags _KnownTags = new KnownTags(CASESENSITIVETAGS);
}