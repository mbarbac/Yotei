namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Options for the 'EasyName' family of methods.
/// </summary>
internal record EasyNameOptions
{
    /// <summary>
    /// True to obtain the fully-qualified name of the symbol, instead of its short one.
    /// </summary>
    public bool UseFullName { get; init; }

    /// <summary>
    /// True to add to the symbol type its nullable annotation, if any.
    /// </summary>
    public bool AddNullable { get; init; }

    /// <summary>
    /// True to use the generic type arguments of the symbol.
    /// </summary>
    public bool UseGenerics { get; init; }

    /// <summary>
    /// True to preceed the member name with the host type and a dot.
    /// Ignored for types.
    /// </summary>
    public bool UseHostType { get; init; }

    /// <summary>
    /// True to use either the return type of the member, or the member type.
    /// Ignored for types.
    /// </summary>
    public bool UseMemberType { get; init; }

    /// <summary>
    /// True to use either the method arguments or the indexer parameters.
    /// Ignored for any other cases.
    /// </summary>
    public bool UseMemberArguments { get; init; }

    /// <summary>
    /// True to use the argument type, if not requested by <see cref="UseMemberArguments"/>.
    /// Ignored for any other cases.
    /// </summary>
    public bool UseArgumentsType { get; init; }

    /// <summary>
    /// True to use the argument name, if not requested by <see cref="UseMemberArguments"/>.
    /// Ignored for any other cases.
    /// </summary>
    public bool UseArgumentsName { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// An instance that represents a shared set of default options, which by convention are all
    /// set to false.
    /// </summary>
    public static EasyNameOptions Default { get; } = new();

    /// <summary>
    /// Initializes a new instance with all its default arguments set to false.
    /// </summary>
    /// <param name="useFullName"></param>
    /// <param name="addNullable"></param>
    /// <param name="useGenerics"></param>
    /// <param name="useHostType"></param>
    /// <param name="useMemberType"></param>
    /// <param name="useMemberArguments"></param>
    /// <param name="useArgumentsType"></param>
    /// <param name="useArgumentsName"></param>
    public EasyNameOptions(
        bool useFullName = false,
        bool addNullable = false,
        bool useGenerics = false,
        bool useHostType = false,
        bool useMemberType = false,
        bool useMemberArguments = false,
        bool useArgumentsType = false,
        bool useArgumentsName = false)
    {
        UseFullName = useFullName;
        AddNullable = addNullable;
        UseGenerics = useGenerics;
        UseHostType = useHostType;
        UseMemberType = useMemberType;
        UseMemberArguments = useMemberArguments;
        UseArgumentsType = useArgumentsType;
        UseArgumentsName = useArgumentsName;
    }
}