using System.Xml.Schema;

namespace Yotei.Tools;

// ========================================================
public static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains the C#-alike easy name of the given element using default options.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string EasName(
        this Type type) => TypeEasyName.Default.EasyName(type);

    /// <summary>
    /// Obtains the C#-alike easy name of the given element using the given options.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasName(this Type type, TypeEasyName options)
    {
        options.ThrowWhenNull();
        return options.EasyName(type);
    }
}

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for <see cref="Type"/> instances.
/// </summary>
public record TypeEasyName
{
    /// <summary>
    /// A shared read-only instance that represents empty options.
    /// </summary>
    public static TypeEasyName Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared read-only instance that represents default options.
    /// </summary>
    public static TypeEasyName Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared read-only instance that represents full options.
    /// </summary>
    public static TypeEasyName Full { get; } = new(Mode.Full);

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public TypeEasyName() : this(Mode.Default) { }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the 'in' and 'out' variance specifiers shall be used or not.
    /// <br/> This setting is only used with generic-alike types.
    /// <br/> The value of this property is 'false' by default.
    /// </summary>
    public bool UseVarianceMask { get; init; }

    /// <summary>
    /// Determines if the namespace of the type shall be used.
    /// <br/> The value of this property is 'false' by default.
    /// </summary>
    public bool UseNamespace { get; init; }

    /// <summary>
    /// Determines if the host of the type shall be used.
    /// <br/> The value of this property is 'false' by default.
    /// </summary>
    public bool UseHost { get; init; }

    /// <summary>
    /// Determines if the name of the type shall be hidden.
    /// <br/> The value of this property is 'false' by default. When used, then it shortcuts
    /// any other setting.
    /// </summary>
    public bool HideName { get; init; }

    /// <summary>
    /// Determines if the nullability annotation of the type element shall be used.
    /// <br/> The value of this property is 'true' by default.
    /// <br/> For reference types, or generic ones, nullable annotations are just syntactic sugar
    /// used by the compiler but, in most circumstances, not persisted in metadata or in custom
    /// attributes. The <see cref="IsNullable{T}"/> type can be used as a workaround when this
    /// metadata-alike must be specified somehow.
    /// <br/> By contrast, value types are translated to <see cref="Nullable{T}"/> instances.
    /// </summary>
    public bool UseNullability { get; init; }

    /// <summary>
    /// Determines if the generic type arguments of the type shall be used, or not.
    /// <br/> The value of this property is 'true' by default.
    /// </summary>
    public bool UseGenerics { get; init; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    private TypeEasyName(Mode mode)
    {
        switch (mode)
        {
            case Mode.Empty:
                HideName = true;
                break;

            case Mode.Default:
                UseNullability = true;
                UseGenerics = true;
                break;

            case Mode.Full:
                UseVarianceMask = true;
                UseNamespace = true;
                UseHost = true;
                UseNullability = true;
                UseGenerics = true;
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
    /// Invoked to obtain the C#-alike name of the given type, after its closed generic type
    /// arguments have been obtained. Otherwise, the detailed information is lost when asking
    /// in a recursive fashion.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    string EasyName(Type source, Type[] types)
    {
        var isgen = source.FullName == null;
        var host = source.DeclaringType;

        var args = source.GetGenericArguments();
        var used = host is null ? 0 : host.GetGenericArguments().Length;
        var need = args.Length - used;

        // Shortcut hide name...
        if (HideName) return string.Empty;

        // Shortcut explicit nullable types...
        if (UseNullability && (IsNullableValueType() || IsNullableFakedType()))
        {
            var str = EasyName(args[0]);
            if (!str.EndsWith('?')) str += '?';
            return str;
        }
        bool IsNullableValueType() => source.Name.StartsWith("Nullable`1") && args.Length == 1;
        bool IsNullableFakedType() => source.Name.StartsWith("IsNullable`1") && args.Length == 1;

        // Standard case...
        var sb = new StringBuilder();

        // Variance attributes...
        if (UseVarianceMask && source.IsGenericParameter)
        {
            var gpa = source.GenericParameterAttributes;
            var variance = gpa & GenericParameterAttributes.VarianceMask;

            if ((variance & GenericParameterAttributes.Covariant) != 0) sb.Append("out ");
            if ((variance & GenericParameterAttributes.Contravariant) != 0) sb.Append("in ");
        }

        // Namespace...
        if (UseNamespace && !isgen && host is null)
        {
            var str = source.Namespace;
            if (str is not null && str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Host...
        if ((UseHost || UseNamespace) && !isgen && host is not null)
        {
            // Using 'types' to prevent loosing bound information...
            var str = EasyName(host, types);
            if (str is not null && str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Name& is a 'ref' type, which shall be intercepted in a parameterinfo instance...
        // Name...
        var name = source.Name;
        var index = name.IndexOf('`'); if (index >= 0) name = name[..index];
        if (name.EndsWith('&')) name = name[..^1];
        sb.Append(name);

        // Generic arguments...
        if (need > 0 && UseGenerics)
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

        // Finishing...
        return sb.ToString();
    }
}