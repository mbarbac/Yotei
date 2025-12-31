namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Provides options for the 'EasyName()' family of methods.
/// </summary>
internal record EasyNameOptions
{
    /// <summary>
    /// A shared read-only instance that represents empty options.
    /// </summary>
    public static EasyNameOptions Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared read-only instance that represents default options.
    /// </summary>
    public static EasyNameOptions Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared read-only instance that represents full options.
    /// </summary>
    public static EasyNameOptions Full { get; } = new(Mode.Full);

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyNameOptions() : this(Mode.Default) { }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the namespace of the type element shall be used.
    /// </summary>
    public bool TypeUseNamespace { get; init; }

    /// <summary>
    /// Determines if the host of the type element, recursively, shall be used.
    /// </summary>
    public bool TypeUseHost { get; init; }

    /// <summary>
    /// Determines if the name of the type element shall be hidden. This setting is mostly used
    /// to prevent generic type names to appear.
    /// </summary>
    public bool TypeHideName { get; init; }

    /// <summary>
    /// Determines if the nullability annotation of the type element shall be used. Note that
    /// for reflection reference types it cannot be found in most circumstances.
    /// </summary>
    public bool TypeUseNullability { get; init; }

    /// <summary>
    /// The options to use with the generic arguments of the type element, or '<c>null</c>' if
    /// they shall not be used.
    /// </summary>
    public EasyNameOptions? TypeArgumentsOptions { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if member prefixes (ie: 'ref', 'static', etc.) shall be used or not.
    /// </summary>
    public bool MemberModifiers { get; init; }

    /// <summary>
    /// The options to use with the return type of the member element, or '<c>null</c>' if it
    /// shall not be used.
    /// </summary>
    public EasyNameOptions? MemberReturnTypeOptions { get; init; }

    /// <summary>
    /// The options to use with the host type of the member element, or '<c>null</c>' if it
    /// shall not be used.
    /// </summary>
    public EasyNameOptions? MemberHostTypeOptions { get; init; }

    /// <summary>
    /// Determines if the technical IL-alike name of the constructor member shall be used. If
    /// not, the C#-alike one shall be used instead.
    /// </summary>
    public bool ConstructorTechName { get; init; }

    /// <summary>
    /// Determines if the technical IL-alike name of the indexer member shall be used. If not,
    /// the C#-alike one shall be used instead.
    /// </summary>
    public bool IndexerTechName { get; init; }

    /// <summary>
    /// The options to use with the generic arguments of the member, or '<c>null</c>' if they
    /// shall not be used.
    /// </summary>
    public EasyNameOptions? MemberGenericArgumentsOptions { get; init; }

    /// <summary>
    /// Determines if the types of the member arguments shall be used.
    /// </summary>
    public bool MemberUseArgumentTypes { get; init; }

    /// <summary>
    /// Determines if the names of the member arguments shall be used.
    /// </summary>
    public bool MemberUseArgumentNames { get; init; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full }
    private EasyNameOptions(Mode mode)
    {
        switch (mode)
        {
            case Mode.Empty: break;

            case Mode.Default:
                TypeUseNullability = true;
                TypeArgumentsOptions = this;
                MemberModifiers = true;
                MemberGenericArgumentsOptions = this;
                MemberUseArgumentTypes = true;
                break;

            case Mode.Full:
                TypeUseNamespace = true;
                TypeUseHost = true;
                TypeHideName = false;
                TypeUseNullability = true;
                TypeArgumentsOptions = this;
                MemberModifiers = true;
                MemberReturnTypeOptions = this;
                MemberHostTypeOptions = this;
                ConstructorTechName = true;
                IndexerTechName = true;
                MemberGenericArgumentsOptions = this;
                MemberUseArgumentTypes = true;
                MemberUseArgumentNames = true;
                break;
        }
    }
}