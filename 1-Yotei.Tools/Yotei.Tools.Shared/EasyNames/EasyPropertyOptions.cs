namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to obtain a C#-alike representation of a given property-alike element.
/// </summary>
public record EasyPropertyOptions
{
    /// <summary>
    /// If enabled, then use the accessibility modifiers, if any.
    /// </summary>
    public bool UseAccessibility { get; set; }

    /// <summary>
    /// If enabled, then use the element's modifiers (such as static, abstract, virtual, override,
    /// new, and ref-alike ones). Otherwise, they are ignored.
    /// </summary>
    public bool UseModifiers { get; set; }

    /// <summary>
    /// If not null, then the options to use to include the element's type.
    /// </summary>
    public EasyTypeOptions? MemberTypeOptions { get; set; }

    /// <summary>
    /// If not null, then the options to use to include the element's host type, if any.
    /// </summary>
    public EasyTypeOptions? HostTypeOptions { get; set; }

    /// <summary>
    /// If enabled, and if the element is an indexed property one, then use the internal CLR name.
    /// Otherwise, the standard "this" is used instead.
    /// </summary>
    public bool UseTechName { get; set; }

    /// <summary>
    /// If enabled, and if the element is an indexed property, use the elements's parenthesis even
    /// if no parameter options were specified.
    /// </summary>
    public bool UseBrackets { get; set; }

    /// <summary>
    /// If not null, and if the element is an indexed property, then the options to use to include
    /// the property arguments. If null, then they are ignored.
    /// </summary>
    public EasyParameterOptions? ParameterOptions { get; set; }

    // ----------------------------------------------------

    public enum Mode { Empty, Default, DefaultEx, Full };
    EasyPropertyOptions(Mode mode)
    {
        UseAccessibility = false;
        UseModifiers = false;
        MemberTypeOptions = null;
        HostTypeOptions = null;
        UseTechName = false;
        UseBrackets = false;
        ParameterOptions = null;

        switch (mode)
        {
            case Mode.Default:
                ParameterOptions = EasyParameterOptions.Default;
                break;

            case Mode.DefaultEx:
                HostTypeOptions = EasyTypeOptions.DefaultEx;
                ParameterOptions = EasyParameterOptions.DefaultEx;
                break;

            case Mode.Full:
                UseAccessibility = true;
                UseModifiers = true;
                MemberTypeOptions = EasyTypeOptions.Full;
                HostTypeOptions = EasyTypeOptions.Full;
                UseTechName = true;
                ParameterOptions = EasyParameterOptions.Full;
                break;
        }
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyPropertyOptions() : this(Mode.Empty) { }

    /// <summary>
    /// Obtains a new empty-alike instance.
    /// </summary>
    public static EasyPropertyOptions Empty => new(Mode.Empty);

    /// <summary>
    /// Obtains a new default-alike instance.
    /// </summary>
    public static EasyPropertyOptions Default => new(Mode.Default);

    /// <summary>
    /// Obtains a new default-alike instance whose type options are default extended ones.
    /// </summary>
    public static EasyPropertyOptions DefaultEx => new(Mode.DefaultEx);

    /// <summary>
    /// Obtains a new full-alike instance.
    /// </summary>
    public static EasyPropertyOptions Full => new(Mode.Full);
}

// ========================================================
public static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains a C#-alike representation for a given property-alike element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this PropertyInfo source) => source.EasyName(new EasyPropertyOptions());

    /// <summary>
    /// Obtains a C#-alike representation for a given property-alike element, using the given options.
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

        // Accessibility...
        if (options.UseAccessibility && method != null)
        {
            if (method.IsPublic && !iface) sb.Append("public ");
            if (method.IsFamily) sb.Append("protected ");
            if (method.IsAssembly) sb.Append("internal ");
            if (method.IsFamilyOrAssembly) sb.Append("internal protected ");
            if (method.IsFamilyAndAssembly) sb.Append("private protected ");
        }

        /* NOTE: 'partial' is a compilation-time only feature, not persisted once the source code
         * is compiled. It seems there is no way to obtain this information using reflections. In
         * any case, in this scenario, is not as bad as it sound.
         */

        // Modifiers...
        if (method != null && options.UseModifiers)
        {
            if (method.IsStatic) sb.Append("static ");
            if (method.IsAbstract && !iface) sb.Append("abstract ");
            if (IsSealed()) sb.Append("sealed ");

            if (IsOverride()) sb.Append("override ");
            else if (IsVirtual() && !iface && !method.IsAbstract) sb.Append("virtual ");
            else if (IsNew()) sb.Append("new ");
        }

        // Member type...
        if (method != null && options.MemberTypeOptions != null)
        {
            var xoptions = options.MemberTypeOptions;
            var arg = source.PropertyType;
            var str = arg.EasyName(xoptions);

            if (str.Length > 0)
            {
                // Nullability...
                var pointer = str.EndsWith('*');
                if (pointer) str = str[..^1].NotNullNotEmpty(trim: false);

                while (str[^1] != '?' && xoptions.NullableStyle != EasyNullableStyle.None)
                {
                    if (xoptions.NullableStyle == EasyNullableStyle.KeepWrappers &&
                        arg.IsNullableWrapper() &&
                        !arg.IsArray && !arg.IsPointer) break;

                    if (arg.IsNullableAnnotated()) { str += '?'; break; }
                    if (source.IsNullableAnnotated()) { str += '?'; break; }
                    break;
                }
                if (pointer) str += '*'; // Don't forget to restore pointer!
            }

            // Ref-alike types (using method first, arg as a fallback)...
            if (options.UseModifiers)
            {
                if (method != null && method.ReturnType.IsByRef)
                {
                    var ronly = method.ReturnTypeCustomAttributes.HasReadOnlyAttribute();
                    sb.Append(ronly ? "ref readonly " : "ref ");
                }
                else if (options.UseModifiers && arg.IsByRef)
                {
                    var ronly = arg.HasReadOnlyAttribute();
                    sb.Append(ronly ? "ref readonly " : "ref ");
                }
            }

            // Adding...
            sb.Append(str).Append(' ');
        }

        // Host type...
        if (options.HostTypeOptions != null && host != null)
        {
            var str = host.EasyName(options.HostTypeOptions);
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
            method.GetBaseDefinition().DeclaringType != method.DeclaringType;

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