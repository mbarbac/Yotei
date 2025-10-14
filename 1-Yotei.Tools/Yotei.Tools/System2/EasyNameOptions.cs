namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the 'EasyName(...)' family of methods.
/// </summary>
/// LOW: Find is there is a way to tell if null annotations are used with reference types.
/// It's easy for value types (they are Nullable{T} ones).
public record EasyNameOptions
{
    /// <summary>
    /// A shared instance with all settings set to false or null.
    /// </summary>
    public static EasyNameOptions Empty => new(BuildMode.Empty);

    /// <summary>
    /// A shared instance with default useful settings.
    /// </summary>
    public static EasyNameOptions Default => new(BuildMode.Default);

    /// <summary>
    /// A shared instance with full settings enabled.
    /// </summary>
    public static EasyNameOptions Full => new(BuildMode.Full);

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyNameOptions() : this(BuildMode.Default) { }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the namespace of the type element shall be used.
    /// <br/> Setting this property implies <see cref="TypeUseHost"/>.
    /// </summary>
    public bool TypeUseNamespace { get; init; }

    /// <summary>
    /// Determines if the host of the type element shall be used.
    /// </summary>
    public bool TypeUseHost { get; init; }

    /// <summary>
    /// Determines if the name of the type element shall be used, or rather just its placeholder.
    /// <br/> This setting is mostly used to prevent generic names to appear.
    /// </summary>
    public bool TypeUseName { get; init; }

    /// <summary>
    /// If not null, the options to use with the generic type arguments of the type element.
    /// <br/> If null, then the generic type arguments are not used.
    /// </summary>
    public EasyNameOptions? TypeGenericArgumentOptions { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// If not null, the options to use with the return type of the member.
    /// <br/> If null, then the member return type is not used.
    /// </summary>
    public EasyNameOptions? MemberReturnTypeOptions { get; init; }

    /// <summary>
    /// If not null, the options to use with the host type of the member.
    /// <br/> If null, then the member host type is not used.
    /// </summary>
    public EasyNameOptions? MemberHostTypeOptions { get; init; }

    /// <summary>
    /// The literal to use as the name of constructor members.
    /// <br/> The default value of this property is 'new'.
    /// </summary>
    public string ConstructorName { get; init => field = value.NotNullNotEmpty(true); }

    /// <summary>
    /// The literal to use as the name of indexed members.
    /// <br/> The default value of this property is 'this'.
    /// <br/> If the value is '$', then the name of the internal property is found and used.
    /// </summary>
    public string IndexerName { get; init => field = value.NotNullNotEmpty(true); }

    /// <summary>
    /// If not null, the options to use with the generic type arguments of the member element.
    /// <br/> If null, then the generic type arguments are not used.
    /// </summary>
    public EasyNameOptions? MemberGenericArgumentOptions { get; init; }

    /// <summary>
    /// If not null, the options to use with the arguments of the member element.
    /// <br/> If null, and <see cref="MemberUseArgumentNames"/> is not set, then member arguments
    /// are not used.
    /// </summary>
    public EasyNameOptions? MemberArgumentTypeOptions { get; init; }

    /// <summary>
    /// Determines if the names of the member arguments shall be used.
    /// <br/> If null, and <see cref="MemberArgumentTypeOptions"/> is null, then member arguments
    /// are not used.
    /// </summary>
    public bool MemberUseArgumentNames { get; init; }

    // ----------------------------------------------------

    enum BuildMode { Empty, Default, Full };

    private EasyNameOptions(BuildMode mode)
    {
        ConstructorName = "new";
        IndexerName = "this";

        switch (mode)
        {
            case BuildMode.Empty:
                TypeUseNamespace = false;
                TypeUseHost = false;
                TypeUseName = false;
                TypeGenericArgumentOptions = null;
                MemberReturnTypeOptions = null;
                MemberHostTypeOptions = null;
                MemberGenericArgumentOptions = null;
                MemberArgumentTypeOptions = null;
                MemberUseArgumentNames = false;
                break;

            case BuildMode.Default:
                TypeUseNamespace = false;
                TypeUseHost = false;
                TypeUseName = true; // Use the type element name
                TypeGenericArgumentOptions = this; // Use type generic arguments
                MemberReturnTypeOptions = null;
                MemberHostTypeOptions = null;
                MemberGenericArgumentOptions = this; // Use member generic arguments
                MemberArgumentTypeOptions = this; // Use the types of the member arguments
                MemberUseArgumentNames = false;
                break;

            case BuildMode.Full:
            default:
                TypeUseNamespace = true;
                TypeUseHost = true;
                TypeUseName = true;
                TypeGenericArgumentOptions = this;
                MemberReturnTypeOptions = this;
                MemberHostTypeOptions = this;
                MemberGenericArgumentOptions = this;
                MemberArgumentTypeOptions = this;
                MemberUseArgumentNames = true;
                break;
        }
    }
}