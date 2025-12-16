namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the 'EasyName(...)' family of methods.
/// <br/> Instances of this type are immutable ones.
/// </summary>
public record EasyNameOptions
{
    /// <summary>
    /// A shared read-only instance that represents empty options.
    /// <br/> All settings are set to 'false' or 'null'.
    /// </summary>
    public static EasyNameOptions Empty => new(BuildMode.Empty);

    /// <summary>
    /// A shared read-only instance that represents default options.
    /// </summary>
    public static EasyNameOptions Default => new(BuildMode.Default);

    /// <summary>
    /// A shared read-only instance that represents full options.
    /// <br/> All seetings are set to 'true' or to a full-instance's reference.
    /// </summary>
    public static EasyNameOptions Full => new(BuildMode.Full);

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyNameOptions() : this(BuildMode.Default) { }

    enum BuildMode { Empty, Default, Full }
    private EasyNameOptions(BuildMode mode)
    {
        switch (mode)
        {
            case BuildMode.Empty:
                break;

            case BuildMode.Default:
                TypeGenericArgumentOptions = this;
                MemberGenericArgumentOptions = this;
                ParameterTypeOptions = this;
                break;

            case BuildMode.Full:
                TypeUseNamespace = true;
                TypeUseHost = true;
                TypeGenericArgumentOptions = this;
                MemberReturnTypeOptions = this;
                MemberHostTypeOptions = this;
                MemberGenericArgumentOptions = this;
                ConstructorTechName = true;
                IndexerTechName = true;
                MemberUseParameters = true;
                ParameterTypeOptions = this;
                ParameterUseName = true;
                break;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the namespace of the type element shall be used.
    /// </summary>
    public bool TypeUseNamespace { get; init; }

    /// <summary>
    /// Determines if the host of the type element shall be used.
    /// </summary>
    public bool TypeUseHost { get; init; }

    /// <summary>
    /// Determines if the type name shall be hidden or not.
    /// <br/> This setting is mostly used to prevent generic names to appear.
    /// </summary>
    public bool TypeHideName { get; init; }

    /// <summary>
    /// If not null, the options to use with the type's generic arguments.
    /// <br/> If 'null', then generic type arguments are ignored.
    /// </summary>
    public EasyNameOptions? TypeGenericArgumentOptions { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// If not null, the options to use with the return type of the member element.
    /// <br/> If 'null', then return types are ignored.
    /// </summary>
    public EasyNameOptions? MemberReturnTypeOptions { get; init; }

    /// <summary>
    /// If not null, the options to use with the host type of the member element.
    /// <br/> If 'null', then members' host types are ignored.
    /// </summary>
    public EasyNameOptions? MemberHostTypeOptions { get; init; }

    /// <summary>
    /// If not null, the options to use with the generic arguments of members and methods.
    /// </summary>
    public EasyNameOptions? MemberGenericArgumentOptions { get; init; }

    /// <summary>
    /// Determines if the technical name of constructor elements shall be used, instead of the
    /// default 'new' one.
    /// </summary>
    public bool ConstructorTechName { get; init; }

    /// <summary>
    /// Determines if the technical name of constructor elements shall be used, instead of the
    /// default 'this' one.
    /// </summary>
    public bool IndexerTechName { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the parameters of the member shall be used, or not.
    /// </summary>
    public bool MemberUseParameters { get; init; }

    /// <summary>
    /// If not null, the options to use with the types of parameter elements.
    /// <br/> If 'null', then parameter types are ignored.
    /// </summary>
    public EasyNameOptions? ParameterTypeOptions { get; init; }

    /// <summary>
    /// Determines if the name of the parameter element shall be used.
    /// </summary>
    public bool ParameterUseName { get; init; }
}