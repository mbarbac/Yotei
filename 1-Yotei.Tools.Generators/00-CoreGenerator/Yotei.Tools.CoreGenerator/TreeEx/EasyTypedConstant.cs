namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal record EasyTypedConstant
{
    /// <summary>
    /// Use the name of the typed constant, if available.
    /// </summary>
    public bool UseName { get; set; }

    /// <summary>
    /// If not null, the options to include the type of the typed constant, between brackets. If
    /// null, it is ignored.
    /// </summary>
    public EasyTypeSymbol? ReturnTypeOptions { get; set; }

    /// <summary>
    /// If not null, the options to use with elements that are CLR ones.
    /// If null, then default ones are used.
    /// </summary>
    public EasyNameOptions? ClrValueOptions { get; set; }

    /// <summary>
    /// If not null, the options to use with elements that are type symbol ones.
    /// If null, then default ones are used.
    /// </summary>
    public EasyTypeSymbol? TypeSymbolValueOptions { get; set; }

    /// <summary>
    /// If not null, the options to use with elements that are property symbol ones.
    /// If null, then default ones are used.
    /// </summary>
    public EasyPropertySymbol? PropertySymbolValueOptions { get; set; }

    /// <summary>
    /// If not null, the options to use with elements that are field symbol ones.
    /// If null, then default ones are used.
    /// </summary>
    public EasyFieldSymbol? FieldSymbolValueOptions { get; set; }

    /// <summary>
    /// If not null, the options to use with elements that are method symbol ones.
    /// If null, then default ones are used.
    /// </summary>
    public EasyMethodSymbol? MethodSymbolValueOptions { get; set; }

    /// <summary>
    /// If not null, the options to use with elements that are event symbol ones.
    /// If null, then default ones are used.
    /// </summary>
    public EasyEventSymbol? EventSymbolValueOptions { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with a set of default code generation settings.
    /// </summary>
    public static EasyTypedConstant Default => new()
    {
        ReturnTypeOptions = EasyTypeSymbol.Default,
        ClrValueOptions = EasyNameOptions.Default,
        TypeSymbolValueOptions = EasyTypeSymbol.Default,
        PropertySymbolValueOptions = EasyPropertySymbol.Default,
        FieldSymbolValueOptions = EasyFieldSymbol.Default,
        MethodSymbolValueOptions = EasyMethodSymbol.Default,
        EventSymbolValueOptions = EasyEventSymbol.Default,
    };

    /// <summary>
    /// Returns a new instance with full settings.
    /// </summary>
    public static EasyTypedConstant Full => new()
    {
        UseName = true,
        ReturnTypeOptions = EasyTypeSymbol.Full,
        ClrValueOptions = EasyNameOptions.Full,
        TypeSymbolValueOptions = EasyTypeSymbol.Full,
        PropertySymbolValueOptions = EasyPropertySymbol.Full,
        FieldSymbolValueOptions = EasyFieldSymbol.Full,
        MethodSymbolValueOptions = EasyMethodSymbol.Full,
        EventSymbolValueOptions = EasyEventSymbol.Full,
    };
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
        if (name != null && options.UseName) sb.Append(name).Append(": ");

        // Constant's type...
        if (options.ReturnTypeOptions != null && source.Type != null)
        {
            var xoptions = options.ReturnTypeOptions with { HideName = false };
            var str = source.Type.EasyName(xoptions);
            sb.Append('(').Append(str);
            if (source.Kind == TypedConstantKind.Array && !str.EndsWith(']')) sb.Append("[]");
            sb.Append(") ");
        }

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
        Type item => item.EasyName(options.ClrValueOptions ?? EasyNameOptions.Default),
        MethodInfo item => item.EasyName(options.ClrValueOptions ?? EasyNameOptions.Default),
        PropertyInfo item => item.EasyName(options.ClrValueOptions ?? EasyNameOptions.Default),
        FieldInfo item => item.EasyName(options.ClrValueOptions ?? EasyNameOptions.Default),

        ITypeSymbol item => item.EasyName(options.TypeSymbolValueOptions ?? EasyTypeSymbol.Default),
        IPropertySymbol item => item.EasyName(options.PropertySymbolValueOptions ?? EasyPropertySymbol.Default),
        IFieldSymbol item => item.EasyName(options.FieldSymbolValueOptions ?? EasyFieldSymbol.Default),
        IMethodSymbol item => item.EasyName(options.MethodSymbolValueOptions ?? EasyMethodSymbol.Default),
        IEventSymbol item => item.EasyName(options.EventSymbolValueOptions ?? EasyEventSymbol.Default),

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