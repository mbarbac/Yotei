#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.Generators;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// Describes how to obtain the display string of a property element.
/// </summary>
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
record EasyPropertyOptions
{
    /// <summary>
    /// If enabled, then use the member accessibility modifiers, if any. Otherwise, they are
    /// ignored.
    /// </summary>
    public bool UseAccessibility { get; init; }

    /// <summary>
    /// If enabled and accesibility is used, then also use the 'private' modifier. In all other
    /// cases, it is ignored.
    /// </summary>
    public bool UsePrivate { get; init; }

    /// <summary>
    /// If enabled, then use the element's modifiers, if possible. Otherwise, they are ignored.
    /// </summary>
    public bool UseModifiers { get; init; }

    /// <summary>
    /// If not null, then the options to use with the element's return type. Otherwise, it is
    /// ignored.
    /// </summary>
    public EasyTypeOptions? MemberTypeOptions { get; init; }

    /// <summary>
    /// If not null, then the options to use with the element's host type. Otherwise, it is
    /// ignored.
    /// </summary>
    public EasyTypeOptions? HostTypeOptions { get; init; }

    /// <summary>
    /// If enabled, and if the property is an indexed one, then use the internal CLR name. If not,
    /// then use 'this' as the name of the indexed property.
    /// </summary>
    public bool UseTechName { get; init; }

    /// <summary>
    /// If enabled, and if the property is an indexed one, then use square brackets after the name
    /// even if no parameters options were specified.
    /// </summary>
    public bool UseBrackets { get; init; }

