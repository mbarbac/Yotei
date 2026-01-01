namespace Yotei.Tools;

// ========================================================
public static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains the C#-alike easy name of the given element using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this ParameterInfo source) => EasyNameParameterOptions.Default.EasyName(source);

    /// <summary>
    /// Obtains the C#-alike easy name of the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this ParameterInfo source, EasyNameParameterOptions options)
    {
        options.ThrowWhenNull();
        return options.EasyName(source);
    }
}

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for <see cref="ParameterInfo"/> instances.
/// <br/> Empty options render an empty string.
/// <br/> Default options just render the name of the element.
/// </summary>
public record EasyNameParameterOptions
{
    /// <summary>
    /// A shared read-only instance that represents empty options.
    /// </summary>
    public static EasyNameParameterOptions Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared read-only instance that represents default options.
    /// </summary>
    public static EasyNameParameterOptions Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared read-only instance that represents full options.
    /// </summary>
    public static EasyNameParameterOptions Full { get; } = new(Mode.Full);

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyNameParameterOptions() : this(Mode.Default) { }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if parameter modifiers such as 'in', 'out' and 'ref' shall be used.
    /// <br/> The value of this property is 'false' by default.
    /// </summary>
    public bool UseModifiers { get; init; }

    /// <summary>
    /// If not null, then the options to use to print the type of the parameter. If null, then it
    /// is ignored.
    /// <br/> The value of this property is '<see cref="EasyNameTypeOptions.Default"/>' by default.
    /// </summary>
    public EasyNameTypeOptions? TypeOptions { get; init; }

    /// <summary>
    /// Determines if the name of the parameter shall be used, or not.
    /// <br/> The value of this property is 'false' by default.
    /// </summary>
    public bool UseName { get; init; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    private EasyNameParameterOptions(Mode mode)
    {
        switch (mode)
        {
            case Mode.Empty:
                break;

            case Mode.Default:
                TypeOptions = EasyNameTypeOptions.Default;
                break;

            case Mode.Full:
                UseModifiers = true;
                TypeOptions = EasyNameTypeOptions.Full;
                UseName = true;
                break;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the C#-alike easy name of the given element.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public string EasyName(ParameterInfo source)
    {
        source.ThrowWhenNull();

        var sb = new StringBuilder();
        var prefix = string.Empty;

        // Parameter modifiers...
        if (UseModifiers && (TypeOptions is not null || UseName))
        {
            if (source.IsIn) prefix = "in ";
            else if (source.IsOut) prefix = "out ";
            else if (source.ParameterType.IsByRef) prefix = "ref ";
        }

        // Parameter type...
        if (TypeOptions is not null)
        {
            var str = source.ParameterType.EasyName(TypeOptions);
            if (str.Length > 0)
            {
                sb.Append(prefix);
                sb.Append(str);
            }

            // Nullability...
            if (TypeOptions.UseNullability && sb.Length > 0 && sb[^1] != '?')
            {
                // Nullability API not reliable for generic types...
                if (source.ParameterType.FullName == null)
                {
                    var at = source.GetCustomAttribute<NullableAttribute>();
                    if (at is not null &&
                        at.NullableFlags.Length > 0 &&
                        at.NullableFlags[0] == 2)
                        sb.Append('?');
                }

                // Standard case using nullability API...
                else
                {
                    var nic = new NullabilityInfoContext();
                    var info = nic.Create(source);

                    if (info.ReadState == NullabilityState.Nullable ||
                        info.WriteState == NullabilityState.Nullable)
                        sb.Append('?');
                }
            }
        }

        // Parameter name...
        if (UseName)
        {
            if (sb.Length > 0) sb.Append(' ');
            sb.Append(source.Name);
        }

        // Finishing...
        return sb.ToString();
    }
}