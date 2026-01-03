namespace Yotei.Tools;

// ========================================================
public static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains the C#-alike easy name of the given element using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this Type source) => EasyNameTypeOptions.Default.EasyName(source);

    /// <summary>
    /// Obtains the C#-alike easy name of the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this Type source, EasyNameTypeOptions options)
    {
        options.ThrowWhenNull();
        return options.EasyName(source);
    }
}

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for <see cref="Type"/> instances.
/// <br/> Default instances just obtain the name of the type with its nullable annotation, if any.
/// </summary>
public record EasyNameTypeOptions
{
    /// <summary>
    /// A shared read-only instance that represents default options.
    /// </summary>
    public static EasyNameTypeOptions Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared read-only instance that represents full options.
    /// </summary>
    public static EasyNameTypeOptions Full { get; } = new(Mode.Full);

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyNameTypeOptions() : this(Mode.Default) { }

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

    enum Mode { Default, Full };
    private EasyNameTypeOptions(Mode mode)
    {
        switch (mode)
        {
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
    /// Obtains the C#-alike easy name of the given element.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public string EasyName(Type source)
    {
        source.ThrowWhenNull();

        var types = source.GetGenericArguments();
        return EasyName(source, types);
    }

    /// <summary>
    /// Invoked to obtain the C#-alike name of the given type, once its closed generic arguments,
    /// if any, have been captured to prevent loosing them. These are the ones in the source along
    /// with all the ones in its inheritance chain.
    /// </summary>
    string EasyName(Type source, Type[] types)
    {
        var host = source.DeclaringType;
        var args = source.GetGenericArguments();
        var used = host is null ? 0 : host.GetGenericArguments().Length;
        var need = args.Length - used;

        // Shortcut hide name...
        if (HideName) return string.Empty;

        // Shortcut not decorated nullability...
        if (UseNullability && !UseNullableWrappers &&
            source.GetCustomAttribute<IsNullableAttribute>() == null)
        {
            if ((need == 1 && source.Name.StartsWith("Nullable`1")) ||
                (need == 1 && source.Name.StartsWith("IsNullable`1")))
            {
                var type = types[used];
                var str = EasyName(type); if (!str.EndsWith('?')) str += '?';
                return str;
            }
        }

        // Processing...
        var sb = new StringBuilder();

        // Variance mask...
        if (UseVarianceMask && source.IsGenericParameter)
        {
            var gpa = source.GenericParameterAttributes;
            var variance = gpa & GenericParameterAttributes.VarianceMask;

            if ((variance & GenericParameterAttributes.Covariant) != 0) sb.Append("out ");
            if ((variance & GenericParameterAttributes.Contravariant) != 0) sb.Append("in ");
        }

        // Namespace...
        if (UseNamespace && host is null)
        {
            var str = source.Namespace;
            if (str is not null && str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Host...
        if ((UseHost || UseNamespace) && host is not null)
        {
            // Using 'types' to prevent loosing bound information...
            var str = EasyName(host, types);
            if (str is not null && str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Name (name& is a ref type, 'ref' to be intercepted elsewhere)...
        var name = source.Name;
        var index = name.IndexOf('`'); if (index >= 0) name = name[..index];
        if (name.EndsWith('&')) name = name[..^1];
        sb.Append(name);

        // Generic arguments...
        if (UseGenericArguments && need > 0)
        {
            sb.Append('<'); for (int i = 0; i < need; i++)
            {
                var arg = types[i + used];
                var str = EasyName(arg);
                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append('>');
        }

        // Decorated nullability...
        if (UseNullability && !UseNullableWrappers &&
            source.GetCustomAttribute<IsNullableAttribute>() != null)
            if (sb.Length > 0 && sb[^1] != '?') sb.Append('?');

        // Finishing...
        return sb.ToString();
    }
}