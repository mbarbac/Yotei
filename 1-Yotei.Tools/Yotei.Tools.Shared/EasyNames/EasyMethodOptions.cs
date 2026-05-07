namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to obtain a C#-alike representation of a given method-alike element.
/// </summary>
public record EasyMethodOptions
{
    /// <summary>
    /// If enabled, then use the accessibility modifiers, if any.
    /// </summary>
    public bool UseAccessibility { get; set; }

    /// <summary>
    /// If enabled, then use the method's modifiers (such as static, abstract, virtual, override,
    /// new, and ref-alike ones). Otherwise, they are ignored.
    /// </summary>
    public bool UseModifiers { get; set; }

    /// <summary>
    /// If not null, then the options to use to include the method's return type.
    /// </summary>
    public EasyTypeOptions? ReturnTypeOptions { get; set; }

    /// <summary>
    /// If not null, then the options to use to include the method's host type, if any.
    /// </summary>
    public EasyTypeOptions? HostTypeOptions { get; set; }

    /// <summary>
    /// If enabled, and if the method is a constructor-alike one, then use the method's CLR name.
    /// Otherwise, the host plain name is used instead.
    /// </summary>
    public bool UseTechName { get; set; }

    /// <summary>
    /// The options to use with the generic method arguments, if any. If null, then that list of
    /// generic type arguments is ignored.
    /// </summary>
    public EasyTypeOptions? GenericListOptions { get; set; }

    /// <summary>
    /// If enabled, use the method's parenthesis even if no parameter options were specified.
    /// </summary>
    public bool UseBrackets { get; set; }

    /// <summary>
    /// If not null, then the options to use to include the method arguments. If null, then they
    /// are ignored.
    /// </summary>
    public EasyParameterOptions? ParameterOptions { get; set; }

    // ----------------------------------------------------

    public enum Mode { Empty, Default, DefaultEx, Full };
    EasyMethodOptions(Mode mode)
    {
        UseAccessibility = false;
        UseModifiers = false;
        ReturnTypeOptions = null;
        HostTypeOptions = null;
        UseTechName = false;
        GenericListOptions = null;
        UseBrackets = false;
        ParameterOptions = null;

        switch (mode)
        {
            case Mode.Default:
                GenericListOptions = EasyTypeOptions.Default;
                ParameterOptions = EasyParameterOptions.Default;
                break;

            case Mode.DefaultEx:
                HostTypeOptions = EasyTypeOptions.DefaultEx;
                GenericListOptions = EasyTypeOptions.DefaultEx;
                ParameterOptions = EasyParameterOptions.DefaultEx;
                break;

            case Mode.Full:
                UseAccessibility = true;
                UseModifiers = true;
                ReturnTypeOptions = EasyTypeOptions.Full;
                HostTypeOptions = EasyTypeOptions.Full;
                UseTechName = true;
                GenericListOptions = EasyTypeOptions.Full;
                ParameterOptions = EasyParameterOptions.Full;
                break;
        }
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyMethodOptions() : this(Mode.Empty) { }

    /// <summary>
    /// Obtains a new empty-alike instance.
    /// </summary>
    public static EasyMethodOptions Empty => new(Mode.Empty);

    /// <summary>
    /// Obtains a new default-alike instance.
    /// </summary>
    public static EasyMethodOptions Default => new(Mode.Default);

    /// <summary>
    /// Obtains a new default-alike instance whose type options are default extended ones.
    /// </summary>
    public static EasyMethodOptions DefaultEx => new(Mode.DefaultEx);

    /// <summary>
    /// Obtains a new full-alike instance.
    /// </summary>
    public static EasyMethodOptions Full => new(Mode.Full);
}

// ========================================================
public static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains a C#-alike representation for a given method-alike element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this MethodInfo source) => source.EasyName(new EasyMethodOptions());

    /// <summary>
    /// Obtains a C#-alike representation for a given method-alike element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this MethodInfo source, EasyMethodOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        return EasyMethodBase(source, options);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains a C#-alike representation for a given constructor-alike element, using default
    /// options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this ConstructorInfo source) => source.EasyName(new EasyMethodOptions());

    /// <summary>
    /// Obtains a C#-alike representation for a given constructor-alike element, using the given
    /// options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ConstructorInfo source, EasyMethodOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        return EasyMethodBase(source, options);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Factorizes common code to obtain the C#-alike representation of the given element.
    /// </summary>
    static string EasyMethodBase(this MethodBase source, EasyMethodOptions options)
    {
        var method = source as MethodInfo;
        var constructor = source as ConstructorInfo;

        var sb = new StringBuilder();
        var host = source.DeclaringType;
        var iface = host != null && host.IsInterface;

        // Accessibility...
        if (options.UseAccessibility)
        {
            if (source.IsPublic && !iface) sb.Append("public ");
            if (source.IsFamily) sb.Append("protected ");
            if (source.IsAssembly) sb.Append("internal ");
            if (source.IsFamilyOrAssembly) sb.Append("internal protected ");
            if (source.IsFamilyAndAssembly) sb.Append("private protected ");
        }

        /* NOTE: 'partial' is a compilation-time only feature, not persisted once the source code
         * is compiled. It seems there is no way to obtain this information using reflections. In
         * any case, in this scenario, is not as bad as it sound.
         */

        // Modifiers...
        if (options.UseModifiers)
        {
            if (source.IsStatic) sb.Append("static ");
            if (source.IsAbstract && !iface) sb.Append("abstract ");
            if (IsSealed()) sb.Append("sealed ");

            if (IsOverride()) sb.Append("override ");
            else if (IsVirtual() && !iface && !source.IsAbstract) sb.Append("virtual ");
            else if (IsNew()) sb.Append("new ");
        }

        // Return type...
        if (method != null && options.ReturnTypeOptions != null)
        {
            var xoptions = options.ReturnTypeOptions;
            var arg = method.ReturnType;
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

                // Ref-alike types...
                if (options.UseModifiers && arg.IsByRef)
                {
                    var ronly = method.ReturnTypeCustomAttributes.HasReadOnlyAttribute();
                    sb.Append(ronly ? "ref readonly " : "ref ");
                }

                // Adding...
                sb.Append(str).Append(' ');
            }
        }

        // Host type...
        if (options.HostTypeOptions != null)
        {
            host = method?.DeclaringType ?? constructor?.DeclaringType;
            if (host != null)
            {
                var str = host.EasyName(options.HostTypeOptions);
                if (str.Length > 0)
                {
                    sb.Append(str);
                    if (method != null) sb.Append('.'); // Only for regular methods...
                }
            }
        }

        // Name...
        if (method != null) sb.Append(source.Name);
        if (constructor != null)
        {
            if (options.HostTypeOptions == null) // Otherwise it has been already captured!
            {
                var str = host?.EasyName() ?? "new";
                sb.Append(str);
            }
            if (options.UseTechName) // Adding the CLR name if requested
            {
                if (!source.Name.StartsWith('.')) sb.Append('.');
                sb.Append(source.Name);
            }
        }

        // Generic arguments (regular methods only)...
        if (method != null && options.GenericListOptions != null)
        {
            var xoptions = options.GenericListOptions;
            var args = source.GetGenericArguments();
            if (args.Length > 0)
            {
                sb.Append('<'); for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(xoptions);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
                sb.Append('>');
            }
        }

        // Parameters...
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