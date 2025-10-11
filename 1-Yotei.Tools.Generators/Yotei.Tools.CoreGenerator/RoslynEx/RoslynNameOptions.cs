namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Provides options to use by the '<c>RoslynName(...)</c>' family of methods.
/// </summary>
internal record RoslynNameOptions
{
    enum BuildMode { Default, Empty, Full };

    private RoslynNameOptions(BuildMode mode)
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
                MemberGenericArgumentsOptions = null;
                MemberUseArguments = false;
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
                MemberGenericArgumentsOptions = this;
                MemberUseArguments = false;
                MemberArgumentTypesOptions = this;
                MemberArgumentsNames = false;
                break;

            // All true or this...
            default:
                TypeUseNamespace = true;
                TypeUseHost = true;
                TypeUseName = true;
                TypeGenericArgumentsOptions = this;
                MemberReturnTypeOptions = this;
                MemberHostTypeOptions = this;
                MemberGenericArgumentsOptions = this;
                MemberUseArguments = false;
                MemberArgumentTypesOptions = this;
                MemberArgumentsNames = true;
                break;
        }
    }

    /// <summary>
    /// An instance with all settings set to false or null.
    /// </summary>
    public static RoslynNameOptions Empty => new(BuildMode.Empty);

    /// <summary>
    /// An instance with common useful settings.
    /// </summary>
    public static RoslynNameOptions Default => new(BuildMode.Default);

    /// <summary>
    /// An instance with full settings enabled.
    /// </summary>
    public static RoslynNameOptions Full => new(BuildMode.Full);

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public RoslynNameOptions() : this(BuildMode.Default) { }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance whose options are based on the ones in this instance.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.NullReferenceException"></exception>
    public EasyNameOptions ToEasyNameOptions() => new()
    {
        TypeUseNamespace = this.TypeUseNamespace,
        TypeUseHost = this.TypeUseHost,
        TypeUseName = this.TypeUseName,
        TypeGenericArgumentsOptions = this.TypeGenericArgumentsOptions?.ToEasyNameOptions(),
        MemberReturnTypeOptions = this.MemberReturnTypeOptions?.ToEasyNameOptions(),
        MemberHostTypeOptions = this.MemberHostTypeOptions?.ToEasyNameOptions(),
        MemberConstructorNew = true,
        MemberIndexerThis = true,
        MemberGenericArgumentsOptions = this.MemberGenericArgumentsOptions?.ToEasyNameOptions(),
        MemberArgumentTypesOptions =
            this.MemberArgumentTypesOptions?.ToEasyNameOptions() ?? (
            this.MemberUseArguments ? EasyNameOptions.Default : null),
        MemberArgumentsNames = this.MemberArgumentsNames,
    };

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
    /// Indicates whether the nullable annotation '?' shall be added to the names of nullable
    /// types, or not.
    /// </summary>
    public bool TypeUseNullable { get; init; }

    /// <summary>
    /// If not null, the options to use with the generic arguments of the type.
    /// </summary>
    public RoslynNameOptions? TypeGenericArgumentsOptions { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// If not null, the options to use with the return type of the member.
    /// </summary>
    public RoslynNameOptions? MemberReturnTypeOptions { get; init; }

    /// <summary>
    /// If not null, the options to use with the host type of the member.
    /// </summary>
    public RoslynNameOptions? MemberHostTypeOptions { get; init; }

    /// <summary>
    /// If not null, the options to use with the generic arguments of the member.
    /// </summary>
    public RoslynNameOptions? MemberGenericArgumentsOptions { get; init; }

    /// <summary>
    /// Detemines if the member arguments shall be used, if any, or not.
    /// </summary>
    public bool MemberUseArguments { get; init; }

    /// <summary>
    /// If not null, the options to use with the types of the arguments of the member.
    /// </summary>
    public RoslynNameOptions? MemberArgumentTypesOptions { get; init; }

    /// <summary>
    /// Determines if the names of the arguments of the member shall be used.
    /// </summary>
    public bool MemberArgumentsNames { get; init; }
}