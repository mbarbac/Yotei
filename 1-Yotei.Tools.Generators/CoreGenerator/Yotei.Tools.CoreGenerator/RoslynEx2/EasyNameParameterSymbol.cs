namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static partial class RoslynNameExtensions
{
    /// <summary>
    /// Obtains the C#-alike easy name of the given element using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this IParameterSymbol source) => EasyNameParameterSymbol.Default.EasyName(source);

    /// <summary>
    /// Obtains the C#-alike easy name of the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IParameterSymbol source, EasyNameParameterSymbol options)
    {
        options.ThrowWhenNull();
        return options.EasyName(source);
    }
}

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for 'type' instances.
/// </summary>
internal record EasyNameParameterSymbol
{
    /// <summary>
    /// A shared read-only instance that represents empty options.
    /// </summary>
    public static EasyNameParameterSymbol Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared read-only instance that represents default options.
    /// </summary>
    public static EasyNameParameterSymbol Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared read-only instance that represents full options.
    /// </summary>
    public static EasyNameParameterSymbol Full { get; } = new(Mode.Full);

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyNameParameterSymbol() : this(Mode.Default) { }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if parameter modifiers (such as 'in', 'out' and 'ref') shall be used.
    /// </summary>
    public bool UseModifiers { get; init; }

    /// <summary>
    /// If not null, then the options to use with the type of the parameter. If null, then it is
    /// ignored.
    /// </summary>
    public EasyNameTypeSymbol? TypeOptions { get; init; }

    /// <summary>
    /// Determines if the name of the parameter shall be used or not.
    /// </summary>
    public bool UseName { get; init; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    private EasyNameParameterSymbol(Mode mode)
    {
        switch (mode)
        {
            case Mode.Empty:
                break;

            case Mode.Default:
                UseModifiers = true;
                TypeOptions = EasyNameTypeSymbol.Default;
                break;

            case Mode.Full:
                UseModifiers = true;
                TypeOptions = EasyNameTypeSymbol.Full;
                UseName = true;
                break;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the C#-alike easy name of the given element.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public string EasyName(IParameterSymbol source)
    {
        source.ThrowWhenNull();

        var sb = new StringBuilder();

        // Parameter type...
        if (TypeOptions is not null)
        {
            var options = TypeOptions.HideName ? TypeOptions with { HideName = false } : TypeOptions;
            var str = options.EasyName(source.Type);
            if (str.Length > 0) sb.Append(str);

            while (TypeOptions.UseNullability && str.Length > 0 && sb[^1] != '?')
            {
                var type = source.Type;
                var name = type.Name;
                var arity = type is INamedTypeSymbol named ? named.Arity : 0;

                // Special case: nullable wrappers...
                if (TypeOptions.UseNullableWrappers &&
                    arity == 1 && (name == "Nullable" || name == "IsNullable"))
                    break;

                // Nullable annotation...
                if (source.NullableAnnotation == NullableAnnotation.Annotated)
                {
                    sb.Append('?');
                    break;
                }

                // Nullable attribute...
                if (source.GetAttributes()
                    .FirstOrDefault(a => a.AttributeClass?.Name == "NullableAttribute") != null)
                {
                    sb.Append('?');
                    break;
                }

                // End of nullability...
                break;
            }
        }

        // Parameter name...
        if (UseName) sb.Append(source.Name);

        // Parameter modifiers...
        if (UseModifiers && sb.Length > 0)
        {
            var prefix = source.RefKind switch
            {
                RefKind.In => "in ",
                RefKind.Out => "out ",
                RefKind.Ref => "ref ",
                RefKind.RefReadOnlyParameter => "ref readonly ",
                _ => null
            };
            if (prefix is not null) sb.Insert(0, prefix);
        }

        // Finishing...
        return sb.ToString();
    }
}