namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IEngine"/>
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
        NativePaging = source.NativePaging;
        ParameterPrefix = source.ParameterPrefix;
        PositionalParameters = source.PositionalParameters;
        UseTerminators = source.UseTerminators;
        LeftTerminator = source.LeftTerminator;
        RightTerminator = source.RightTerminator;
        KnownTags = source.KnownTags;
    }

    /// <inheritdoc/>
    public override string ToString() => "ORM.Engine";

    // ----------------------------------------------------

    public bool CaseSensitiveNames { get; init; } = CASESENSITIVENAMES;

    /// <inheritdoc/>
    public string NullValueLiteral
    {
        get => _NullValueLiteral;
        init => _NullValueLiteral = value.NotNullNotEmpty();
    }
    string _NullValueLiteral = NULLVALUELITERAL;

    /// <inheritdoc/>
    public bool NativePaging { get; init; } = NATIVEPAGING;

    /// <inheritdoc/>
    public string ParameterPrefix
    {
        get => _ParameterPrefix;
        init => _ParameterPrefix = value.NotNullNotEmpty();
    }
    string _ParameterPrefix = PARAMETERPREFIX;

    /// <inheritdoc/>
    public bool PositionalParameters { get; init; } = POSITIONALPARAMETERS;

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
    char _LeftTerminator = LEFTERMINATOR;

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

    // ----------------------------------------------------

    /// <inheritdoc/>
    public List<int> UnwrappedIndexes(string? value, char ch)
    {
        var nums = new List<int>();
        var deep = 0;

        // Obvious case...
        if (value is null) return nums;

        // No terminators...
        if (!UseTerminators)
        {
            for (int i = 0; i < value.Length; i++) if (value[i] == ch) nums.Add(i);
            return nums;
        }

        // Left and Right terminators are the same...
        if (LeftTerminator == RightTerminator)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == LeftTerminator) { deep = deep == 0 ? 1 : 0; continue; }
                if (value[i] == ch && deep == 0) nums.Add(i);
            }
            return nums;
        }

        // Different terminators...
        else
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == LeftTerminator) { deep++; continue; }
                if (value[i] == RightTerminator) { deep--; if (deep < 0) deep = 0; continue; }
                if (value[i] == ch && deep == 0) nums.Add(i);
            }
            return nums;
        }
    }
}