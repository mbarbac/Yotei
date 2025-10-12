namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Provides options to use by the '<c>EasyName(...)</c>' family of methods.
/// </summary>
internal record EasyNameOptions
{
    enum BuildMode { Default, Empty, Full };

    private EasyNameOptions(BuildMode mode)
    {
        switch (mode)
        {
            // All false or null...
            case BuildMode.Empty:
                TypeUseNamespace = false;
                TypeUseHost = false;
                TypeUseName = false;
                TypeGenericArgumentsOptions = null;
                MemberReturnTypeOptions = null;
                MemberHostTypeOptions = null;
                MemberConstructorNew = false;
                MemberIndexerThis = false;
                MemberGenericArgumentsOptions = null;
                MemberArgumentTypesOptions = null;
                MemberArgumentsNames = false;
                break;

            // True: TypeUseName, ConstructorNew, IndexerThis..
            // Null: ReturnType, HostType
            case BuildMode.Default:
                TypeUseNamespace = false;
                TypeUseHost = false;
                TypeUseName = true;
                TypeGenericArgumentsOptions = this;
                MemberReturnTypeOptions = null;
                MemberHostTypeOptions = null;
                MemberConstructorNew = true;
                MemberIndexerThis = true;
                MemberGenericArgumentsOptions = this;
                MemberArgumentTypesOptions = this;
                MemberArgumentsNames = false;
                break;

            // All true or this...
            case BuildMode.Full:
            default:
                TypeUseNamespace = true;
                TypeUseHost = true;
                TypeUseName = true;
                TypeGenericArgumentsOptions = this;
                MemberReturnTypeOptions = this;
                MemberHostTypeOptions = this;
                MemberConstructorNew = true;
                MemberIndexerThis = true;
                MemberGenericArgumentsOptions = this;
                MemberArgumentTypesOptions = this;
                MemberArgumentsNames = true;
                break;
        }
    }

    /// <summary>
    /// An instance with all settings set to false or null.
    /// </summary>
    public static EasyNameOptions Empty => new(BuildMode.Empty);

    /// <summary>
    /// An instance with common useful settings.
    /// </summary>
    public static EasyNameOptions Default => new(BuildMode.Default);

    /// <summary>
    /// An instance with full settings enabled.
    /// </summary>
    public static EasyNameOptions Full => new(BuildMode.Full);

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyNameOptions() : this(BuildMode.Default) { }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the type namespace shall be used.
    /// </summary>
    public bool TypeUseNamespace { get; init; }

    /// <summary>
    /// Determines if the type host shall be used.
    /// </summary>
    public bool TypeUseHost { get; init; }

    /// <summary>
    /// Determines if the type name shall be used.
    /// It is set to '<c>false</c>' when ony its empty placeholder is needed.
    /// </summary>
    public bool TypeUseName { get; init; }

    /// <summary>
    /// If not null, the options to use with the generic arguments of the type.
    /// </summary>
    public EasyNameOptions? TypeGenericArgumentsOptions { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// If not null, the options to use with the return type of the member.
    /// </summary>
    public EasyNameOptions? MemberReturnTypeOptions { get; init; }

    /// <summary>
    /// If not null, the options to use with the host type of the member.
    /// </summary>
    public EasyNameOptions? MemberHostTypeOptions { get; init; }

    /// <summary>
    /// Determines if the name of constructors shall be 'new', or rather '.ctor'.
    /// </summary>
    public bool MemberConstructorNew { get; init; }

    /// <summary>
    /// Determines if the name of indexers shall be "this" instead of the internal indexer name
    /// (it being "Item" by default).
    /// </summary>
    public bool MemberIndexerThis { get; init; }

    /// <summary>
    /// If not null, the options to use with the generic arguments of the member.
    /// </summary>
    public EasyNameOptions? MemberGenericArgumentsOptions { get; init; }

    /// <summary>
    /// If not null, the options to use with the types of the arguments of the member.
    /// </summary>
    public EasyNameOptions? MemberArgumentTypesOptions { get; init; }

    /// <summary>
    /// Determines if the names of the arguments of the member shall be used.
    /// </summary>
    public bool MemberArgumentsNames { get; init; }
}