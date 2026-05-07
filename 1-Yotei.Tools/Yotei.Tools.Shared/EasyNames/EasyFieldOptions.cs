namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to obtain a C#-alike representation of a given field-alike element.
/// </summary>
public record EasyFieldOptions
{
    /// <summary>
    /// If enabled, then use the accessibility modifiers, if any.
    /// </summary>
    public bool UseAccessibility { get; set; }

    /// <summary>
    /// If enabled, then use the element's modifiers (such as static, abstract, virtual, override,
    /// new, and ref-alike ones). Otherwise, they are ignored.
    /// </summary>
    public bool UseModifiers { get; set; }

    /// <summary>
    /// If not null, then the options to use to include the element's type.
    /// </summary>
    public EasyTypeOptions? MemberTypeOptions { get; set; }

    /// <summary>
    /// If not null, then the options to use to include the element's host type, if any.
    /// </summary>
    public EasyTypeOptions? HostTypeOptions { get; set; }

    // ----------------------------------------------------

    public enum Mode { Empty, Default, DefaultEx, Full };
    EasyFieldOptions(Mode mode)
    {
        UseAccessibility = false;
        UseModifiers = false;
        MemberTypeOptions = null;
        HostTypeOptions = null;

        switch (mode)
        {
            case Mode.DefaultEx:
                HostTypeOptions = EasyTypeOptions.DefaultEx;
                break;

            case Mode.Full:
                UseAccessibility = true;
                UseModifiers = true;
                MemberTypeOptions = EasyTypeOptions.Full;
                HostTypeOptions = EasyTypeOptions.Full;
                break;
        }
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyFieldOptions() : this(Mode.Empty) { }

    /// <summary>
    /// Obtains a new empty-alike instance.
    /// </summary>
    public static EasyFieldOptions Empty => new(Mode.Empty);

    /// <summary>
    /// Obtains a new default-alike instance.
    /// </summary>
    public static EasyFieldOptions Default => new(Mode.Default);

    /// <summary>
    /// Obtains a new default-alike instance whose type options are default extended ones.
    /// </summary>
    public static EasyFieldOptions DefaultEx => new(Mode.DefaultEx);

    /// <summary>
    /// Obtains a new full-alike instance.
    /// </summary>
    public static EasyFieldOptions Full => new(Mode.Full);
}

// ========================================================
public static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains a C#-alike representation for a given property-alike element, using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this FieldInfo source) => source.EasyName(new EasyFieldOptions());

    /// <summary>
    /// Obtains a C#-alike representation for a given property-alike element, using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this FieldInfo source, EasyFieldOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();
        var host = source.DeclaringType;

        // Accessibility...
        if (options.UseAccessibility)
        {
            if (source.IsPublic) sb.Append("public ");
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
            if (IsNew()) sb.Append("new ");

            if (source.IsLiteral) sb.Append("const ");
            else if (source.IsStatic) sb.Append("static ");

            if (IsVolatile()) sb.Append("volatile ");
            if (source.IsInitOnly) sb.Append("readonly ");
        }

        // Member type...
        if (options.MemberTypeOptions != null)
        {
            var xoptions = options.MemberTypeOptions;
            var arg = source.FieldType;
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
            }

            // Ref-alike types (using method first, arg as a fallback)...
            if (options.UseModifiers && arg.IsByRef)
            {
                var ronly =
                    source.HasReadOnlyAttribute() ||
                    source.FieldType.HasReadOnlyAttribute();

                sb.Append(ronly ? "ref readonly " : "ref ");
            }

            // Adding...
            sb.Append(str).Append(' ');
        }

        // Host type...
        if (options.HostTypeOptions != null && host != null)
        {
            var str = host.EasyName(options.HostTypeOptions);
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