namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal record EasyPropertySymbol
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
    public EasyTypeSymbol? ReturnTypeOptions { get; set; }

    /// <summary>
    /// If not null, the options to include the host type of the member. If null, it is ignored.
    /// </summary>
    public EasyTypeSymbol? HostTypeOptions { get; set; }

    /// <summary>
    /// If the property is an indexed one, use its CLR name instead of '<see langword="this"/>'.
    /// </summary>
    public bool UseTechName { get; set; }

    /// <summary>
    /// If enabled, include member brackets, even if <see cref="ParameterOptions"/> is null. This
    /// setting is only used with indexed properties.
    /// </summary>
    public bool UseBrackets { get; set; }

    /// <summary>
    /// If not null, the options to include the parameters of the member. This setting is only used
    /// with indexed properties. If null, they are ignored.
    /// </summary>
    public EasyParameterSymbol? ParameterOptions { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with a set of default code generation settings.
    /// </summary>
    public static EasyPropertySymbol Default => new()
    {
        UseAccessibility = true,
        UseModifiers = true,
        ReturnTypeOptions = EasyTypeSymbol.Default,
        UseBrackets = true,
        ParameterOptions = EasyParameterSymbol.Default,
    };

    /// <summary>
    /// Returns a new instance with full settings.
    /// </summary>
    public static EasyPropertySymbol Full => new()
    {
        UseAccessibility = true,
        UseModifiers = true,
        ReturnTypeOptions = EasyTypeSymbol.Full,
        HostTypeOptions = EasyTypeSymbol.Full,
        UseTechName = true,
        UseBrackets = true,
        ParameterOptions = EasyParameterSymbol.Full,
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
    public static string EasyName(
        this IPropertySymbol source) => source.EasyName(EasyPropertySymbol.Default);

    /// <summary>
    /// Returns a display string for the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IPropertySymbol source, EasyPropertySymbol options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();
        var host = source.ContainingType ?? throw new ArgumentException("Element's containing type is null.").WithData(source);

        // Accessibility...
        if (options.UseAccessibility)
        {
            var temp = source.DeclaredAccessibility.ToAccessibilityString();
            if (temp != null) sb.Append(temp).Append(' ');
        }

        // Modifiers...
        if (options.UseModifiers && options.ReturnTypeOptions != null)
        {
            if (source.IsSealed) sb.Append("sealed ");
            if (source.IsStatic) sb.Append("static ");
            if (source.IsVirtual) sb.Append("virtual ");
            if (source.IsOverride) sb.Append("override ");
            if (source.IsAbstract) sb.Append("abstract ");
            if (source.IsNew) sb.Append("new ");
            if (source.IsPartialDefinition) sb.Append("partial ");

            var str = source.RefKind switch
            {
                RefKind.Ref => "ref",
                RefKind.Out => "out",
                RefKind.In => "ref readonly",
                _ => null
            };
            if (str != null) sb.Append(str).Append(' ');
        }

        // Return type...
        if (options.ReturnTypeOptions != null)
        {
            var xoptions = options.ReturnTypeOptions.DisabledHideName();
            var str = source.Type.EasyName(xoptions);
            sb.Append(str).Append(' ');
        }

        // Host type...
        if (options.HostTypeOptions != null && host != null)
        {
            var xoptions = options.HostTypeOptions.DisabledHideName();
            var str = host.EasyName(xoptions);
            sb.Append(str).Append('.');
        }

        // Name...
        var name = source.Name;
        var args = source.Parameters;
        if (args.Length > 0) name = options.UseTechName ? source.MetadataName : "this";
        sb.Append(name);

        // Parameters...
        if (args.Length > 0 && (options.UseBrackets || options.ParameterOptions != null))
        {
            sb.Append('['); if (options.ParameterOptions != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var str = EasyName(arg, options.ParameterOptions);
                    if (i > 0) sb.Append(str.Length > 0 ? ", " : ",");
                    sb.Append(str);
                }
            }
            sb.Append(']');
        }

        // Finishing...
        return sb.ToString();
    }
}