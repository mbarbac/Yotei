namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal record EasyEventSymbol
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
    /// If not null, the options to include the return type of the event. If null, it is ignored.
    /// </summary>
    public EasyTypeSymbol? ReturnTypeOptions { get; set; }

    /// <summary>
    /// If not null, the options to include the host type of the member. If null, it is ignored.
    /// </summary>
    public EasyTypeSymbol? HostTypeOptions { get; set; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    EasyEventSymbol(Mode mode)
    {
        switch (mode)
        {
            case Mode.Full:
                UseAccessibility = true;
                UseModifiers = true;
                ReturnTypeOptions = EasyTypeSymbol.Full;
                HostTypeOptions = EasyTypeSymbol.Full;
                break;

            case Mode.Default:
                UseAccessibility = true;
                UseModifiers = true;
                ReturnTypeOptions = EasyTypeSymbol.Default;
                break;
        }
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyEventSymbol() : this(Mode.Empty) { }

    /// <summary>
    /// A shared instance with default-alike settings.
    /// </summary>
    public static EasyEventSymbol Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared instance with full-alike settings.
    /// </summary>
    public static EasyEventSymbol Full { get; } = new(Mode.Full);
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
        this IEventSymbol source) => source.EasyName(EasyEventSymbol.Default);

    /// <summary>
    /// Returns a display string for the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IEventSymbol source, EasyEventSymbol options)
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
        }

        // Return type...
        if (options.ReturnTypeOptions != null)
        {
            var xoptions = options.ReturnTypeOptions.WithNoHideName();
            var str = source.Type.EasyName(xoptions);
            sb.Append(str).Append(' ');
        }

        // Host type...
        if (options.HostTypeOptions != null && host != null)
        {
            var xoptions = options.HostTypeOptions.WithNoHideName();
            var str = host.EasyName(xoptions);
            sb.Append(str).Append('.');
        }

        // Name...
        sb.Append(source.Name);

        // Finishing...
        return sb.ToString();
    }
}