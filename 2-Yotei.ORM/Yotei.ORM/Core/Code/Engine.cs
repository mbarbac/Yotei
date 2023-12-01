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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => "ORM.Engine";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool CaseSensitiveNames { get; init; } = CASESENSITIVENAMES;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string NullValueLiteral
    {
        get => _NullValueLiteral;
        init => _NullValueLiteral = value.NotNullNotEmpty();
    }
    string _NullValueLiteral = NULLVALUELITERAL;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool NativePaging { get; init; } = NATIVEPAGING;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string ParameterPrefix
    {
        get => _ParameterPrefix;
        init => _ParameterPrefix = value.NotNullNotEmpty();
    }
    string _ParameterPrefix = PARAMETERPREFIX;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool PositionalParameters { get; init; } = POSITIONALPARAMETERS;

    /// <summary>
    /// <inheritdoc/>
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
    /// <inheritdoc/>
    /// </summary>
    public char LeftTerminator
    {
        get => _LeftTerminator;
        init => _LeftTerminator = ValidateTerminator(value);
    }
    char _LeftTerminator = LEFTERMINATOR;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public char RightTerminator
    {
        get => _RightTerminator;
        init => _RightTerminator = ValidateTerminator(value);
    }
    char _RightTerminator = RIGHTTERMINATOR;



    /// <summary>
    /// The collection of metadata tags that are well-known to this engine, for the purposes
    /// of the framework.
    /// </summary>
    public IKnownTags KnownTags
    {
        get => _KnownTags;
        set => _KnownTags = value.ThrowWhenNull();
    }
    IKnownTags _KnownTags = new KnownTags(CASESENSITIVETAGS);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="ch"></param>
    /// <returns></returns>
    public List<int> GetUnwrappedIndexes(string? value, char ch)
    {
        var items = new List<int>();
        var deep = 0;

        // Obvious case...
        if (value is null) return items;

        // No terminators used...
        if (!UseTerminators)
        {
            for (int i = 0; i < value.Length; i++) if (value[i] == ch) items.Add(i);
            return items;
        }

        // Terminators are different characters...
        if (LeftTerminator != RightTerminator)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == LeftTerminator) { deep++; continue; }
                if (value[i] == RightTerminator) { deep--; if (deep < 0) deep = 0; continue; }
                if (value[i] == ch && deep == 0) items.Add(i);
            }
            return items;
        }

        // Terminators are the same character...
        else
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == LeftTerminator) { deep = deep == 0 ? 1 : 0; continue; }
                if (value[i] == ch && deep == 0) items.Add(i);
            }
            return items;
        }
    }
}