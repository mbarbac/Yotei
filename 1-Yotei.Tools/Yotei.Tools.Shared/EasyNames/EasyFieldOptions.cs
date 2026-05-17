namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to obtain a C#-alike representation of a given parameter-alike element.
/// </summary>
public sealed record EasyFieldOptions
{
    /// <summary>
    /// If enabled, then use accessibility modifiers, if any. Otherwise, they are ignored.
    /// </summary>
    public bool UseAccessibility { get; set; }

    /// <summary>
    /// If enabled, then use member's modifiers (such as static, abstract, virtual, override,
    /// new, and ref-alike ones). Otherwise, they are ignored.
    /// </summary>
    public bool UseModifiers { get; set; }

    /// <summary>
    /// If not null, then the options to use to obtain the easy name of the member's type. If
    /// null, then it is ignored.
    /// </summary>
    public EasyTypeOptions? MemberTypeOptions { get; set; }

    /// <summary>
    /// If not null, then the options to use to obtain the easy name of the member's host type.
    /// If null, then it is ignored.
    /// </summary>
    public EasyTypeOptions? HostTypeOptions { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var num = 0;
        var sb = new StringBuilder();
        sb.Append('{');
        if (UseAccessibility) Append(nameof(UseAccessibility));
        if (UseModifiers) Append(nameof(UseModifiers));
        if (MemberTypeOptions != null) Append($"ReturnOptions:{MemberTypeOptions.Id}");
        if (HostTypeOptions != null) Append($"HostOptions:{HostTypeOptions.Id}");
        sb.Append(" }");
        return sb.ToString();

        void Append(string value)
        {
            if (num != 0) sb.Append(", "); num++;
            sb.Append(value);
        }
    }

    // Internal constructor
    public enum Mode { Empty, Default, Full };
    EasyFieldOptions(Mode mode)
    {
        UseAccessibility = false;
        UseModifiers = false;
        MemberTypeOptions = null;
        HostTypeOptions = null;

        switch (mode)
        {
            case Mode.Default:
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
    /// Obtains a new instance with empty-alike values that obtains the bare minimum display
    /// string.
    /// </summary>
    public static EasyFieldOptions Empty => new(Mode.Empty);

    /// <summary>
    /// Obtains a new instance with default-alike values that obtains the most common display
    /// string.
    /// </summary>
    public static EasyFieldOptions Default => new(Mode.Default);

    /// <summary>
    /// Obtains a new instance with full-alike values that obtains a full display string.
    /// </summary>
    public static EasyFieldOptions Full => new(Mode.Full);
}