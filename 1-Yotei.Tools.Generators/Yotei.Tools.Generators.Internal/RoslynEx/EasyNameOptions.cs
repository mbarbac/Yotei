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
    /// <param name="fullTypeName"></param>
    /// <param name="typeParameters"></param>
    /// <param name="nullableAnnotation"></param>
    /// <param name="memberType"></param>
    /// <param name="hostType"></param>
    /// <param name="withArguments"></param>
    public EasyNameOptions(
        bool fullTypeName = false,
        bool typeParameters = true,
        bool nullableAnnotation = true,
        bool memberType = true,
        bool hostType = true,
        bool withArguments = true)
    {
        FullTypeName = fullTypeName;
        TypeParameters = typeParameters;
        NullableAnnotation = nullableAnnotation;
        MemberType = memberType;
        HostType = hostType;
        WithArguments = withArguments;
    }

    /// <summary>
    /// Whether to obtain the fully qualified name of the type, instead of its short one.
    /// <br/> The default value of this property is: <see langword="false"/>
    /// </summary>
    public bool FullTypeName { get; init; }

    /// <summary>
    /// Whether to include the generic type arguments found in the element, ot not.
    /// <br/> The default value of this property is: <see langword="true"/>
    /// </summary>
    public bool TypeParameters { get; init; }

    /// <summary>
    /// Whether to include a question mark for nullable elements, or not.
    /// <br/> The default value of this property is: <see langword="true"/>
    /// </summary>
    public bool NullableAnnotation { get; init; }

    /// <summary>
    /// Whether to use the member type (or return type), or not.
    /// <br/> The default value of this property is: <see langword="true"/>
    /// </summary>
    public bool MemberType { get; init; }

    /// <summary>
    /// Whether to use the host type for the member easy name, with a dot after, or not.
    /// <br/> The default value of this property is: <see langword="true"/>
    /// </summary>
    public bool HostType { get; init; }

    /// <summary>
    /// Whether to use the indexer specification for indexed properties, or method arguments.
    /// <br/> The default value of this property is: <see langword="true"/>
    /// </summary>
    public bool WithArguments { get; init; }
}