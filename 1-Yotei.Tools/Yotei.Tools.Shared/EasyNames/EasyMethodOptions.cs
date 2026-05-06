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
    /// Facorizes common code to obtain the C#-alike representation of the given element.
    /// </summary>
    static string EasyMethodBase(this MethodBase source, EasyMethodOptions options)
    {
        var method = source as MethodInfo;
        var constructor = source as ConstructorInfo;

        var sb = new StringBuilder();
        var host = source.DeclaringType;
        var iface = host != null && host.IsInterface;

        // Accessibility...
        // TODO...

        // Modifiers...
        // TODO...

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

                while (str[^1] != '?')
                {
                    if (xoptions.NullableStyle == EasyNullableStyle.UseAnnotations)
                    {
                        if (arg.IsNullableAnnotated()) { str += '?'; break; }
                        if (source.IsNullableAnnotated()) { str += '?'; break; }
                    }
                    if (xoptions.NullableStyle == EasyNullableStyle.KeepWrappers && (
                        arg.IsArray || arg.IsPointer))
                    {
                        if (source.IsNullableAnnotated()) { str += '?'; break; }
                        if (arg.IsNullableAnnotated()) { str += '?'; break; }
                    }
                    break;
                }
                if (pointer) str += '*';

                // Ref-alike types...
                // TODO...

                // Adding...
                sb.Append(str).Append(' ');
            }
        }

        // Host type...
        // TODO...
        // Modify the options to prevent return type

        // Name...
        // TODO...
        sb.Append(source.Name);

        // Finishing...
        return sb.ToString();
    }
}