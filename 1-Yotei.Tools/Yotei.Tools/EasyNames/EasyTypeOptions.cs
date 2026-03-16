namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for <see cref="Type"/> instances.
/// </summary>
public record EasyTypeOptions
{
    /// <summary>
    /// If enabled use the type's variance (the 'in' and 'out' keywords) in the display string,
    /// if any is specified. Otherwise, it is ignored.
    /// </summary>
    public bool UseVariance { get; init; }

    /// <summary>
    /// If enabled use the type's namespace in the display string. Otherwise, it is ignored.
    /// Enabling this option automatically enabled <see cref="UseHost"/>.
    /// </summary>
    public bool UseNamespace { get; init; }

    /// <summary>
    /// If enabled use the type hosts chain in the display string (with this options). Otherwise,
    /// it is ignored.
    /// </summary>
    public bool UseHost { get; init; }

    /// <summary>
    /// If enabled inconditionally returns an empty string as the display string. This setting is
    /// mostly used to obtain an anonymous list of generic arguments. When enabled it shortcuts
    /// all other settings.
    /// </summary>
    public bool HideName { get; init; }

    public EasyTypeOptions WithNoHideName => HideName
        ? this with { HideName = false }
        : this;

    /// <summary>
    /// If enabled use in the display string the predefined keywords for known special types (eg:
    /// <see langword="int"/> instead of <see langword="Int32"/>).
    /// </summary>
    public bool UseSpecialNames { get; init; }

    /// <summary>
    /// If enabled remove the 'Attribute' suffix from the type's display string. Otherwise,
    /// it is kept in the display string.
    /// </summary>
    public bool RemoveAttributeSuffix { get; init; }

    /// <summary>
    /// Determines the nullable style to use with nullable annotations, if any.
    /// </summary>
    public IsNullableStyle NullableStyle { get; init; }

    /// <summary>
    /// If not <see langword="null"/> the options to use the type's generic arguments, if any.
    /// Otherwise, they are ignored.
    /// </summary>
    public EasyTypeOptions? GenericOptions { get; init; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    EasyTypeOptions(Mode mode)
    {
        switch (mode)
        {
            case Mode.Full:
                UseVariance = true;
                UseNamespace = true;
                UseHost = true;
                HideName = false;
                UseSpecialNames = false;
                RemoveAttributeSuffix = false;
                NullableStyle = IsNullableStyle.KeepWrappers;
                GenericOptions = this;
                break;

            case Mode.Default:
                UseSpecialNames = true;
                RemoveAttributeSuffix = true;
                NullableStyle = IsNullableStyle.UseAnnotations;
                GenericOptions = this;
                break;
        }
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyTypeOptions() : this(Mode.Empty) { }

    /// <summary>
    /// A shared empty instance.
    /// </summary>
    public static EasyTypeOptions Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared instance with default settings.
    /// </summary>
    public static EasyTypeOptions Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared instance with full settings.
    /// </summary>
    public static EasyTypeOptions Full { get; } = new(Mode.Full);
}

// ========================================================
public static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this Type source) => source.EasyName(EasyTypeOptions.Default);

    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this Type source, EasyTypeOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var types = source.GetGenericArguments();
        return source.EasyName(types, options);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked once the details of the closed generic arguments of the original type have been
    /// obtained. Otherwise, these details will be lost after recursive calls.
    /// </summary>
    static string EasyName(this Type source, Type[] types, EasyTypeOptions options)
    {
        // Inconditional shortcut...
        if (options.HideName) return string.Empty;

        // Shortcut nullable wrappers...
        if (options.NullableStyle != IsNullableStyle.KeepWrappers &&
            source.IsNullableWrapper())
        {
            var type = source.GetGenericArguments()[0];
            var str = type.EasyName(options);

            if (str.Length > 0 && !str.EndsWith('?')) str += '?';
            return str;
        }

        // Processing...
        var sb = new StringBuilder();
        var isgen = source.IsGenericAlike();
        var host = source.DeclaringType;

        var args = source.GetGenericArguments();
        var used = host == null ? 0 : host.GetGenericArguments().Length;
        var need = args.Length - used;

        // Variance...
        if (options.UseVariance && source.IsGenericParameter)
        {
            var gpa = source.GenericParameterAttributes;
            var variance = gpa & GenericParameterAttributes.VarianceMask;

            if ((variance & GenericParameterAttributes.Covariant) != 0) sb.Append("out ");
            if ((variance & GenericParameterAttributes.Contravariant) != 0) sb.Append("in ");
        }

        // Special names...
        var xname = source switch
        {
            Type t when t == typeof(void) => "void",
            Type t when t == typeof(object) => "object",
            Type t when t == typeof(string) => "string",
            Type t when t == typeof(bool) => "bool",
            Type t when t == typeof(char) => "char",
            Type t when t == typeof(byte) => "byte",
            Type t when t == typeof(sbyte) => "sbyte",
            Type t when t == typeof(short) => "short",
            Type t when t == typeof(ushort) => "ushort",
            Type t when t == typeof(int) => "int",
            Type t when t == typeof(uint) => "uint",
            Type t when t == typeof(long) => "long",
            Type t when t == typeof(ulong) => "ulong",
            Type t when t == typeof(float) => "float",
            Type t when t == typeof(double) => "double",
            Type t when t == typeof(decimal) => "decimal",
            _ => null
        };

        // Namespace...
        if (options.UseNamespace)
        {
            var str = source.Namespace;
            if (str != null && str.Length > 0) sb.Append(str).Append('.');
        }

        // Host...
        if ((options.UseHost || options.UseNamespace) && host != null && !isgen)
        {
            var xoptions = options.WithNoHideName;
            var str = host.EasyName(types, xoptions);
            if (str.Length > 0) sb.Append(str).Append('.');
        }

        // Names...
        string name = options.UseSpecialNames ? xname! : null!;
        if (name == null)
        {
            name = source.Name;
            var index = name.IndexOf('`'); if (index >= 0) name = name[..index];
            if (name.Length > 0 && name[^1] == '&') name = name[..^1];

            if (options.RemoveAttributeSuffix &&
                name.EndsWith("Attribute"))
                name = name.RemoveLast("Attribute").ToString();
        }
        sb.Append(name);

        // Generic arguments...
        if (options.GenericOptions != null && need > 0)
        {
            sb.Append('<'); for (int i = 0; i < need; i++)
            {
                var arg = types[used + i];
                var str = arg.EasyName(options.GenericOptions);
                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append('>');
        }

        // Nullability...
        switch (options.NullableStyle)
        {
            case IsNullableStyle.KeepWrappers when source.IsNullableWrapper():
                break;

            case IsNullableStyle.KeepWrappers:
            case IsNullableStyle.UseAnnotations:
                if (source.HasNullableEnabledAttribute() &&
                    sb.Length > 0 && sb[^1] != '?')
                    sb.Append('?');
                break;
        }

        // Finishing...
        return sb.ToString();
    }
}