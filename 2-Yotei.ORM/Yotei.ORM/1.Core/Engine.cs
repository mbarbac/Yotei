namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an underlying database engine.
/// </summary>
public abstract record Engine
{
    public const bool CASESENSITIVENAMES = false;
    public const string NULLVALUELITERAL = "NULL";
    public const bool POSITIONALPARAMETERS = false;
    public const string PARAMETERPREFIX = "#";
    public const bool NATIVEPAGING = false;
    public const bool USETERMINATORS = true;
    public const char LEFTTERMINATOR = '[';
    public const char RIGHTTERMINATOR = ']';
    public const bool CASESENSITIVETAGS = false;

    static char ValidateTerminator(char value)
    {
        if (value < ' ') throw new ArgumentException("Invalid terminator.").WithData(value);
        if (value == '.') throw new ArgumentException("Dots are not valid terminators.").WithData(value);
        if (value == ' ') throw new ArgumentException("Spaces are not validterminators.").WithData(value);
        return value;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public Engine()
    {
        CaseSensitiveNames = CASESENSITIVENAMES;
        NullValueLiteral = NULLVALUELITERAL;
        PositionalParameters = POSITIONALPARAMETERS;
        ParameterPrefix = PARAMETERPREFIX;
        NativePaging = NATIVEPAGING;
        UseTerminators = USETERMINATORS;
        LeftTerminator = LEFTTERMINATOR;
        RightTerminator = RIGHTTERMINATOR;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Engine(Engine source)
    {
        source.ThrowWhenNull();

        CaseSensitiveNames = source.CaseSensitiveNames;
        NullValueLiteral = source.NullValueLiteral;
        PositionalParameters = source.PositionalParameters;
        ParameterPrefix = source.ParameterPrefix;
        NativePaging = source.NativePaging;
        UseTerminators = source.UseTerminators;
        LeftTerminator = source.LeftTerminator;
        RightTerminator = source.RightTerminator;
    }

    /// <inheritdoc/>
    public override string ToString() => "ORM.Engine";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(Engine? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        return
            CaseSensitiveNames == other.CaseSensitiveNames &&
            string.Compare(NullValueLiteral, other.NullValueLiteral, !CaseSensitiveNames) == 0 &&
            PositionalParameters == other.PositionalParameters &&
            string.Compare(ParameterPrefix, other.ParameterPrefix, !CaseSensitiveNames) == 0 &&
            NativePaging == other.NativePaging &&
            UseTerminators == other.UseTerminators &&
            LeftTerminator == other.LeftTerminator &&
            RightTerminator == other.RightTerminator;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, CaseSensitiveNames);
        code = HashCode.Combine(code, NullValueLiteral);
        code = HashCode.Combine(code, PositionalParameters);
        code = HashCode.Combine(code, ParameterPrefix);
        code = HashCode.Combine(code, NativePaging);
        code = HashCode.Combine(code, UseTerminators);
        code = HashCode.Combine(code, LeftTerminator);
        code = HashCode.Combine(code, RightTerminator);
        return code;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the identifier-alike names in the database are case sensitive or not.
    /// </summary>
    public bool CaseSensitiveNames { get; init; }

    /// <summary>
    /// The literal used to represent NULL values in database commands.
    /// </summary>
    public string NullValueLiteral { get; init => field = value.NotNullNotEmpty(true); }

    /// <summary>
    /// Determines if the command parameters are positional, rather than named ones.
    /// </summary>
    public bool PositionalParameters { get; init; }

    /// <summary>
    /// The prefix used by this engine to preceed command parameter names.
    /// </summary>
    public string ParameterPrefix { get; init => field = value.NotNullNotEmpty(true); }

    /// <summary>
    /// Determines if the underlying engine provides native paging functionality, or rather it
    /// shall be emulated by the framework.
    /// </summary>
    public bool NativePaging { get; init; }

    /// <summary>
    /// Determines if the identifier-alike names shall be wrapped between terminators, or not.
    /// </summary>
    public bool UseTerminators { get; init; }

    /// <summary>
    /// The left terminator this engine uses if needed.
    /// </summary>
    public char LeftTerminator { get; init => field = ValidateTerminator(value); }

    /// <summary>
    /// The right terminator this engine uses if needed.
    /// </summary>
    public char RightTerminator { get; init => field = ValidateTerminator(value); }
}