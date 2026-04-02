#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.Generators;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// Describes how to obtain the display string of a method or constructor element.
/// </summary>
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
record EasyMethodOptions
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
    public EasyTypeOptions? ReturnTypeOptions { get; init; }

    /// <summary>
    /// If not null, then the options to use with the element's host type. Otherwise, it is
    /// ignored.
    /// </summary>
    public EasyTypeOptions? HostTypeOptions { get; init; }

    /// <summary>
    /// If enabled, and if the method represents a constructor, then use the method's CLR name
    /// as well. If not, just use the host's name.
    /// </summary>
    public bool UseTechName { get; init; }

    /// <summary>
    /// If not null, then the options to use with the member's generic arguments, if any. If
    /// null, then they are ignored.
    /// </summary>
    public EasyTypeOptions? GenericArgumentOptions { get; init; }

    /// <summary>
    /// If enabled, then use the method's parenthesis, even if no parameters options were
    /// specified.
    /// </summary>
    public bool UseBrackets { get; init; }

    /// <summary>
    /// If not null, then the options to use with the member's parameters, if any. If null, then
    /// they are ignored.
    /// <br/> A not-null value of this property implies <see cref="UseBrackets"/>.
    /// </summary>
    public EasyParameterOptions? ParameterOptions { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyMethodOptions() { }

    /// <summary>
    /// A shared instance with empty-alike settings.
    /// </summary>
    public static EasyMethodOptions Empty { get; } = new();

    /// <summary>
    /// A shared instance with default-alike settings.
    /// </summary>
    public static EasyMethodOptions Default { get; } = new()
    {
        GenericArgumentOptions = EasyTypeOptions.Default,
        ParameterOptions = EasyParameterOptions.Default,
    };

    /// <summary>
    /// A shared instance with full-alike settings.
    /// </summary>
    public static EasyMethodOptions Full { get; } = new()
    {
        UseAccessibility = true,
        UsePrivate = true,
        UseModifiers = true,
        ReturnTypeOptions = EasyTypeOptions.Full,
        HostTypeOptions = EasyTypeOptions.Full,
        UseTechName = true,
        GenericArgumentOptions = EasyTypeOptions.Full,
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
        this MethodInfo source) => source.EasyName(EasyMethodOptions.Default);

    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(
        this MethodInfo source, EasyMethodOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        return EasyMethod(source, options);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this ConstructorInfo source) => source.EasyName(EasyMethodOptions.Default);

    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(
        this ConstructorInfo source, EasyMethodOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        return EasyMethod(source, options);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the c#-alike string representation of the given element.
    /// </summary>
    static string EasyMethod(this MethodBase source, EasyMethodOptions options)
    {
        var method = source as MethodInfo;
        var constructor = source as ConstructorInfo;

        var sb = new StringBuilder();
        var host = source.DeclaringType;
        var iface = host != null && host.IsInterface;

        // Accessibility (not using 'private' for simplicity)...
        if (options.UseAccessibility)
        {
            if (source.IsPrivate && options.UsePrivate) sb.Append("private ");
            if (source.IsPublic && !iface) sb.Append("public ");
            if (source.IsFamily) sb.Append("protected ");
            if (source.IsAssembly) sb.Append("internal ");
            if (source.IsFamilyOrAssembly) sb.Append("internal protected ");
            if (source.IsFamilyAndAssembly) sb.Append("private protected ");
        }

        // Modifiers...
        if (options.UseModifiers)
        {
            if (source.IsStatic) sb.Append("static ");

            if (source.IsAbstract && !iface) sb.Append("abstract ");
            if (IsSealed()) sb.Append("sealed ");
            if (IsNew()) sb.Append("new ");

            if (IsOverride()) sb.Append("override ");
            else if (IsVirtual() && !iface) sb.Append("virtual ");
        }

        // Return type (regular methods only)...
        if (options.ReturnTypeOptions != null && method != null)
        {
            // 'ref'-alike return types...
            if (options.UseModifiers && method.ReturnType.IsByRef)
            {
                var ronly = method.ReturnTypeCustomAttributes.HasReadOnlyAttribute();
                sb.Append(ronly ? "ref readonly " : "ref ");
            }

            // The type itself...
            var xoptions = options.ReturnTypeOptions;
            var type = method.ReturnType;
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
        if (options.HostTypeOptions != null)
        {
            var type = method?.DeclaringType ?? constructor?.DeclaringType;
            if (type != null)
            {
                var xoptions = options.HostTypeOptions;
                var str = type.EasyName(xoptions);
                if (str.Length > 0) sb.Append(str).Append('.');
            }
        }

        // Name...
        if (method != null) sb.Append(source.Name);
        if (constructor != null)
        {
            var str = host?.EasyName(EasyTypeOptions.Empty) ?? "new";
            sb.Append(str);

            if (options.UseTechName)
            {
                if (!source.Name.StartsWith('.')) sb.Append('.');
                sb.Append(source.Name);
            }
        }

        // Generic arguments (regular methods only)...
        if (options.GenericArgumentOptions != null && method != null)
        {
            var xoptions = options.GenericArgumentOptions;
            var args = source.GetGenericArguments();
            if (args.Length > 0)
            {
                sb.Append('<'); for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = xoptions.GenericListStyle == EasyGenericListStyle.PlaceHolders
                        ? string.Empty
                        : arg.EasyName(xoptions);

                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
                sb.Append('>');
            }
        }

        // Member parameters....
        if (options.UseBrackets || options.ParameterOptions != null)
        {
            sb.Append('('); if (options.ParameterOptions != null)
            {
                var xoptions = options.ParameterOptions;
                var args = source.GetParameters();
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(xoptions);

                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
            }
            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();

        // ------------------------------------------------

        /// <summary>
        /// Determines if the given source is a 'virtual' one.
        /// </summary>
        bool IsVirtual() => source.IsVirtual && !source.IsFinal;

        /// <summary>
        /// Determines if the given source method (not constructor) is an 'sealed' one.
        /// </summary>
        bool IsSealed() => source.IsVirtual && source.IsFinal;

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
        bool IsNew() =>
            method != null &&
            !method.IsVirtual &&
            FindBaseMethod(host?.BaseType) != null;

        MethodInfo? FindBaseMethod(Type? parent)
        {
            while (parent != null)
            {
                var temp = parent.GetMethod(
                    method.Name,
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Instance | BindingFlags.Static,
                    null,
                    [.. method.GetParameters().Select(p => p.ParameterType)],
                    null);

                if (temp != null) return temp;
                parent = parent.BaseType;
            }
            return null;
        }
    }
}