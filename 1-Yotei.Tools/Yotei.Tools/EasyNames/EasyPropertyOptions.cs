namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for property instances.
/// </summary>
public record EasyPropertyOptions
{
    /// <summary>
    /// If enabled use member accessibility modifiers.
    /// </summary>
    public bool UseAccessibility { get; init; }

    /// <summary>
    /// If enabled use member modifiers.
    /// </summary>
    public bool UseModifiers { get; init; }

    /// <summary>
    /// If not null, the options to use with the member type. Otherwise, it is ignored.
    /// </summary>
    public EasyTypeOptions? MemberTypeOptions { get; init; }

    /// <summary>
    /// If not null, the options to use with the method host type. Otherwise, it is ignored.
    /// </summary>
    public EasyTypeOptions? HostTypeOptions { get; init; }

    /// <summary>
    /// If the method represents an indexed property, then use its technical CLR name instead of
    /// the simplified 'this' one.
    /// </summary>
    public bool UseTechName { get; init; }

    /// <summary>
    /// If enabled and the property is an indexed one, then use square brackets after the member
    /// name, even if no parameter options are specified.
    /// </summary>
    public bool UseBrackets { get; init; }

    /// <summary>
    /// If not null and the property is an indexed one, then the options to use with the member
    /// parameters, if any. Enabling this setting also enables <see cref="UseBrackets"/>.
    /// </summary>
    public EasyParameterOptions? ParameterOptions { get; init; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    EasyPropertyOptions(Mode mode)
    {
        switch (mode)
        {
            case Mode.Full:
                UseAccessibility = true;
                UseModifiers = true;
                MemberTypeOptions = EasyTypeOptions.Full;
                HostTypeOptions = EasyTypeOptions.Full;
                UseTechName = true;
                UseBrackets = true;
                ParameterOptions = EasyParameterOptions.Full;
                break;

            case Mode.Default:
                UseBrackets = true;
                ParameterOptions = EasyParameterOptions.Default;
                break;
        }
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyPropertyOptions() : this(Mode.Empty) { }

    /// <summary>
    /// A shared empty instance.
    /// </summary>
    public static EasyPropertyOptions Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared instance with default settings.
    /// </summary>
    public static EasyPropertyOptions Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared instance with full settings.
    /// </summary>
    public static EasyPropertyOptions Full { get; } = new(Mode.Full);
}

// ========================================================
public static partial class EasyNameExtensions
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
    public static string EasyName(this PropertyInfo source, EasyPropertyOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();
        var host = source.DeclaringType;
        var iface = host != null && host.IsInterface;
        var method = source.GetGetMethod() ?? source.GetSetMethod();

        // Accesibility (not using 'private' for simplicity)...
        if (options.UseAccessibility && method != null)
        {
            if (method.IsPublic && !iface) sb.Append("public ");
            if (method.IsFamily) sb.Append("protected ");
            if (method.IsAssembly) sb.Append("internal ");
            if (method.IsFamilyOrAssembly) sb.Append("protected internal ");
            if (method.IsFamilyAndAssembly) sb.Append("private protected ");
        }

        // Modifiers...
        if (options.UseModifiers && method != null)
        {
            if (IsNew()) sb.Append("new ");
            if (method.IsStatic) sb.Append("static ");
            if (!iface && method.IsAbstract) sb.Append("abstract ");
            if (IsOverride()) sb.Append("override ");
            else if (!iface && IsVirtual()) sb.Append("virtual ");

            // LOW: no reliable way to obtain 'sealed' modifier in EasyName(method).
            //bool IsSealed() =>
            //    (source.IsVirtual && source.IsFinal) ||
            //    (source.Attributes & MethodAttributes.Final) != 0 ||
            //    (iface && method != null && !method.IsVirtual);

            bool IsOverride() =>
                method != null &&
                method.IsVirtual &&
                method.GetBaseDefinition().DeclaringType != source.DeclaringType;

            bool IsVirtual() => method.IsVirtual && !method.IsFinal;

            bool IsNew() => method != null && !method.IsVirtual && BaseMethod(host?.BaseType) != null;
            MethodInfo? BaseMethod(Type? parent)
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

        // Member type...
        if (options.MemberTypeOptions != null)
        {
            if (options.UseModifiers && source.PropertyType.IsByRef)
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

            var xoptions = options.MemberTypeOptions.NoHideName();
            var str = source.PropertyType.EasyName(xoptions);
            
            if (str.Length > 0 &&
                str[^1] != '?' && (
                source.HasNullableEnabledAttribute() || source.IsNullableByApi()))
            {
                if (xoptions.NullableStyle == EasyNullableStyle.KeepWrappers &&
                    source.PropertyType.IsNullableWrapper())
                    goto ENDNULLABLE;

                if (xoptions.NullableStyle != EasyNullableStyle.None) str += '?';
            }

            ENDNULLABLE:
            sb.Append(str).Append(' ');
        }

        // Host type...
        if (options.HostTypeOptions != null && host != null)
        {
            var xoptions = options.HostTypeOptions.NoHideName();
            var str = host.EasyName(xoptions);
            if (str.Length > 0) sb.Append(str).Append('.');
        }

        // Name...
        var name = source.Name;
        var args = source.GetIndexParameters();
        if (args.Length > 0 && !options.UseTechName) name = "this";
        sb.Append(name);

        // Parameters...
        if (args.Length > 0 && (
            options.UseBrackets || options.ParameterOptions != null))
        {
            sb.Append('['); if (options.ParameterOptions != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(options.ParameterOptions);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
            }
            sb.Append(']');
        }

        // Finishing...
        return sb.ToString();
    }
}