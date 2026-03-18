namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for method instances.
/// </summary>
public record EasyMethodOptions
{
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
public static partial class EasyNameExtensions
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

        // Modifiers...
        if (options.UseModifiers)
        {
            if (source.IsPublic && !iface) sb.Append("public ");
            if (source.IsPrivate) sb.Append("private ");
            if (source.IsFamily) sb.Append("protected ");
            if (source.IsAssembly) sb.Append("internal ");
            if (source.IsFamilyOrAssembly) sb.Append("protected internal ");
            if (source.IsFamilyAndAssembly) sb.Append("private protected ");

            if (source.IsStatic) sb.Append("static ");
            if (!iface && source.IsAbstract) sb.Append("abstract ");
            // if (IsSealed()) sb.Append("sealed ");

            if (IsOverride()) sb.Append("override ");
            else if (!iface && IsVirtual()) sb.Append("virtual ");
        }

        //LOW: EasyName, 'sealed' modifier in methods from interfaces.
        //I have not found a way to obtain this in a reliable way, ie: from interface...
        //bool IsSealed() =>
        //    (source.IsVirtual && source.IsFinal) ||
        //    (source.Attributes & MethodAttributes.Final) != 0 ||
        //    (iface && method != null && !method.IsVirtual);

        bool IsOverride() =>
            method != null &&
            method.IsVirtual &&
            method.GetBaseDefinition().DeclaringType != source.DeclaringType;

        bool IsVirtual() => source.IsVirtual && !source.IsFinal;

        // Return type...
        if (method != null && options.ReturnTypeOptions != null)
        {
            if (options.UseModifiers && method.ReturnType.IsByRef)
            {
                var name = "System.Runtime.CompilerServices.IsReadOnlyAttribute";
                var attr = method.ReturnTypeCustomAttributes
                    .GetCustomAttributes(false)
                    .Any(x => x.GetType().FullName == name);

                sb.Append(attr ? "ref readonly " : "ref ");
            }

            var xoptions = options.ReturnTypeOptions.NoHideName();
            var str = method.ReturnType.EasyName(xoptions);
            if (str.Length > 0) sb.Append(str).Append(' ');
        }

        // Host type...
        if (method != null && options.HostTypeOptions != null && host != null)
        {
            var xoptions = options.HostTypeOptions.NoHideName();
            var str = host.EasyName(xoptions);
            if (str.Length > 0) sb.Append(str).Append('.');
        }

        // Name...
        if (constructor != null)
        {
            // HIGH: EasyMethod constructor name
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