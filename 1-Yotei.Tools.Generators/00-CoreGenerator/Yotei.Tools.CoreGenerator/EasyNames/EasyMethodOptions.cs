namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for method instances.
/// </summary>
internal record EasyMethodOptions
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
    /// If not null, the options to use with the method return type. Otherwise, it is ignored.
    /// </summary>
    public EasyTypeOptions? ReturnTypeOptions { get; init; }

    /// <summary>
    /// If not null, the options to use with the method host type. Otherwise, it is ignored.
    /// </summary>
    public EasyTypeOptions? HostTypeOptions { get; init; }

    /// <summary>
    /// If the method represents a constructor, then use its technical CLR name instead of the
    /// simplified one.
    /// </summary>
    public bool UseTechName { get; init; }

    /// <summary>
    /// If not null, the options to use with the method's generic arguments, if any. If null,
    /// then they are ignored.
    /// </summary>
    public EasyTypeOptions? GenericOptions { get; init; }

    /// <summary>
    /// If enabled then use parenthesis after the method's name, even if no parameter options
    /// are specified.
    /// </summary>
    public bool UseBrackets { get; init; }

    /// <summary>
    /// If not null, the options to use with the method parameters, if any. Enabling this setting
    /// also enables <see cref="UseBrackets"/>.
    /// </summary>
    public EasyParameterOptions? ParameterOptions { get; init; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    EasyMethodOptions(Mode mode)
    {
        switch (mode)
        {
            case Mode.Full:
                UseAccessibility = true;
                UseModifiers = true;
                ReturnTypeOptions = EasyTypeOptions.Full;
                HostTypeOptions = EasyTypeOptions.Full;
                UseTechName = true;
                GenericOptions = EasyTypeOptions.Full;
                UseBrackets = true;
                ParameterOptions = EasyParameterOptions.Full;
                break;

            case Mode.Default:
                GenericOptions = EasyTypeOptions.Default;
                UseBrackets = true;
                ParameterOptions = EasyParameterOptions.Default;
                break;
        }
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyMethodOptions() : this(Mode.Empty) { }

    /// <summary>
    /// A shared empty instance.
    /// </summary>
    public static EasyMethodOptions Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared instance with default settings.
    /// </summary>
    public static EasyMethodOptions Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared instance with full settings.
    /// </summary>
    public static EasyMethodOptions Full { get; } = new(Mode.Full);
}

// ========================================================
internal static partial class EasyNameExtensions
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
    public static string EasyName(this MethodInfo source, EasyMethodOptions options)
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
    public static string EasyName(this ConstructorInfo source, EasyMethodOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        return EasyMethod(source, options);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Factorizes common code.
    /// </summary>
    static string EasyMethod(this MethodBase source, EasyMethodOptions options)
    {   
        var method = source as MethodInfo;
        var constructor = source as ConstructorInfo;

        var sb = new StringBuilder();
        var host = source.DeclaringType;
        var iface = host != null && host.IsInterface;

        // Accesibility (not using 'private' for simplicity)...
        if (options.UseAccessibility)
        {
            if (source.IsPublic && !iface) sb.Append("public ");
            if (source.IsFamily) sb.Append("protected ");
            if (source.IsAssembly) sb.Append("internal ");
            if (source.IsFamilyOrAssembly) sb.Append("protected internal ");
            if (source.IsFamilyAndAssembly) sb.Append("private protected ");
        }

        //bool IsSealed() =>
        //    (source.IsVirtual && source.IsFinal) ||
        //    (source.Attributes & MethodAttributes.Final) != 0 ||
        //    (iface && method != null && !method.IsVirtual);

        // Modifiers...
        if (options.UseModifiers)
        {
            // LOW: not found a reliable way to determine if 'sealed' was used...
            // LOW: not found a way to determine if 'partial' was used...

            if (IsNew()) sb.Append("new ");
            if (source.IsStatic) sb.Append("static ");
            
            if (!iface && source.IsAbstract) sb.Append("abstract ");
            if (IsOverride()) sb.Append("override ");
            else if (!iface && IsVirtual()) sb.Append("virtual ");

            bool IsOverride() =>
                method != null &&
                method.IsVirtual &&
                method.GetBaseDefinition().DeclaringType != source.DeclaringType;

            bool IsVirtual() => source.IsVirtual && !source.IsFinal;

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

        // Return type...
        if (method != null && options.ReturnTypeOptions != null)
        {
            if (options.UseModifiers && method.ReturnType.IsByRef)
            {
                var ronly = method.ReturnTypeCustomAttributes.HasReadOnlyAttribute();
                sb.Append(ronly ? "ref readonly " : "ref ");
            }

            var xoptions = options.ReturnTypeOptions.NoHideName();
            var str = method.ReturnType.EasyName(xoptions);
            
            while (str.Length > 0 && str[^1] != '?' && source.HasNullableEnabledAttribute())
            {
                if (xoptions.NullableStyle == EasyNullableStyle.KeepWrappers &&
                    method.ReturnType.IsNullableWrapper())
                    break;

                if (xoptions.NullableStyle != EasyNullableStyle.None) str += '?';
                break;
            }

            sb.Append(str).Append(' ');
        }

        // Host type...
        if (options.HostTypeOptions != null)
        {
            var temp = method != null ? host : host?.DeclaringType;
            if (temp != null)
            {
                var xoptions = options.HostTypeOptions.NoHideName();
                var str = temp.EasyName(xoptions);
                if (str.Length > 0) sb.Append(str).Append('.');
            }
        }

        // Name...
        if (constructor != null)
        {
            var name = host?.EasyName(EasyTypeOptions.Empty) ?? "new";
            if (options.UseTechName) sb.Append(source.Name); // already has a dot!
        }
        if (method != null) sb.Append(source.Name);

        // Generic arguments...
        if (method != null && options.GenericOptions != null)
        {
            var args = source.GetGenericArguments();
            if (args.Length > 0)
            {
                sb.Append('<'); for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(options.GenericOptions);
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
                var args = source.GetParameters();
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = arg.EasyName(options.ParameterOptions);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
            }
            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();
    }
}