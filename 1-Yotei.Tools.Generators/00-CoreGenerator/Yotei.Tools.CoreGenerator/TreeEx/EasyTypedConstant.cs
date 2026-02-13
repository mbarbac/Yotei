namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal record EasyTypedConstant
{
    /// <summary>
    /// Use the name of the typed constant, if available.
    /// </summary>
    public bool UseName { get; set; }

    /// <summary>
    /// If not null, the options to use with values that are CLR ones.
    /// If null, then default ones are used.
    /// </summary>
    public EasyNameOptions? ValueClrOptions { get; set; }

    /// <summary>
    /// If not null, the options to use with values that are type symbol ones.
    /// If null, then default ones are used.
    /// </summary>
    public EasyTypeSymbol? ValueTypeSymbolOptions { get; set; }

    /// <summary>
    /// If not null, the options to use with values that are property symbol ones.
    /// If null, then default ones are used.
    /// </summary>
    public EasyPropertySymbol? ValuePropertySymbolOptions { get; set; }

    /// <summary>
    /// If not null, the options to use with values that are field symbol ones.
    /// If null, then default ones are used.
    /// </summary>
    public EasyFieldSymbol? ValueFieldSymbolOptions { get; set; }

    /// <summary>
    /// If not null, the options to use with values that are method symbol ones.
    /// If null, then default ones are used.
    /// </summary>
    public EasyMethodSymbol? ValueMethodSymbolOptions { get; set; }

    /// <summary>
    /// If not null, the options to use with values that are event symbol ones.
    /// If null, then default ones are used.
    /// </summary>
    public EasyEventSymbol? ValueEventSymbolOptions { get; set; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    EasyTypedConstant(Mode mode)
    {
        switch (mode)
        {
            case Mode.Full:
                UseName = true;
                ValueClrOptions = EasyNameOptions.Full;
                ValueTypeSymbolOptions = EasyTypeSymbol.Full;
                ValuePropertySymbolOptions = EasyPropertySymbol.Full;
                ValueFieldSymbolOptions = EasyFieldSymbol.Full;
                ValueMethodSymbolOptions = EasyMethodSymbol.Full;
                ValueEventSymbolOptions = EasyEventSymbol.Full;
                break;

            case Mode.Default:
                ValueClrOptions = EasyNameOptions.Default;
                ValueTypeSymbolOptions = EasyTypeSymbol.Default;
                ValuePropertySymbolOptions = EasyPropertySymbol.Default;
                ValueFieldSymbolOptions = EasyFieldSymbol.Default;
                ValueMethodSymbolOptions = EasyMethodSymbol.Default;
                ValueEventSymbolOptions = EasyEventSymbol.Default;
                break;
        }
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyTypedConstant() : this(Mode.Empty) { }

    /// <summary>
    /// A shared instance with default-alike settings.
    /// </summary>
    public static EasyTypedConstant Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared instance with full-alike settings.
    /// </summary>
    public static EasyTypedConstant Full { get; } = new(Mode.Full);
}

// ========================================================
internal static partial class EasyNameExtensions
{
    /// <summary>
    /// Returns a display string for the given element using default options. The optional name
    /// is included only if the options enable so.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string EasyName(
        this TypedConstant source,
        string? name = null)
        => source.EasyName(EasyTypedConstant.Default, name);

    /// <summary>
    /// Returns a display string for the given element using the given options. The optional name
    /// is included only if the options enable so.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string EasyName(
        this TypedConstant source, EasyTypedConstant options, string? name = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();
        if (source.Type is null) throw new ArgumentException("Element's type is null.").WithData(source);

        // Constant's name...
        if (name != null && options.UseName) sb.Append(name).Append(" = ");

        // The actual value...
        if (source.IsNull) sb.Append("null");
        else
        {
            if (source.Kind == TypedConstantKind.Array) // Array values...
            {
                sb.Append('['); for (int i = 0; i < source.Values.Length; i++)
                {
                    if (i > 0) sb.Append(", ");
                    var str = source.Values[i].EasyName(options);
                    sb.Append(str);
                }
                sb.Append(']');
            }
            else // Regular (non-array) values...
            {
                var str = TypedConstantValue(source.Value, options);
                sb.Append(str);
            }
        }

        // Finishing...
        return sb.ToString();
    }

    /// <summary>
    /// Invoked to obtain the representation of the actual value of the typed constant.
    /// </summary>
    /// <returns></returns>
    static string TypedConstantValue(object? value, EasyTypedConstant options) => value switch
    {
        Type item => $"typeof({item.EasyName(options.ValueClrOptions ?? EasyNameOptions.Default)})",
        MethodInfo item => item.EasyName(options.ValueClrOptions ?? EasyNameOptions.Default),
        PropertyInfo item => item.EasyName(options.ValueClrOptions ?? EasyNameOptions.Default),
        FieldInfo item => item.EasyName(options.ValueClrOptions ?? EasyNameOptions.Default),

        ITypeSymbol item => $"typeof({item.EasyName(options.ValueTypeSymbolOptions ?? EasyTypeSymbol.Default)})",
        IPropertySymbol item => item.EasyName(options.ValuePropertySymbolOptions ?? EasyPropertySymbol.Default),
        IFieldSymbol item => item.EasyName(options.ValueFieldSymbolOptions ?? EasyFieldSymbol.Default),
        IMethodSymbol item => item.EasyName(options.ValueMethodSymbolOptions ?? EasyMethodSymbol.Default),
        IEventSymbol item => item.EasyName(options.ValueEventSymbolOptions ?? EasyEventSymbol.Default),

        bool item => item ? "true" : "false",
        char item => item.ToString(),
        string item => item,
        sbyte item => item.ToString(),
        byte item => item.ToString(),
        short item => item.ToString(),
        ushort item => item.ToString(),
        int item => item.ToString(),
        uint item => item.ToString(),
        long item => item.ToString(),
        ulong item => item.ToString(),
        decimal item => item.ToString(),
        float item => item.ToString(),
        double item => item.ToString(),

        null => "null",
        _ => value?.ToString() ?? string.Empty
    };
}