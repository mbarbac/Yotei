namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Determines the style to use with nullable elements.
/// </summary>
public enum EasyNullableStyle { None, Annotated, Wrapped }

// ========================================================
/// <summary>
/// Provides options for the 'EasyName' family of methods.
/// </summary>
public class EasyNameOptions
{
    /// <summary>
    /// Determines if the 'in' and 'out' variance specifiers of the type are used or not.
    /// </summary>
    public bool TypeUseVarianceMask { get; set; }

    /// <summary>
    /// Determines if type elements use their namespace, or not.
    /// </summary>
    public bool TypeUseNamespace { get; set; }

    /// <summary>
    /// If not null, and it the type is a nested one, the options to use to print the type host.
    /// If null, then it is ignored.
    /// </summary>
    public EasyNameOptions? TypeHostOptions { get; set; }

    /// <summary>
    /// Determines if type elements hide their name, or not. If this setting is enabled, then
    /// it shorcuts other settings related to the type elements.
    /// </summary>
    public bool TypeHideName { get; set; }

    /// <summary>
    /// Determines the style to use with nullable types, or annotated or wrapped ones.
    /// </summary>
    public EasyNullableStyle TypeNullableStyle
    {
        get;
        set => field = value switch
        {
            EasyNullableStyle.None => value,
            EasyNullableStyle.Annotated => value,
            EasyNullableStyle.Wrapped => value,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }

    /// <summary>
    /// If not null, the options to use to print the generic arguments of the type. If null,
    /// then they are ignored.
    /// </summary>
    public EasyNameOptions? TypeGenericArgumentOptions { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the member modifiers shall be used or not.
    /// </summary>
    public bool MemberUseModifiers { get; set; }

    /// <summary>
    /// If not null, the options to use to print the return type of the member, if any. If null,
    /// then it is ignored.
    /// </summary>
    public EasyNameOptions? MemberReturnTypeOptions { get; set; }

    /// <summary>
    /// If not null, the options to use to print the member host. If null, then it is ignored.
    /// </summary>
    public EasyNameOptions? MemberHostOptions { get; set; }

    /// <summary>
    /// If not null, the options to use to print the generic arguments of the member. If null,
    /// then they are ignored.
    /// </summary>
    public EasyNameOptions? MemberGenericArgumentOptions { get; set; }

    /// <summary>
    /// Determines if constructors shall use their tech name, instead of the default 'new' one.
    /// </summary>
    public bool UseConstructorTechName { get; set; }

    /// <summary>
    /// Determines if indexers shall use their tech name, instead of the default 'this' one.
    /// </summary>
    public bool UseIndexerTechName { get; set; }

    /// <summary>
    /// Determines if, even if no argument options are provided for the member, at least its
    /// brakets shall be printed or not.
    /// </summary>
    public bool MemberUseBrackets { get; set; }

    /// <summary>
    /// If not null, the options to use to print the arguments of the member. If null, then they
    /// are ignored.
    /// </summary>
    public EasyNameOptions? MemberArgumentOptions { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if parameter modifiers (ref, out, in, etc.) are used or not.
    /// </summary>
    public bool ParameterUseModifiers { get; set; }

    /// <summary>
    /// If not null, the options to use to print the type of parameters. If null, then they are
    /// ignored.
    /// </summary>
    public EasyNameOptions? ParameterTypeOptions { get; set; }

    /// <summary>
    /// Determines if the names of the parameters are used or not.
    /// </summary>
    public bool ParameterUseName { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance that represents empty options.
    /// </summary>
    public static EasyNameOptions Empty => new(Mode.Empty);

    /// <summary>
    /// Returns a new instance that represents default options.
    /// </summary>
    public static EasyNameOptions Default => new(Mode.Default);

    /// <summary>
    /// Returns a new instance that represents full options.
    /// </summary>
    public static EasyNameOptions Full => new(Mode.Full);

    /// <summary>
    /// Initializes a new instance with default options.
    /// </summary>
    public EasyNameOptions() : this(Mode.Default) { }

    /// <summary>
    /// Used to determine what mode is used when initializing this instance. The main ideas are:
    /// - Empty: prints an empty string.
    /// - Default: just prints the name of the element.
    /// - Full: prints the most complete name of the element.
    /// </summary>
    enum Mode { Empty, Default, Full }
    EasyNameOptions(Mode mode)
    {
        switch (mode)
        {
            case Mode.Empty:
                TypeHideName = true;
                break;

            default:
            case Mode.Default:
                ParameterTypeOptions = this;
                break;

            case Mode.Full:
                TypeUseVarianceMask = true;
                TypeUseNamespace = true;
                TypeHostOptions = this;
                TypeNullableStyle = EasyNullableStyle.Wrapped;
                TypeGenericArgumentOptions = this;
                MemberReturnTypeOptions = this;
                MemberHostOptions = this;
                MemberGenericArgumentOptions = this;
                UseConstructorTechName = true;
                UseIndexerTechName = true;
                MemberUseBrackets = true;
                MemberArgumentOptions = this;
                ParameterTypeOptions = this;
                break;
        }
    }
}