namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Determines the style to use with the nullable annotations of type elements.
/// </summary>
public enum EasyNullableStyle { None, UseAnnotations, KeepWrappers }

// ========================================================
/// <summary>
/// Provides options for the 'EasyName' family of methods.
/// </summary>
public record EasyNameOptions
{
    /// <summary>
    /// Determines if the variance specifiers ('in', 'out') of the generic type element are
    /// used, or not.
    /// </summary>
    public bool UseTypeVariance { get; init; }

    /// <summary>
    /// Determines if the namespace of the type element is used, or not.
    /// </summary>
    public bool UseTypeNamespace { get; init; }

    /// <summary>
    /// Determines if the host type of the element is used, or not.
    /// <br/> This setting is not used with constructor elements.
    /// </summary>
    public bool UseHost { get; init; }

    /// <summary>
    /// Determines if the name of the type element is hidden, or not.
    /// <br/> If this setting is enabled, it shortcuts all other ones.
    /// </summary>
    public bool HideTypeName { get; init; }

    /// <summary>
    /// Determines the style to use with the nullable annotations of the type elements.
    /// </summary>
    public EasyNullableStyle TypeNullableStyle { get; init; }

    /// <summary>
    /// Determines if the constructo tech name shall be used, or not.
    /// </summary>
    public bool UseConstructorTechName { get; init; }

    /// <summary>
    /// Determines if the indexed property tech name shall be used, or not.
    /// </summary>
    public bool UseIndexedTechName { get; init; }

    /// <summary>
    /// Determines if the generic arguments of the element are used, or not.
    /// </summary>
    public bool UseGenericArguments { get; init; }

    /// <summary>
    /// Determines if the return type of the element is used, or not.
    /// <br/> This setting is only used with regular methods, properties and fields.
    /// </summary>
    public bool UseReturnType { get; init; }

    /// <summary>
    /// Determines if the member element brackets (if any) are used, even if no usage of member
    /// arguments is requested.
    /// </summary>
    public bool UseBrackets { get; init; }

    /// <summary>
    /// Determines if the modifiers ('ref', 'out', etc.) of the member arguments are used, or not.
    /// </summary>
    public bool UseArgumentModifiers { get; init; }

    /// <summary>
    /// Determines if the types of the member arguments are used, or not.
    /// </summary>
    public bool UseArgumentType { get; init; }

    /// <summary>
    /// Determines the nullable style to use with the member argument types.
    /// </summary>
    public EasyNullableStyle ArgumentNullableStyle { get; init; }

    /// <summary>
    /// Determines if the names of the member arguments are used, or not.
    /// </summary>
    public bool UseArgumentName { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// A shared instance with default settings.
    /// </summary>
    public static EasyNameOptions Default { get; } = new EasyNameOptions(Mode.Default);

    /// <summary>
    /// A shared instance with full settings.
    /// </summary>
    public static EasyNameOptions Full { get; } = new EasyNameOptions(Mode.Full);

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EasyNameOptions() : this(Mode.Empty) { }

    /// <summary>
    /// Determines the mode used to initialize this instance.
    /// </summary>
    enum Mode { Empty, Default, Full };
    EasyNameOptions(Mode mode)
    {
        switch (mode)
        {
            case Mode.Empty:
                break;

            default:
            case Mode.Default:
                TypeNullableStyle = EasyNullableStyle.UseAnnotations;
                UseGenericArguments = true;
                UseArgumentModifiers = true;
                UseArgumentType = true;
                ArgumentNullableStyle = EasyNullableStyle.UseAnnotations;
                break;

            case Mode.Full:
                UseTypeVariance = true;
                UseTypeNamespace = true;
                UseHost = true;
                TypeNullableStyle = EasyNullableStyle.KeepWrappers;
                UseConstructorTechName = true;
                UseIndexedTechName = true;
                UseGenericArguments = true;
                UseReturnType = true;
                UseArgumentModifiers = true;
                UseArgumentType = true;
                ArgumentNullableStyle = EasyNullableStyle.KeepWrappers;
                UseArgumentName = true;
                break;
        }
    }
}