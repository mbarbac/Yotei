namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to obtain a C#-alike representation of a given parameter-alike element.
/// </summary>
public record EasyParameterOptions
{
    /// <summary>
    /// If enabled, and if the given parameter is the first one of an extension method, then use
    /// the 'this' prefix.
    /// </summary>
    public bool UseThis { get; set; }

    /// <summary>
    /// If enabled, then use the parameter modifiers (such as: params, scoped, ref, in, out...)
    /// if any.
    /// </summary>
    public bool UseModifiers { get; set; }

    /// <summary>
    /// If not null, then the options to use to obtain the easy name of the parameter's type.
    /// If null, then the type is ignored.
    /// </summary>
    public EasyTypeOptions? TypeOptions { get; set; }

    /// <summary>
    /// If enabled, then use the parameter's name. Otherwise, it is ignored.
    /// </summary>
    public bool UseName { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines what options to use when creating a new instance.
    /// </summary>
    public enum Mode { Empty, Default, Full };

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyParameterOptions() : this(Mode.Empty) { }

    /// <summary>
    /// Obtains a new empty-alike instance.
    /// </summary>
    public static EasyParameterOptions Empty => new(Mode.Empty);

    /// <summary>
    /// Obtains a new default-alike instance.
    /// </summary>
    public static EasyParameterOptions Default => new(Mode.Default);

    /// <summary>
    /// Obtains a new full-alike instance.
    /// </summary>
    public static EasyParameterOptions Full => new(Mode.Full);

    /// <summary>
    /// Initializes a new instance with values associated with the given mode.
    /// </summary>
    /// <param name="mode"></param>
    public EasyParameterOptions(Mode mode)
    {
        UseThis = false;
        UseModifiers = false;
        TypeOptions = null;
        UseName = false;

        switch (mode)
        {
            case Mode.Default:
                UseThis = true;
                UseModifiers = true;
                TypeOptions = new EasyTypeOptions(EasyTypeOptions.Mode.Default);
                break;

            case Mode.Full:
                UseThis = true;
                UseModifiers = true;
                TypeOptions = new EasyTypeOptions(EasyTypeOptions.Mode.Full);
                UseName = true;
                break;
        }
    }
}


// ========================================================
public static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains a C#-alike representation for a given parameter-alike element, using default
    /// options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this ParameterInfo source) => source.EasyName(new EasyParameterOptions());

    /// <summary>
    /// Obtains a C#-alike representation for a given parameter-alike element, using the given
    /// options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ParameterInfo source, EasyParameterOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();

        // Parameter type (only if requested)...
        if (options.TypeOptions != null)
        {
            var xoptions = options.TypeOptions;
            var type = source.ParameterType;
            var str = type.EasyName(xoptions);

            // Parameter instances are sensible to the nullability API, which we can leverage
            // to intercept reference types...
            if (str.Length > 0 &&
                str[^1] != '?' &&
                source.IsNullableAnnotated()) str += '?';

            // Adding...
            sb.Append(str);
        }

        // Parameter name (only if requested)...
        if (options.UseName)
        {
            if (sb.Length > 0) sb.Append(' ');
            sb.Append(source.Name);
        }

        // Modifiers (only if 'sb' is not an empty one)...
        if (sb.Length > 0)
        {
            // Special case for 'this'...
            if (options.UseThis && source.Name != null)
            {
                var method = source.Member as MethodBase;

                if (method != null &&
                    method.IsDefined(typeof(ExtensionAttribute), false) &&
                    source.Position == 0)
                    sb.Insert(0, "this ");
            }

            // Other modifiers...
            if (options.UseModifiers)
            {
                // Traditional 'in' parameter...
                if (source.ParameterType.IsByRef &&
                    source.IsIn &&
                    source.HasReadOnlyAttribute())
                    sb.Insert(0, "in ");

                // Special 'ref readonly' ones...
                else if (source.ParameterType.IsByRef &&
                    source.GetCustomAttributes().Any(x => x.GetType().FullName == REQUIRES_LOCATION))
                    sb.Insert(0, "ref readonly ");

                // Other ref-alike ones...
                else if (source.IsIn) sb.Insert(0, "in ");
                else if (source.IsOut) sb.Insert(0, "out ");
                else if (source.ParameterType.IsByRef) sb.Insert(0, "ref ");

                // Params...
                if (source.IsDefined(typeof(ParamArrayAttribute), false)) sb.Insert(0, "params ");

                // Scoped...
                if (source
                    .GetCustomAttributes()
                    .Any(x => x.GetType().FullName == SCOPED_ATTRIBUTE))
                    sb.Insert(0, "scoped ");
            }
        }

        // Finishing...
        return sb.ToString();
    }
}