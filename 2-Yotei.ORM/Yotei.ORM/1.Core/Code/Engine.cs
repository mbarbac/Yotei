namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IEngine"/>
[InheritWiths]
public partial class Engine : IEngine
{
    public const bool CASESENSITIVENAMES = false;
    public const string NULLVALUELITERAL = "NULL";
    public const bool POSITIONALPARAMETERS = false;
    public const string PARAMETERSPREFIX = "#";
    public const bool NATIVEPAGING = false;
    public const bool USETERMINATORS = true;
    public const char LEFTTERMINATOR = '[';
    public const char RIGHTTERMINATOR = ']';

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public Engine() { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Engine(IEngine source)
    {
        CaseSensitiveNames = source.CaseSensitiveNames;
        NullValueLiteral = source.NullValueLiteral;
        PositionalParameters = source.PositionalParameters;
        ParametersPrefix = source.ParametersPrefix;
        NativePaging = source.NativePaging;
        UseTerminators = source.UseTerminators;
        LeftTerminator = source.LeftTerminator;
        RightTerminator = source.RightTerminator;
    }

    /// <inheritdoc/>
    public override string ToString() => "ORM.Engine";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IEngine? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        if (CaseSensitiveNames != other.CaseSensitiveNames) return false;
        if (string.Compare(NullValueLiteral, other.NullValueLiteral, !CaseSensitiveNames) != 0) return false;
        if (PositionalParameters != other.PositionalParameters) return false;
        if (string.Compare(ParametersPrefix, other.ParametersPrefix, !CaseSensitiveNames) != 0) return false;
        if (NativePaging != other.NativePaging) return false;
        if (UseTerminators != other.UseTerminators) return false;
        if (LeftTerminator != other.LeftTerminator) return false;
        if (RightTerminator != other.RightTerminator) return false;

        return true;
    }
    public override bool Equals(object? obj) => Equals(obj as IEngine);
    public static bool operator ==(Engine x, IEngine y) => x is not null && x.Equals(y);
    public static bool operator !=(Engine x, IEngine y) => !(x == y);
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, CaseSensitiveNames);
        code = HashCode.Combine(code, NullValueLiteral);
        code = HashCode.Combine(code, PositionalParameters);
        code = HashCode.Combine(code, ParametersPrefix);
        code = HashCode.Combine(code, NativePaging);
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
    public string ParametersPrefix
    {
        get => _ParametersPrefix;
        init => _ParametersPrefix = value.NotNullNotEmpty();
    }
    string _ParametersPrefix = PARAMETERSPREFIX;

    /// <inheritdoc/>
    public bool NativePaging { get; init; } = NATIVEPAGING;

    /// <inheritdoc/>
    public bool UseTerminators { get; init; } = USETERMINATORS;

    static char ValidateTerminator(char value)
    {
        if (value < 32) throw new ArgumentException("Invalid terminator.").WithData(value);
        if (value == '.') throw new ArgumentException("Dots cannot be terminators.").WithData(value);
        if (value == ' ') throw new ArgumentException("Spaces cannot be terminators.").WithData(value);
        return value;
    }

    /// <inheritdoc/>
    public char LeftTerminator
    {
        get => _LeftTerminator;
        init => _LeftTerminator = ValidateTerminator(value);
    }
    char _LeftTerminator = LEFTTERMINATOR;

    /// <inheritdoc/>
    public char RightTerminator
    {
        get => _RightTerminator;
        init => _RightTerminator = ValidateTerminator(value);
    }
    char _RightTerminator = RIGHTTERMINATOR;
}