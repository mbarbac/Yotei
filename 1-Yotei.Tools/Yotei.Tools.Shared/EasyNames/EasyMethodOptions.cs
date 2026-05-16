namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to obtain a C#-alike representation of a given method-alike element.
/// </summary>
public sealed record EasyMethodOptions
{
    /// <summary>
    /// If enabled, then use accessibility modifiers, if any. Otherwise, they are ignored.
    /// </summary>
    public bool UseAccessibility { get; set; }

    /// <summary>
    /// If enabled, then use method's modifiers (such as static, abstract, virtual, override,
    /// new, and ref-alike ones). Otherwise, they are ignored.
    /// </summary>
    public bool UseModifiers { get; set; }

    /// <summary>
    /// If not null, then the options to use to obtain the easy name of the method's return type.
    /// If null, then it is ignored.
    /// </summary>
    public EasyTypeOptions? ReturnTypeOptions { get; set; }

    /// <summary>
    /// If not null, then the options to use to obtain the easy name of the method's host type.
    /// If null, then it is ignored.
    /// </summary>
    public EasyTypeOptions? HostTypeOptions { get; set; }

    /// <summary>
    /// If enabled, and if the method is a constructor-alike one, then add the CLR name to the
    /// display string. Otherwise, the host plain name is used instead.
    /// </summary>
    public bool UseTechName { get; set; }

    /// <summary>
    /// If not null, then the use these options to obtain the list of generic type arguments. If
    /// null, then that list is ignored. When not null, then the value of this property typically
    /// refers to this host instance in a recursive fashion.
    /// </summary>
    public EasyTypeOptions? GenericListOptions { get; set; }

    /// <summary>
    /// If enabled, then use parenthesis even if no parameter options were specified.
    /// </summary>
    public bool UseBrackets { get; set; }

    /// <summary>
    /// If not null, then the options to obtain the display string of the method's arguments. If
    /// null, then they are ignored.
    /// </summary>
    public EasyParameterOptions? ParameterOptions { get; set; }

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
        if (ReturnTypeOptions != null) Append($"ReturnOptions:{ReturnTypeOptions.Id}");
        if (HostTypeOptions != null) Append($"HostOptions:{HostTypeOptions.Id}");
        if (UseTechName) Append(nameof(UseTechName));
        if (GenericListOptions != null) Append($"GenericOptions:{GenericListOptions.Id}");
        if (UseBrackets) Append(nameof(UseBrackets));
        if (ParameterOptions != null) Append($"ParameterOptions:{ParameterOptions}");
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
    EasyMethodOptions(Mode mode)
    {
        UseAccessibility = false;
        UseModifiers = false;
        ReturnTypeOptions = null;
        HostTypeOptions = null;
        UseTechName = false;
        GenericListOptions = null;
        UseBrackets = false;
        ParameterOptions = null;

        switch (mode)
        {
            case Mode.Default:
                GenericListOptions = EasyTypeOptions.Default;
                UseBrackets = true;
                ParameterOptions = EasyParameterOptions.Default;
                break;

            case Mode.Full:
                UseAccessibility = true;
                UseModifiers = true;
                ReturnTypeOptions = EasyTypeOptions.Full;
                HostTypeOptions = EasyTypeOptions.Full;
                UseTechName = true;
                GenericListOptions = EasyTypeOptions.Full;
                UseBrackets = true;
                ParameterOptions = EasyParameterOptions.Full;
                break;
        }
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyMethodOptions() : this(Mode.Empty) { }

    /// <summary>
    /// Obtains a new instance with empty-alike values that obtains the bare minimum display
    /// string.
    /// </summary>
    public static EasyMethodOptions Empty => new(Mode.Empty);

    /// <summary>
    /// Obtains a new instance with default-alike values that obtains the most common display
    /// string.
    /// </summary>
    public static EasyMethodOptions Default => new(Mode.Default);

    /// <summary>
    /// Obtains a new instance with full-alike values that obtains a full display string.
    /// </summary>
    public static EasyMethodOptions Full => new(Mode.Full);
}