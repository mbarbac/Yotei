namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal record EasyField
{
    /// <summary>
    /// Include the accessibility modifiers of the member (ie: public).
    /// </summary>
    public bool UseAccessibility { get; set; }

    /// <summary>
    /// Include the modifiers of the member (ie: static readonly).
    /// </summary>
    public bool UseModifiers { get; set; }

    /// <summary>
    /// If not null, the options to include the type of the property. If null, it is ignored.
    /// </summary>
    public EasyType? ReturnTypeOptions { get; set; }

    /// <summary>
    /// If not null, the options to include the host type of the member. If null, it is ignored.
    /// </summary>
    public EasyType? HostTypeOptions { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with a set of default code generation settings.
    /// </summary>
    public static EasyField Default => new()
    {
        UseAccessibility = true,
        UseModifiers = true,
        ReturnTypeOptions = EasyType.Default,
    };

    /// <summary>
    /// Returns a new instance with full settings.
    /// </summary>
    public static EasyField Full => new()
    {
        UseAccessibility = true,
        UseModifiers = true,
        ReturnTypeOptions = EasyType.Full,
        HostTypeOptions = EasyType.Full,
    };
}

// ========================================================
internal static partial class EasyNameExtensions
{
    /// <summary>
    /// Returns a display string for the given element using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this IFieldSymbol source) => source.EasyName(new());

    /// <summary>
    /// Returns a display string for the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IFieldSymbol source, EasyField options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();
        var host = source.ContainingType;

        // Accessibility...
        if (options.UseAccessibility)
        {
            var temp = source.DeclaredAccessibility.ToAccesibilityString();
            if (temp != null) sb.Append(temp).Append(' ');
        }

        // Modifiers...
        if (options.UseModifiers && options.ReturnTypeOptions != null)
        {
            if (source.IsSealed) sb.Append("sealed ");
            if (source.IsStatic) sb.Append("static ");
            if (source.IsNew) sb.Append("new ");
            if (source.IsVolatile) sb.Append("volatile ");

            var str = source.RefKind switch
            {
                RefKind.Ref => "ref",
                RefKind.In => "ref readonly",
                _ => null
            };
            if (str != null) sb.Append(str).Append(' ');
        }

        // Return type...
        if (options.ReturnTypeOptions != null)
        {
            var xoptions = options.ReturnTypeOptions with { HideName = false };
            var str = source.Type.EasyName(xoptions);
            sb.Append(str).Append(' ');
        }

        // Host type...
        if (options.HostTypeOptions != null && host != null)
        {
            var xoptions = options.HostTypeOptions with { HideName = false };
            var str = host.EasyName(xoptions);
            sb.Append(str).Append('.');
        }

        // Name...
        sb.Append(source.Name);

        // Finishing...
        return sb.ToString();
    }
}