    /// <summary>
    /// If not null, and if the property is an indexed one, then the options to use with its
    /// parameters. If null, then they are ignored.
    /// <br/> A not-null value of this property implies <see cref="UseBrackets"/>.
    /// </summary>
    public EasyParameterOptions? ParameterOptions { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyPropertyOptions() { }

    /// <summary>
    /// A shared instance with empty-alike settings.
    /// </summary>
    public static EasyPropertyOptions Empty { get; } = new();

    /// <summary>
    /// A shared instance with default-alike settings.
    /// </summary>
    public static EasyPropertyOptions Default { get; } = new()
    {
        ParameterOptions = EasyParameterOptions.Default,
    };

    /// <summary>
    /// A shared instance with full-alike settings.
    /// </summary>
    public static EasyPropertyOptions Full { get; } = new()
    {
        UseAccessibility = true,
        UsePrivate = true,
        UseModifiers = true,
        MemberTypeOptions = EasyTypeOptions.Full,
        HostTypeOptions = EasyTypeOptions.Full,
        UseTechName = true,
        ParameterOptions = EasyParameterOptions.Full,
    };
}

// ========================================================
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this PropertyInfo source) => source.EasyName(EasyPropertyOptions.Default);

    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(
        this PropertyInfo source, EasyPropertyOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();
        var host = source.DeclaringType;
        var iface = host != null && host.IsInterface;
        var method = source.GetGetMethod() ?? source.GetSetMethod();

        // Accessibility (not using 'private' for simplicity)...
        if (options.UseAccessibility && method != null)
        {
            if (method.IsPrivate && options.UsePrivate) sb.Append("private ");
            if (method.IsPublic && !iface) sb.Append("public ");
            if (method.IsFamily) sb.Append("protected ");
            if (method.IsAssembly) sb.Append("internal ");
            if (method.IsFamilyOrAssembly) sb.Append("internal protected ");
            if (method.IsFamilyAndAssembly) sb.Append("private protected ");
        }

        // Modifiers...
        if (options.UseModifiers && method != null)
        {
            if (method.IsStatic) sb.Append("static ");

            if (method.IsAbstract && !iface) sb.Append("abstract ");
            if (IsSealed()) sb.Append("sealed ");

            if (IsOverride()) sb.Append("override ");
            else if (IsVirtual() && !iface && !method.IsAbstract) sb.Append("virtual ");
            else if (IsNew()) sb.Append("new ");
        }

        // Member type...
        if (options.MemberTypeOptions != null)
        {
            // 'ref'-alike return types...
            if (options.UseModifiers)
            {
                if (source.PropertyType.IsByRef) // This should be the default way...
                {
                    var getter = source.GetGetMethod();
                    if (getter != null && getter.ReturnType.IsByRef)
                    {
                        var ronly =
                            getter.ReturnTypeCustomAttributes.HasReadOnlyAttribute() ||
                            source.HasReadOnlyAttribute();

                        sb.Append(ronly ? "ref readonly " : "ref ");
                    }
                }
                else if (method != null && method.ReturnType.IsByRef) // Fall-back...
                {
                    var ronly = method.ReturnTypeCustomAttributes.HasReadOnlyAttribute();
                    sb.Append(ronly ? "ref readonly " : "ref ");
                }
            }

            // The type itself...
            var xoptions = options.MemberTypeOptions;
            var type = source.PropertyType;
            var str = type.EasyName(xoptions);

            // Nullability...
            while (xoptions.NullableStyle != EasyNullableStyle.None && str.Length > 0)
            {
                if (xoptions.NullableStyle == EasyNullableStyle.KeepWrappers &&
                    type.IsNullableWrapper())
                    break;

                if (str.Length > 0 && str[^1] != '?')
                {
                    if (type.IsNullableAnnotated()) { str += '?'; break; }
                    if (source.IsNullableAnnotated()) { str += '?'; break; }
                }

                break;
            }

            // Adding...
            if (str.Length > 0) sb.Append(str).Append(' ');
        }

        // Host type...
        if (options.HostTypeOptions != null && host != null)
        {
            var xoptions = options.HostTypeOptions;
            var str = host.EasyName(xoptions);
            if (str.Length > 0) sb.Append(str).Append('.');
        }

        // Name...
        var name = source.Name;
        var args = source.GetIndexParameters();
        if (args.Length > 0 && !options.UseTechName) name = "this";
        sb.Append(name);

        // Parameters...
        if (args.Length > 0 && (options.UseBrackets || options.ParameterOptions != null))
        {
            sb.Append('['); if (options.ParameterOptions != null)
            {
                var xoptions = options.ParameterOptions;
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(xoptions);

                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
            }
            sb.Append(']');
        }

        // Finishing...
        return sb.ToString();

        // ------------------------------------------------

        /// <summary>
        /// Determines if the given source is a 'virtual' one.
        /// </summary>
        bool IsVirtual() => method != null && method.IsVirtual && !method.IsFinal;

        /// <summary>
        /// Determines if the given source method (not constructor) is an 'sealed' one.
        /// </summary>
        bool IsSealed() => method != null && method.IsVirtual && method.IsFinal;

        /// <summary>
        /// Determines if the given source method (not constructor) is an 'override' one.
        /// </summary>
        bool IsOverride() =>
            method != null &&
            method.IsVirtual &&
            method.GetBaseDefinition().DeclaringType != source.DeclaringType;

        /// <summary>
        /// Determines if the given source method (not constructor) is a 'new' one.
        /// </summary>
        bool IsNew()
        {
            if (method == null) return false;
            return iface
                ? (method.IsVirtual && FindBaseMethod(host, true) != null)
                : (!method.IsVirtual && FindBaseMethod(host) != null);
        }

        MethodInfo? FindBaseMethod(Type? host, bool ifaces = false)
        {
            if (host != null)
            {
                // First, the host's base types...
                var parent = host.BaseType;
                while (parent != null)
                {
                    var temp = FindMethodAt(parent);
                    if (temp != null) return temp;
                    parent = parent.BaseType;
                }

                // Then the host's interfaces, if requested...
                if (ifaces)
                {
                    foreach (var iface in host.GetInterfaces())
                    {
                        var temp = FindMethodAt(iface);
                        if (temp != null) return temp;
                    }
                }
            }
            return null;

            MethodInfo? FindMethodAt(Type type) => type.GetMethod(
                method.Name,
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.Static,
                null,
                [.. method.GetParameters().Select(p => p.ParameterType)],
                null);
        }
    }
}