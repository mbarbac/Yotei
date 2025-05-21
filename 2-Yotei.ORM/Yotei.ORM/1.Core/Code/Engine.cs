namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IEngine"/>
[Cloneable]
[InheritWiths]
public partial class Engine : IEngine
{
    public const bool CASESENSITIVENAMES = false;
    public const string NULLVALUELITERAL = "NULL";
    public const bool POSITIONALPARAMETERS = false;
    public const string PARAMETERPREFIX = "#";
    public const bool SUPPORTSNATIVEPAGING = false;
    public const bool USETERMINATORS = true;
    public const char LEFTTERMINATOR = '[';
    public const char RIGHTTERMINATOR = ']';
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
    protected Engine(Engine source)
    {
        CaseSensitiveNames = source.CaseSensitiveNames;
        NullValueLiteral = source.NullValueLiteral;
        PositionalParameters = source.PositionalParameters;
        ParameterPrefix = source.ParameterPrefix;
        SupportsNativePaging = source.SupportsNativePaging;
        UseTerminators = source.UseTerminators;
        LeftTerminator = source.LeftTerminator;
        RightTerminator = source.RightTerminator;
        KnownTags = source.KnownTags;
    }

    /// <inheritdoc/>
    public override string ToString() => "ORM.Engine";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IEngine? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        return
            CaseSensitiveNames == other.CaseSensitiveNames &&
            string.Compare(NullValueLiteral, other.NullValueLiteral, !CaseSensitiveNames) == 0 &&
            PositionalParameters == other.PositionalParameters &&
            string.Compare(ParameterPrefix, other.ParameterPrefix, !CaseSensitiveNames) == 0 &&
            SupportsNativePaging == other.SupportsNativePaging &&
            UseTerminators == other.UseTerminators &&
            LeftTerminator == other.LeftTerminator &&
            RightTerminator == other.RightTerminator &&
            KnownTags.Equals(other.KnownTags);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IEngine);

    // Equality operator.
    public static bool operator ==(Engine? x, Engine? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;
        return x.Equals(y);
    }

    // Inequality operator.
    public static bool operator !=(Engine? x, Engine? y) => !(x == y);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, CaseSensitiveNames);
        code = HashCode.Combine(code, NullValueLiteral);
        code = HashCode.Combine(code, PositionalParameters);
        code = HashCode.Combine(code, ParameterPrefix);
        code = HashCode.Combine(code, SupportsNativePaging);
        code = HashCode.Combine(code, UseTerminators);
        code = HashCode.Combine(code, LeftTerminator);
        code = HashCode.Combine(code, RightTerminator);

        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool CaseSensitiveNames { get; init; } = CASESENSITIVENAMES;

    /// <inheritdoc/>
    public string NullValueLiteral
    {
        get => _NullValueLiteral;
        init => _NullValueLiteral = value.NotNullNotEmpty();
    }
    string _NullValueLiteral = NULLVALUELITERAL;

    /// <inheritdoc/>
    public bool PositionalParameters { get; init; } = POSITIONALPARAMETERS;

    /// <inheritdoc/>
    public string ParameterPrefix
    {
        get => _ParameterPrefix;
        init => _ParameterPrefix = value.NotNullNotEmpty();
    }
    string _ParameterPrefix = PARAMETERPREFIX;

    /// <inheritdoc/>
    public bool SupportsNativePaging { get; init; } = SUPPORTSNATIVEPAGING;

    /// <inheritdoc/>
    public bool UseTerminators { get; init; } = USETERMINATORS;

    /// <inheritdoc/>
    public char LeftTerminator
    {
        get => _LeftTerminator;
        init => _LeftTerminator = ValidateTerminator(value);
    }
    char _LeftTerminator = LEFTTERMINATOR;

    static char ValidateTerminator(char value)
    {
        if (value < 32) throw new ArgumentException("Invalid terminator.").WithData(value);
        if (value == '.') throw new ArgumentException("Dots cannot be terminators.").WithData(value);
        if (value == ' ') throw new ArgumentException("Spaces cannot be terminators.").WithData(value);
        return value;
    }

    /// <inheritdoc/>
    public char RightTerminator
    {
        get => _RightTerminator;
        init => _RightTerminator = ValidateTerminator(value);
    }
    char _RightTerminator = RIGHTTERMINATOR;

    /// <inheritdoc/>
    public IKnownTags KnownTags
    {
        get => _KnownTags;
        init => _KnownTags = value.ThrowWhenNull();
    }
    IKnownTags _KnownTags = new KnownTags(CASESENSITIVETAGS);
}