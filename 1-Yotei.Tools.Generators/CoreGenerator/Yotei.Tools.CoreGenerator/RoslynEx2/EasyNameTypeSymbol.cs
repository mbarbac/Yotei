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
        this ITypeSymbol source) => EasyNameTypeSymbol.Default.EasyName(source);

    /// <summary>
    /// Obtains the C#-alike easy name of the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ITypeSymbol source, EasyNameTypeSymbol options)
    {
        options.ThrowWhenNull();
        return options.EasyName(source);
    }
}

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for 'type symbol' instances.
/// </summary>
internal record EasyNameTypeSymbol
{
    /// <summary>
    /// A shared read-only instance that represents empty options.
    /// </summary>
    public static EasyNameTypeSymbol Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared read-only instance that represents default options.
    /// </summary>
    public static EasyNameTypeSymbol Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared read-only instance that represents full options.
    /// </summary>
    public static EasyNameTypeSymbol Full { get; } = new(Mode.Full);

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyNameTypeSymbol() : this(Mode.Default) { }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the 'in' and 'out' variance specifiers shall be used or not.
    /// </summary>
    public bool UseVarianceMask { get; init; }

    /// <summary>
    /// Determines if the namespace of the type shall be used.
    /// </summary>
    public bool UseNamespace { get; init; }

    /// <summary>
    /// Determines if the host of the type shall be used.
    /// </summary>
    public bool UseHost { get; init; }

    /// <summary>
    /// Determines if the name of the type shall be hidden, and shortcuts any other setting.
    /// </summary>
    public bool HideName { get; init; }

    /// <summary>
    /// Determines if the nullability annotation of the type, if any, shall be used.
    /// </summary>
    public bool UseNullability { get; init; }

    /// <summary>
    /// Determines if the <see cref="Nullable{T}"/> and <see cref="IsNullable{T}"/> nullable
    /// wrappers shall be printed, or print the annotated wrapped type instead.
    /// </summary>
    public bool UseNullableWrappers { get; init; }

    /// <summary>
    /// Determines if the generic type arguments of the type shall be used.
    /// </summary>
    public bool UseGenericArguments { get; init; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    private EasyNameTypeSymbol(Mode mode)
    {
        switch (mode)
        {
            case Mode.Empty:
                HideName = true;
                break;

            case Mode.Default:
                UseNullability = true;
                break;

            case Mode.Full:
                UseVarianceMask = true;
                UseNamespace = true;
                UseHost = true;
                UseNullability = true;
                UseNullableWrappers = true;
                UseGenericArguments = true;
                break;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the source is a namespace.
    /// </summary>
    string EasyName(INamespaceSymbol source)
    {
        List<string> names = [];

        ISymbol? node = source;
        while (node != null)
        {
            if (node is INamespaceSymbol ns &&
                ns.Name != null &&
                ns.Name.Length > 0) names.Add(ns.Name);

            if (!UseNamespace) break; // But at least done once!
            node = node.ContainingSymbol;
        }

        if (names.Count == 0) return string.Empty;

        names.Reverse();
        return string.Join(".", names);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the C#-alike easy name of the given element.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public string EasyName(ITypeSymbol source)
    {
        source.ThrowWhenNull();

        // Shortcuts...
        if (HideName) return string.Empty;
        if (source is INamespaceSymbol nsx) return EasyName(nsx);

        // Capturing...
        var isgen = source.TypeKind == TypeKind.TypeParameter;
        var host = source.ContainingType;
        var named = source as INamedTypeSymbol;
        var args = named is null ? [] : named.TypeArguments;

        // Shortcut wrapped nullability...
        if (UseNullability && !UseNullableWrappers && args.Length == 1)
        {
            if (source.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T ||
                source.OriginalDefinition.Name == "IsNullable")
            {
                var type = args[0];
                var str = EasyName(type); if (!str.EndsWith('?')) str += '?';
                return str;
            }
        }

        // Processing...
        var sb = new StringBuilder();

        // Variance mask...
        if (UseVarianceMask && source is ITypeParameterSymbol par)
        {
            switch (par.Variance)
            {
                case VarianceKind.Out: sb.Append("out "); break;
                case VarianceKind.In: sb.Append("in "); break;
            }
        }

        // Namespace...
        if (UseNamespace && host is null && !isgen)
        {
            var ns = source.ContainingNamespace;
            var str = EasyName(ns);
            if (str is not null && str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Host...
        if ((UseHost || UseNamespace) && host is not null && !isgen)
        {
            var options = HideName ? this with { HideName = false } : this;

            var str = options.EasyName(host);
            if (str is not null && str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Name...
        var name = source.Name;
        sb.Append(name);

        // Generic arguments...
        if (UseGenericArguments && args.Length > 0)
        {
            sb.Append('<'); for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var str = EasyName(arg);
                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append('>');
        }

        // Nullable annotations...
        if (UseNullability && source.NullableAnnotation == NullableAnnotation.Annotated)
            if (sb.Length > 0 && sb[^1] != '?') sb.Append('?');

        // Decorated nullability...
        if (source.GetAttributes()
            .FirstOrDefault(x => x.AttributeClass?.Name == "IsNullableAttribute") != null)
            if (sb.Length > 0 && sb[^1] != '?') sb.Append('?');

        // Finishing...
        return sb.ToString();
    }
}