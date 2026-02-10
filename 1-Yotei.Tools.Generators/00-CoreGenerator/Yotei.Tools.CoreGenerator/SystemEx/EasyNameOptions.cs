namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Provides options for the 'EasyName' methods.
/// </summary>
internal record EasyNameOptions
{
    /// <summary>
    /// Determines if the type variance modifiers shall be used or not.
    /// </summary>
    public bool UseTypeVariance { get; init; }

    /// <summary>
    /// Determines if the type namespace shall be used or not.
    /// </summary>
    public bool UseTypeNamespace { get; init; }

    /// <summary>
    /// Determines if the type host shall be used or not.
    /// </summary>
    public bool UseTypeHost { get; init; }

    /// <summary>
    /// Determines if the type name shall be hidden, or not.
    /// If this setting is enabled, then it shortcuts all other type-related ones.
    /// </summary>
    public bool HideTypeName { get; init; }

    /// <summary>
    /// Determines the nullable style to use with type elements.
    /// </summary>
    public IsNullableStyle TypeNullableStyle { get; init; }

    /// <summary>
    /// If not null, then the options to use with the generic type arguments of the type.
    /// If null, then they are ignored.
    /// </summary>
    public EasyNameOptions? TypeGenericArgumentOptions { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the member modifiers ('ref', 'in', 'out', etc.) shall be used or not.
    /// </summary>
    public bool UseMemberModifiers { get; init; }

    /// <summary>
    /// If not null, the options to use with the return type of the member.
    /// If null, then it is ignored.
    /// </summary>
    public EasyNameOptions? MemberReturnTypeOptions { get; init; }

    /// <summary>
    /// If not null, the options to use with the host type of the member.
    /// If null, then it is ignored.
    /// </summary>
    public EasyNameOptions? MemberHostTypeOptions { get; init; }

    /// <summary>
    /// Determines if the tech name of the constructor shall be added to the host name.
    /// </summary>
    public bool ConstructorTechName { get; init; }

    /// <summary>
    /// Determines if the tech name of the indexed property shall be used instead of the default
    /// 'this' one.
    /// </summary>
    public bool IndexedTechName { get; init; }

    /// <summary>
    /// If not null, then the options to use with the generic type arguments of the member.
    /// If null, then they are ignored.
    /// </summary>
    public EasyNameOptions? MemberGenericArgumentOptions { get; init; }

    /// <summary>
    /// Determines if the member brackets are used or not, even if no member arguments have been
    /// requested.
    /// </summary>
    public bool UseMemberBrackets { get; init; }

    /// <summary>
    /// If not null, then the options to use with the member arguments.
    /// If null, then they are ignored.
    /// </summary>
    public EasyNameOptions? MemberArgumentOptions { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the argument modifiers ('ref', 'out', etc.) shall be used or not.
    /// </summary>
    public bool UseArgumentModifiers { get; init; }

    /// <summary>
    /// If not null, then the options to use with the type of the argument.
    /// If null, then it is ignored.
    /// </summary>
    public EasyNameOptions? ArgumentTypeOptions { get; init; }

    /// <summary>
    /// Determines if the name of the argument shall be used or not.
    /// </summary>
    public bool UseArgumentName { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains a shared empty instance.
    /// </summary>
    public static EasyNameOptions Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// Obtains a shared default instance.
    /// </summary>
    public static EasyNameOptions Default { get; } = new(Mode.Default);

    /// <summary>
    /// Obtains a shared full instance.
    /// </summary>
    public static EasyNameOptions Full { get; } = new(Mode.Full);

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyNameOptions() : this(Mode.Empty) { }
    enum Mode { Empty, Default, Full };
    EasyNameOptions(Mode mode)
    {
        HideTypeName = false;

        switch (mode)
        {
            case Mode.Full:
                UseTypeVariance = true;
                UseTypeNamespace = true;
                UseTypeHost = true;
                TypeNullableStyle = IsNullableStyle.KeepWrappers;
                TypeGenericArgumentOptions = this;
                UseMemberModifiers = true;
                MemberReturnTypeOptions = this;
                MemberHostTypeOptions = this;
                ConstructorTechName = true;
                IndexedTechName = true;
                MemberGenericArgumentOptions = this;
                UseMemberBrackets = true;
                MemberArgumentOptions = this;
                UseArgumentModifiers = true;
                ArgumentTypeOptions = this;
                UseArgumentName = true;
                break;

            default:
            case Mode.Default:
                TypeNullableStyle = IsNullableStyle.UseAnnotations;
                TypeGenericArgumentOptions = this;
                MemberGenericArgumentOptions = this;
                MemberArgumentOptions = this;
                UseArgumentModifiers = true;
                ArgumentTypeOptions = this;
                break;

            case Mode.Empty:
                TypeNullableStyle = IsNullableStyle.None;
                break;
        }
    }
}