namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static partial class EasyNameExtensions
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
/// Provides 'EasyName' capabilities for 'parameter' instances.
/// </summary>
internal record EasyNameParameterOptions
{/// <summary>
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
    /// Determines if parameter modifiers (such as 'in', 'out' and 'ref') shall be used.
    /// </summary>
    public bool UseModifiers { get; init; }

    /// <summary>
    /// If not null, then the options to use with the type of the parameter. If null, then it is
    /// ignored.
    /// </summary>
    public EasyNameTypeOptions? TypeOptions { get; init; }

    /// <summary>
    /// Determines if the name of the parameter shall be used or not.
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
                UseModifiers = true;
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

        // Parameter type...
        if (TypeOptions is not null)
        {
            var str = TypeOptions.EasyName(source.ParameterType);
            if (str.Length > 0) sb.Append(str);

            while (TypeOptions.UseNullability && str.Length > 0 && sb[^1] != '?')
            {
                var type = source.ParameterType;
                var name = type.Name;

                // Special case: nullable wrappers...
                if (TypeOptions.UseNullableWrappers &&
                    (name.StartsWith("Nullable`1") || name.StartsWith("IsNullable`1")))
                    break;

                // Nullable attribute...
                var at = source.GetCustomAttributes().FirstOrDefault(
                    x => x.GetType().Name == "NullableAttribute");

                if (at is not null)
                {
                    type = at.GetType();

                    var member = type.GetProperty("NullableFlags");
                    if (member != null)
                    {
                        var value = member.GetValue(at) as byte[];
                        if (value is not null &&
                            value.Length > 0 &&
                            value[0] == 2)
                        {
                            sb.Append('?');
                            break;
                        }
                    }
                }

                // IsNullable attribute...
                at = source.GetCustomAttribute<IsNullableAttribute>();
                if (at is not null)
                {
                    sb.Append('?');
                    break;
                }

                /* Net Standard 2.0 doesn't support nullability API...
                // Standard case via nullability API, althoug limited...
                var nic = new NullabilityInfoContext();
                var info = nic.Create(source);

                if (info.ReadState == NullabilityState.Nullable ||
                    info.WriteState == NullabilityState.Nullable)
                {
                    sb.Append('?');
                    break;
                }*/

                // End of nullability...
                break;
            }
        }

        // Parameter name...
        if (UseName)
        {
            if (sb.Length > 0) sb.Append(' ');
            sb.Append(source.Name);
        }

        // Parameter modifiers...
        if (UseModifiers && sb.Length > 0)
        {
            string? prefix = null;
            if (source.IsIn) prefix = "in ";
            else if (source.IsOut) prefix = "out ";
            else if (source.ParameterType.IsByRef) prefix = "ref ";

            if (prefix is not null) sb.Insert(0, prefix);
        }

        // Finishing...
        return sb.ToString();
    }
}