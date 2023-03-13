namespace Yotei.ORM.Code;
/*
// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
public record Engine : IEngine
{
    public const bool DEFAULT_CASE_SENSITIVE_NAMES = false;
    public const string DEFAULT_NULL_VALUE_STRING = "NULL";
    public const bool DEFAULT_SUPPORTS_NATIVE_SKIP_TAKE = false;
    public const string DEFAULT_PARAMETER_PREFIX = "#";
    public const bool DEFAULT_USE_TERMINATORS = true;
    public const char DEFAULT_LEFT_TERMINATOR = '[';
    public const char DEFAULT_RIGHT_TERMINATOR = ']';

    // ----------------------------------------------------

    /// <summary>
    /// Returns a default instance.
    /// </summary>
    public Engine() { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected Engine(Engine other)
    {
        other = other.ThrowIfNull();

        CaseSensitiveNames = other.CaseSensitiveNames;
        NullValueString = other.NullValueString;
        SupportsNativeSkipTake = other.SupportsNativeSkipTake;
        ParameterPrefix = other.ParameterPrefix;
        UseTerminators = other.UseTerminators;
        LeftTerminator = other.LeftTerminator;
        RightTerminator = other.RightTerminator;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => "ORM.Engine";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool CaseSensitiveNames { get; init; } = DEFAULT_CASE_SENSITIVE_NAMES;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string NullValueString
    {
        get => _NullValueString;
        init => _NullValueString = value.NotNullNotEmpty();
    }
    string _NullValueString = DEFAULT_NULL_VALUE_STRING;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool SupportsNativeSkipTake { get; init; } = DEFAULT_SUPPORTS_NATIVE_SKIP_TAKE;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string ParameterPrefix
    {
        get => _ParameterPrefix;
        init => _ParameterPrefix = value.NotNullNotEmpty();
    }
    string _ParameterPrefix = DEFAULT_PARAMETER_PREFIX;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool UseTerminators { get; init; } = DEFAULT_USE_TERMINATORS;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public char LeftTerminator
    {
        get => _LeftTerminator;
        init => _LeftTerminator = ValidateTerminator(value);
    }
    char _LeftTerminator = DEFAULT_LEFT_TERMINATOR;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public char RightTerminator
    {
        get => _RightTerminator;
        init => _RightTerminator = ValidateTerminator(value);
    }
    char _RightTerminator = DEFAULT_RIGHT_TERMINATOR;

    // ----------------------------------------------------

    /// <summary>
    /// Returns a validated terminator, or throws an exception.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private char ValidateTerminator(char value)
    {
        return _ValidTerminators.Contains(value)
            ? value
            : throw new ArgumentException($"Invalid terminator: ({(int)value}) '{value}'");
    }
    private const string _ValidTerminators = "[]()´";
}*/