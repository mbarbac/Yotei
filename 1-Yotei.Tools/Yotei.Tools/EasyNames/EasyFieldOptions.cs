namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for field instances.
/// </summary>
public record EasyFieldOptions
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
    /// If not null, the options to use with the member type. Otherwise, it is ignored.
    /// </summary>
    public EasyTypeOptions? MemberTypeOptions { get; init; }

    /// <summary>
    /// If not null, the options to use with the method host type. Otherwise, it is ignored.
    /// </summary>
    public EasyTypeOptions? HostTypeOptions { get; init; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    EasyFieldOptions(Mode mode)
    {
        switch (mode)
        {
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
    /// A shared empty instance.
    /// </summary>
    public static EasyFieldOptions Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared instance with default settings.
    /// </summary>
    public static EasyFieldOptions Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared instance with full settings.
    /// </summary>
    public static EasyFieldOptions Full { get; } = new(Mode.Full);
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
    public static string EasyName(this FieldInfo source, EasyFieldOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();
        var host = source.DeclaringType;

        // Accesibility (not using 'private' for simplicity)...
        if (options.UseAccessibility)
        {
            if (source.IsPublic) sb.Append("public ");
            if (source.IsFamily) sb.Append("protected ");
            if (source.IsAssembly) sb.Append("internal ");
            if (source.IsFamilyOrAssembly) sb.Append("protected internal ");
            if (source.IsFamilyAndAssembly) sb.Append("private protected ");
        }

        // Modifiers...
        if (options.UseModifiers)
        {
            if (IsNew()) sb.Append("new ");

            if (source.IsLiteral) sb.Append("const ");
            else if (source.IsStatic) sb.Append("static ");

            if (source.IsInitOnly) sb.Append("readonly ");           

            bool IsNew() => BaseField(host?.BaseType) != null;

            FieldInfo? BaseField(Type? parent)
            {
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
                return null;
            }
        }

        // Member type...
        if (options.MemberTypeOptions != null)
        {
            if (options.UseModifiers && source.FieldType.IsByRef)
            {
                var ronly =
                    source.HasReadOnlyAttribute() ||
                    source.FieldType.HasReadOnlyAttribute();
                
                sb.Append(ronly ? "ref readonly " : "ref ");
            }

            var xoptions = options.MemberTypeOptions.NoHideName();
            var str = source.FieldType.EasyName(xoptions);

            if (str.Length > 0 &&
                str[^1] != '?' && (
                source.HasNullableEnabledAttribute() || source.IsNullableByApi()))
            {
                if (xoptions.NullableStyle == EasyNullableStyle.KeepWrappers &&
                    source.FieldType.IsNullableWrapper())
                    goto ENDNULLABLE;

                if (xoptions.NullableStyle != EasyNullableStyle.None) str += '?';
            }

            ENDNULLABLE:
            sb.Append(str).Append(' ');
        }

        // Host type...
        if (options.HostTypeOptions != null && host != null)
        {
            var xoptions = options.HostTypeOptions.NoHideName();
            var str = host.EasyName(xoptions);
            if (str.Length > 0) sb.Append(str).Append('.');
        }

        // Name & Finishing...
        sb.Append(source.Name);
        return sb.ToString();
    }
}