namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to obtain an alternate C#-alike representation of a given element.
/// </summary>
public sealed record SketchOptions
{
    /// <summary>
    /// If not null, then the options to use with type-alike reflection elements.
    /// </summary>
    public EasyTypeOptions? TypeOptions { get; set; }

    /// <summary>
    /// If not null, then the options to use with method-alike reflection elements.
    /// </summary>
    public EasyMethodOptions? MethodOptions { get; set; }

    /// <summary>
    /// If not null, then the options to use with parameter-alike reflection elements.
    /// </summary>
    public EasyParameterOptions? ParameterOptions { get; set; }

    /// <summary>
    /// If not null, then the options to use with property-alike reflection elements.
    /// </summary>
    public EasyPropertyOptions? PropertyOptions { get; set; }

    /// <summary>
    /// If not null, then the options to use with field-alike reflection elements.
    /// </summary>
    public EasyFieldOptions? FieldOptions { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// If not null, then the options to obtain the element's type as a prefix.
    /// </summary>
    public EasyTypeOptions? HeadOptions { get; set; }

    /// <summary>
    /// If enabled, then ignore any 'ToString' methods in the element's type or in its inheritance
    /// chain.
    /// </summary>
    public bool PreventToString { get; set; }

    /// <summary>
    /// If not null, then the format string to use with a suitable 'ToString' method, if any.
    /// </summary>
    public string? FormatString { get; set; }

    /// <summary>
    /// If not null, then the format provider to use with a suitable 'ToString' method, if any.
    /// </summary>
    public IFormatProvider? FormatProvider { get; set; }

    /// <summary>
    /// If not null, then the string used to represent NULL values. If null, then an empty string
    /// is used instead.
    /// </summary>
    public string? NullString { get; set; }

    /// <summary>
    /// If enabled, the use the element's shape if the all other ways to obtain the alternate
    /// string representation have not succeeded. Unless otherwise requested, only the public
    /// members are used.
    /// </summary>
    public bool UseShape { get; set; }

    /// <summary>
    /// If enabled and shape is used, then also include the private members.
    /// </summary>
    public bool UsePrivateMembers { get; set; }

    /// <summary>
    /// If enabled and shape is used, then also include the static members.
    /// </summary>
    public bool UseStaticMembers { get; set; }

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
        
        if (TypeOptions != null) Append($"TypeOptions:{TypeOptions.Id}");
        if (MethodOptions != null) Append($"MethodOptions:{MethodOptions}");
        if (ParameterOptions != null) Append($"ParameterOptions:{ParameterOptions}");
        if (PropertyOptions != null) Append($"ParameterOptions:{PropertyOptions}");
        if (FieldOptions != null) Append($"ParameterOptions:{FieldOptions}");

        if (HeadOptions != null) Append($"HeadOptions:{HeadOptions.Id}");
        if (PreventToString) Append(nameof(PreventToString));
        if (FormatString != null) Append($"FormatString:'{FormatString}'");
        if (FormatProvider != null) Append($"FormatProvider:'{FormatProvider}'");
        if (NullString != null) Append($"NullString:'{NullString}'");
        if (UseShape) Append(nameof(UseShape));
        if (UsePrivateMembers) Append(nameof(UsePrivateMembers));
        if (UseStaticMembers) Append(nameof(UseStaticMembers));

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
    SketchOptions(Mode mode)
    {
        TypeOptions = null;
        MethodOptions = null;
        ParameterOptions = null;
        PropertyOptions = null;
        FieldOptions = null;

        HeadOptions = null;
        PreventToString = false;
        FormatString = null;
        FormatProvider = null;
        NullString = null;
        UseShape = false;
        UsePrivateMembers = false;
        UseStaticMembers = false;

        switch (mode)
        {
            case Mode.Default:
                TypeOptions = EasyTypeOptions.Default;
                MethodOptions = EasyMethodOptions.Default;
                ParameterOptions = EasyParameterOptions.Default;
                PropertyOptions = EasyPropertyOptions.Default;
                FieldOptions = EasyFieldOptions.Default;        
                
                NullString = "NULL";
                UseShape = true;
                break;

            case Mode.Full:
                TypeOptions = EasyTypeOptions.Full;
                MethodOptions = EasyMethodOptions.Full;
                ParameterOptions = EasyParameterOptions.Full;
                PropertyOptions = EasyPropertyOptions.Full;
                FieldOptions = EasyFieldOptions.Full;

                HeadOptions = EasyTypeOptions.Full;
                NullString = "NULL";
                UseShape = true;
                break;
        }
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public SketchOptions() : this(Mode.Empty) { }

    /// <summary>
    /// Obtains a new instance with empty-alike values that obtains the bare minimum display
    /// string.
    /// </summary>
    public static SketchOptions Empty => new(Mode.Empty);

    /// <summary>
    /// Obtains a new instance with default-alike values that obtains the most common display
    /// string.
    /// </summary>
    public static SketchOptions Default => new(Mode.Default);

    /// <summary>
    /// Obtains a new instance with full-alike values that obtains a full display string.
    /// </summary>
    public static SketchOptions Full => new(Mode.Full);
}