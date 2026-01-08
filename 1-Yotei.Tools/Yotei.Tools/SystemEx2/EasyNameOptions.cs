namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the 'EasyName' family of methods.
/// </summary>
public record EasyNameOptions
{
    /// <summary>
    /// Determines if the 'in' and 'out' variance specifiers of the type are used or not.
    /// </summary>
    public bool TypeUseVarianceMask { get; init; }

    /// <summary>
    /// Determines if the namespace of the given type elements is used, or not.
    /// </summary>
    public bool TypeUseNamespace { get; init; }

    /// <summary>
    /// Determines if the host type of the nested type element is used, or not.
    /// </summary>
    public bool TypeUseHost { get; init; }

    /// <summary>
    /// Determines if type elements hide their name, or not. If this setting is enabled, then
    /// it shorcuts other settings related to the type elements.
    /// </summary>
    public bool TypeHideName { get; init; }

    /// <summary>
    /// Determines if nullable annotations are printed, or not.
    /// </summary>
    public bool TypeUseNullability { get; init; }

    /// <summary>
    /// Determines if the nullable wrappers are kept instead of being replaced by '?'.
    /// </summary>
    public bool TypeKeepNullableWrappers { get; init; }

    /// <summary>
    /// Determines if the type generic arguments of the given type element are used, or not.
    /// </summary>
    public bool TypeUseGenericArguments { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the member modifiers are used or not.
    /// </summary>
    public bool MemberUseModifiers { get; init; }

    /// <summary>
    /// If not null, the options to use to print the return type of the member, if any.
    /// If null, then it is ignored.
    /// </summary>
    public EasyNameOptions? MemberReturnTypeOptions { get; init; }

    /// <summary>
    /// If not null, the options to use to print the member host.
    /// If null, then it is ignored.
    /// </summary>
    public EasyNameOptions? MemberHostOptions { get; init; }

    /// <summary>
    /// If not null, the options to use to print the generic arguments of the member.
    /// If null, then they are ignored.
    /// </summary>
    public EasyNameOptions? MemberGenericArgumentOptions { get; init; }

    /// <summary>
    /// Determines if constructors use their tech name instead of the default 'new' one.
    /// </summary>
    public bool UseConstructorTechName { get; init; }

    /// <summary>
    /// Determines if indexers use their tech name instead of the default 'this' one.
    /// </summary>
    public bool UseIndexerTechName { get; init; }

    /// <summary>
    /// Determines if, even if no argument options are provided for the member, at least its
    /// brakets are printed, or not.
    /// </summary>
    public bool MemberUseBrackets { get; init; }

    /// <summary>
    /// If not null, the options to use to print the arguments of the member.
    /// If null, then they are ignored.
    /// </summary>
    public EasyNameOptions? MemberArgumentOptions { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if parameter modifiers (ref, out, in, etc.) are used or not.
    /// </summary>
    public bool ParameterUseModifiers { get; init; }

    /// <summary>
    /// If not null, the options to use to print the type of parameters.
    /// If null, then they are ignored.
    /// </summary>
    public EasyNameOptions? ParameterTypeOptions { get; init; }

    /// <summary>
    /// Determines if the names of the parameters are used or not.
    /// </summary>
    public bool ParameterUseName { get; init; }

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
                TypeUseHost = true;
                TypeUseNullability = true;
                TypeKeepNullableWrappers = true;
                TypeUseGenericArguments = true;
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