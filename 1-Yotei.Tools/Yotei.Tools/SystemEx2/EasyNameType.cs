namespace Yotei.Tools;

// ========================================================
public static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains a C#-alike name for the given element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this Type source) => source.EasyName(EasyNameType.Default);

    /// <summary>
    /// Obtains a C#-alike name for the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this Type source, EasyNameType options)
    {
        options.ThrowWhenNull();
        return options.EasyName(source);
    }
}

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities to <see cref="Type"/> instances.
/// </summary>
public record EasyNameType
{
    /// <summary>
    /// Determines if the variance specifiers ('in', 'out') of the type element, when it is a
    /// generic parameter, are used or not.
    /// </summary>
    public bool UseVarianceMask { get; init; }

    /// <summary>
    /// Determines if the namespace of the type element is used or not.
    /// </summary>
    public bool UseNamespace { get; init; }

    /// <summary>
    /// Determines if the host type of the nested type element is used or not.
    /// </summary>
    public bool UseHost { get; init; }

    /// <summary>
    /// Determines if the name of the type element is hidden or not. This setting is mostly used
    /// with generic argument types to hide the 'T' or similar names. If this setting is enabled,
    /// then it shorcuts any other ones.
    /// </summary>
    public bool HideName { get; init; }

    /// <summary>
    /// Determines the nullability style used with the type element.
    /// <br/> Note that you may obtain inconsistent results when requesting annotations and you
    /// annotate both value and reference types. By default, the former keeps the annotation while
    /// the later may not. The <see cref="IsNullable{T}"/> and <see cref="IsNullableAttribute"/>
    /// types can be used as workarounds.
    /// </summary>
    public EasyNullableStyle NullableStyle { get; init; }

    /// <summary>
    /// Determines if the generic arguments of the type element are used, or not.
    /// </summary>
    public bool UseGenericArguments { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// A shared instance with empty options.
    /// </summary>
    public static EasyNameType Empty { get; } = new EasyNameType(Mode.Empty);

    /// <summary>
    /// A shared instance with default options.
    /// </summary>
    public static EasyNameType Default { get; } = new EasyNameType(Mode.Default);

    /// <summary>
    /// A shared instance with full options.
    /// </summary>
    public static EasyNameType Full { get; } = new EasyNameType(Mode.Full);

    /// <summary>
    /// Determines the mode to use when initializing this instance.
    /// </summary>
    enum Mode { Empty, Default, Full }
    EasyNameType(Mode mode)
    {
        switch (mode)
        {
            case Mode.Empty:
                HideName = true;
                break;

            default:
            case Mode.Default:
                NullableStyle = EasyNullableStyle.None;
                UseGenericArguments = true;
                break;

            case Mode.Full:
                UseVarianceMask = true;
                UseNamespace = true;
                UseHost = true;
                NullableStyle = EasyNullableStyle.UseWrappers;
                UseGenericArguments = true;
                break;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains a C#-alike name for the given element.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public string EasyName(Type source)
    {
        source.ThrowWhenNull();

        var types = source.GetGenericArguments();
        return EasyName(source, types);
    }

    /// <summary>
    /// Invoked to obtain the C#-alike name of the given element, once the details of its closed
    /// generic arguments have been captured, to prevent loosing them in recursive calls.
    /// </summary>
    string EasyName(Type source, Type[] types)
    {
        var isgen = source.FullName == null;
        var host = source.DeclaringType;
        var args = source.GetGenericArguments();
        var used = host is null ? 0 : host.GetGenericArguments().Length;
        var need = args.Length - used;

        // Shortcut hide name...
        if (HideName) return string.Empty;

        // Shortcut wrapped nullability...
        if (NullableStyle is not EasyNullableStyle.UseWrappers)
        {
            if ((need == 1 && source.Name.StartsWith("Nullable`1")) ||
                (need == 1 && source.Name.StartsWith("IsNullable`1")))
            {
                var type = types[used];
                var str = EasyName(type);
                if (NullableStyle is EasyNullableStyle.UseAnnotation &&
                    !str.EndsWith('?'))
                    str += '?';

                return str;
            }
        }

        // Processing...
        var sb = new StringBuilder();

        // Variance mask...
        if (UseVarianceMask && source.IsGenericParameter)
        {
            var gpa = source.GenericParameterAttributes;
            var variance = gpa & GenericParameterAttributes.VarianceMask;

            if ((variance & GenericParameterAttributes.Covariant) != 0) sb.Append("out ");
            if ((variance & GenericParameterAttributes.Contravariant) != 0) sb.Append("in ");
        }

        // Namespace...
        if (UseNamespace && host == null && !isgen)
        {
            var str = source.Namespace;
            if (str is not null && str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Host...
        if (UseHost && host != null && !isgen)
        {
            var str = EasyName(host, types);
            if (str is not null && str.Length > 0) { sb.Append(str); sb.Append('.'); }
        }

        // Name (if 'name&' is a 'ref' type, but such is handled elsewhere)...
        var name = source.Name;
        var index = name.IndexOf('`'); if (index >= 0) name = name[..index];
        if (name.Length > 0 && name[^1] == '&') name = name[..^1];
        sb.Append(name);

        // Generic arguments...
        if (UseGenericArguments && need > 0)
        {
            sb.Append('<'); for (var i = 0; i < need; i++)
            {
                var arg = types[used + i];
                var str = EasyName(arg);
                if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
            }
            sb.Append('>');
        }

        // Nullability...
        if (NullableStyle is not EasyNullableStyle.None && sb.Length > 0 && sb[^1] != '?')
        {
            if (NullableStyle is EasyNullableStyle.UseWrappers && (
                source.Name.StartsWith("Nullable`1") ||
                source.Name.StartsWith("IsNullable`1"))) goto END;

            // Nullable attribute (API)...
            var nullability = source.GetCustomAttribute<NullableAttribute>();
            if (nullability != null && nullability.NullableFlags.Length > 0)
            {
                var flag = nullability.NullableFlags[0];
                if (flag == 2) { sb.Append('?'); goto END; }
            }

            // IsNullable attribute...
            if (source.GetCustomAttribute<IsNullableAttribute>() != null)
            { sb.Append('?'); goto END; }
        }

        // Finishing...
        END:
        return sb.ToString();
    }
}