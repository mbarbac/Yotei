namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Options for the family of 'EasyName' methods.
/// </summary>
internal record EasyNameOptions
{
    /// <summary>
    /// A set of default options.
    /// </summary>
    public static EasyNameOptions Default { get; } = new();

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="typeFullName"></param>
    /// <param name="typeGenerics"></param>
    /// <param name="typeNullable"></param>
    /// <param name="memberType"></param>
    /// <param name="memberHostType"></param>
    /// <param name="memberArguments"></param>
    public EasyNameOptions(
        bool typeFullName = false,
        bool typeGenerics = true,
        bool typeNullable = true,
        bool typeNullableGenerics = true,
        bool memberType = true,
        bool memberHostType = true,
        bool memberArguments = true)
    {
        TypeFullName = typeFullName;
        TypeGenerics = typeGenerics;
        TypeNullable = typeNullable;
        TypeNullableGenerics = typeNullableGenerics;

        MemberType = memberType;
        MemberHostType = memberHostType;
        MemberArguments = memberArguments;
    }

    /// <summary>
    /// Whether to obtain the fully qualified name of the type, instead of its short one.
    /// </summary>
    public bool TypeFullName { get; init; }

    /// <summary>
    /// Whether to include the generic type arguments found in the element, ot not.
    /// </summary>
    public bool TypeGenerics { get; init; }

    /// <summary>
    /// Whether to include a question mark for nullable types, or not.
    /// </summary>
    public bool TypeNullable { get; init; }

    /// <summary>
    /// Whether to include a question mark for the nullable generic parameters, or not.
    /// </summary>
    public bool TypeNullableGenerics { get; init; }

    /// <summary>
    /// Whether to use the member type (or return type), or not.
    /// </summary>
    public bool MemberType { get; init; }

    /// <summary>
    /// Whether to use the host type for the member easy name, with a dot after, or not.
    /// </summary>
    public bool MemberHostType { get; init; }

    /// <summary>
    /// Whether to use the indexer specification for indexed properties, or method arguments.
    /// </summary>
    public bool MemberArguments { get; init; }
}