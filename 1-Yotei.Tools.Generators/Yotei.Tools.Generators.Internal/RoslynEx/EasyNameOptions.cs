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
    /// Whether to obtain the fully qualified name of the type, instead of its short one.
    /// <br/> The default value of this property is: <see langword="false"/>
    /// </summary>
    public bool UseFullTypeName { get; init; } // = false;

    /// <summary>
    /// Whether to include the generic type arguments found in the element, ot not.
    /// <br/> The default value of this property is: <see langword="true"/>
    /// </summary>
    public bool UseTypeParameters { get; init; } = true;

    /// <summary>
    /// Whether to include a question mark for nullable elements, or not.
    /// <br/> The default value of this property is: <see langword="true"/>
    /// </summary>
    public bool UseNullableAnnotation { get; init; } = true;

    /// <summary>
    /// Whether to use the member type (or return type), or not.
    /// <br/> The default value of this property is: <see langword="true"/>
    /// </summary>
    public bool UseMemberType { get; init; } = true;

    /// <summary>
    /// Whether to use the host type for the member easy name, with a dot after, or not.
    /// <br/> The default value of this property is: <see langword="true"/>
    /// </summary>
    public bool UseHostType { get; init; } = true;

    /// <summary>
    /// Whether to use the indexer specification for indexed properties, or method arguments.
    /// <br/> The default value of this property is: <see langword="true"/>
    /// </summary>
    public bool UseArguments { get; init; } = true;
}