namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IEngine"/>
/// </summary>
[WithGenerator]
public partial class Engine : IEngine
{
    public const bool CASESENSITIVENAMES = false;
    public const string NULLVALUELITERAL = "NULL";
    public const bool NATIVEPAGING = false;
    public const string PARAMETERPREFIX = "#";
    public const bool POSITIONALPARAMETERS = false;
    public const bool USETERMINATORS = true;
    public const char LEFTERMINATOR = '[';
    public const char RIGHTTERMINATOR = ']';
    public const bool CASESENSITIVETAGS = false;

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public Engine() { }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="source"></param>
    protected Engine(Engine source)
    {
        source = source.ThrowWhenNull();

        CaseSensitiveNames = source.CaseSensitiveNames;
        NullValueLiteral = source.NullValueLiteral;
        NativePaging = source.NativePaging;
        ParameterPrefix = source.ParameterPrefix;
        PositionalParameters = source.PositionalParameters;
        UseTerminators = source.UseTerminators;
        LeftTerminator = source.LeftTerminator;
        RightTerminator = source.RightTerminator;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = CaseSensitiveNames.GetHashCode();
        code = HashCode.Combine(code, NullValueLiteral);
        code = HashCode.Combine(code, NativePaging);
        code = HashCode.Combine(code, ParameterPrefix);
        code = HashCode.Combine(code, PositionalParameters);
        code = HashCode.Combine(code, UseTerminators);
        code = HashCode.Combine(code, LeftTerminator);
        code = HashCode.Combine(code, RightTerminator);
        return code;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => "ORM.Engine";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(IEngine? other)
    {
        if (other is null) return false;

        if (CaseSensitiveNames != other.CaseSensitiveNames) return false;
        if (!NullValueLiteral.Equals(other.NullValueLiteral)) return false;
        if (NativePaging != other.NativePaging) return false;
        if (!ParameterPrefix.Equals(other.ParameterPrefix)) return false;
        if (PositionalParameters != other.PositionalParameters) return false;
        if (UseTerminators != other.UseTerminators) return false;
        if (LeftTerminator != other.LeftTerminator) return false;
        if (RightTerminator != other.RightTerminator) return false;

        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as IEngine);

    // ----------------------------------------------------

    /// <summary>
    /// <<inheritdoc/>
    /// </summary>
    public bool CaseSensitiveNames { get; init; } = CASESENSITIVENAMES;

    /// <summary>
    /// <<inheritdoc/>
    /// </summary>
    public string NullValueLiteral
    {
        get => _NullValueLiteral;
        init => _NullValueLiteral = value.NotNullNotEmpty();
    }
    string _NullValueLiteral = NULLVALUELITERAL;

    /// <summary>
    /// <<inheritdoc/>
    /// </summary>
    public bool NativePaging { get; init; } = NATIVEPAGING;

    /// <summary>
    /// <<inheritdoc/>
    /// </summary>
    public string ParameterPrefix
    {
        get => _ParameterPrefix;
        init => _ParameterPrefix = value.NotNullNotEmpty();
    }
    string _ParameterPrefix = PARAMETERPREFIX;

    /// <summary>
    /// <<inheritdoc/>
    /// </summary>
    public bool PositionalParameters { get; init; } = POSITIONALPARAMETERS;

    /// <summary>
    /// <<inheritdoc/>
    /// </summary>
    public bool UseTerminators { get; init; } = USETERMINATORS;

    static char ValidateTerminator(char value)
    {
        if (value < 32) throw new ArgumentException("Invalid terminator.").WithData(value);
        if (value == '.') throw new ArgumentException("Dots cannot be terminators.").WithData(value);
        if (value == ' ') throw new ArgumentException("Spaces cannot be terminators.").WithData(value);
        return value;
    }

    /// <summary>
    /// <<inheritdoc/>
    /// </summary>
    public char LeftTerminator
    {
        get => _LeftTerminator;
        init => _LeftTerminator = ValidateTerminator(value);
    }
    char _LeftTerminator = LEFTERMINATOR;

    /// <summary>
    /// <<inheritdoc/>
    /// </summary>
    public char RightTerminator
    {
        get => _RightTerminator;
        init => _RightTerminator = ValidateTerminator(value);
    }
    char _RightTerminator = RIGHTTERMINATOR;
}