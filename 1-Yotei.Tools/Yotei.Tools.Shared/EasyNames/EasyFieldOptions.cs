namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to obtain the display string of a field element.
/// </summary>
public record EasyFieldOptions
{
    /// <summary>
    /// If enabled, then use the member accessibility modifiers, if any. Otherwise, they are
    /// ignored.
    /// </summary>
    public bool UseAccessibility { get; init; }

    /// <summary>
    /// If enabled and accesibility is used, then also use the 'private' modifier. In all other
    /// cases, it is ignored.
    /// </summary>
    public bool UsePrivate { get; init; }

    /// <summary>
    /// If enabled, then use the element's modifiers, if possible. Otherwise, they are ignored.
    /// </summary>
    public bool UseModifiers { get; init; }

    /// <summary>
    /// If not null, then the options to use with the element's return type. Otherwise, it is
    /// ignored.
    /// </summary>
    public EasyTypeOptions? MemberTypeOptions { get; init; }

    /// <summary>
    /// If not null, then the options to use with the element's host type. Otherwise, it is
    /// ignored.
    /// </summary>
    public EasyTypeOptions? HostTypeOptions { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyFieldOptions() { }

    /// <summary>
    /// A shared instance with empty-alike settings.
    /// </summary>
    public static EasyFieldOptions Empty { get; } = new();

    /// <summary>
    /// A shared instance with default-alike settings.
    /// </summary>
    public static EasyFieldOptions Default { get; } = new();

    /// <summary>
    /// A shared instance with full-alike settings.
    /// </summary>
    public static EasyFieldOptions Full { get; } = new()
    {
        UseAccessibility = true,
        UsePrivate = true,
        UseModifiers = true,
        MemberTypeOptions = EasyTypeOptions.Full,
        HostTypeOptions = EasyTypeOptions.Full,
    };
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
        this FieldInfo source) => source.EasyName(EasyFieldOptions.Default);

    /// <summary>
    /// Obtains a c#-alike string representation of the given element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(
        this FieldInfo source, EasyFieldOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();
        var host = source.DeclaringType;

        // Accessibility (not using 'private' for simplicity)...
        if (options.UseAccessibility)
        {
            if (source.IsPrivate && options.UsePrivate) sb.Append("private ");
            if (source.IsPublic) sb.Append("public ");
            if (source.IsFamily) sb.Append("protected ");
            if (source.IsAssembly) sb.Append("internal ");
            if (source.IsFamilyOrAssembly) sb.Append("internal protected ");
            if (source.IsFamilyAndAssembly) sb.Append("private protected ");
        }

        // Modifiers...
        if (options.UseModifiers)
        {
            if (IsNew()) sb.Append("new ");

            if (source.IsLiteral) sb.Append("const ");
            else if (source.IsStatic) sb.Append("static ");

            if (IsVolatile()) sb.Append("volatile ");
            if (source.IsInitOnly) sb.Append("readonly ");
        }

        // Member type...
        if (options.MemberTypeOptions != null)
        {
            // 'ref'-alike return types...
            if (options.UseModifiers)
            {
                var ronly =
                    source.HasReadOnlyAttribute() ||
                    source.FieldType.HasReadOnlyAttribute();

                sb.Append(ronly ? "ref readonly " : "ref ");
            }

            // The type itself...
            var xoptions = options.MemberTypeOptions;
            var type = source.FieldType;
            var str = type.EasyName(xoptions);

            // Nullability...
            while (xoptions.NullableStyle != EasyNullableStyle.None && str.Length > 0)
            {
                if (xoptions.NullableStyle == EasyNullableStyle.KeepWrappers &&
                    type.IsNullableWrapper())
                    break;

                if (str.Length > 0 && str[^1] != '?')
                {
                    if (type.IsNullableAnnotated()) { str += '?'; break; }
                    if (source.IsNullableAnnotated()) { str += '?'; break; }
                }

                break;
            }

            // Adding...
            if (str.Length > 0) sb.Append(str).Append(' ');
        }

        // Host type...
        if (options.HostTypeOptions != null && host != null)
        {
            var xoptions = options.HostTypeOptions;
            var str = host.EasyName(xoptions);
            if (str.Length > 0) sb.Append(str).Append('.');
        }

        // Name...
        sb.Append(source.Name);

        // Finishing...
        return sb.ToString();

        // ------------------------------------------------

        /// <summary>
        /// Determines if the field is a volatile one.
        /// </summary>
        bool IsVolatile() =>
            source.GetRequiredCustomModifiers().Any(x => x == typeof(IsVolatile) ||
            source.GetOptionalCustomModifiers().Any(x => x == typeof(IsVolatile)));

        /// <summary>
        /// Determines if the given field is a new one.
        /// </summary>
        bool IsNew() => FindBaseField(host) != null;

        FieldInfo? FindBaseField(Type? host)
        {
            if (host != null)
            {
                var parent = host.BaseType;
                while (parent != null)
                {
                    var temp = parent.GetField(
                        source.Name,
                        BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.Static |
                        BindingFlags.DeclaredOnly);

                    if (temp != null) return temp;
                    parent = parent.BaseType;
                }
            }
            return null;
        }
    }
}