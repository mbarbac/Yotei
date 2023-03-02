namespace Yotei.Generators;

// ========================================================
/// <summary>
/// Represents an argument used by the <see cref="NewBuilder"/> class.
/// </summary>
internal class NewArgument
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="argumentName"></param>
    /// <param name="memberName"></param>
    /// <param name="isCloneable"></param>
    public NewArgument(
        ITypeSymbol type,
        string argumentName,
        string? memberName = null,
        bool isCloneable = false)
    {
        Type = type.ThrowIfNull(nameof(type));
        ArgumentName = argumentName.NotNullNotEmpty(nameof(argumentName));
        MemberName = memberName?.NotNullNotEmpty(nameof(memberName));
        IsCloneable = isCloneable;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString()
    {
        var nstr = IsNullable ? "?" : string.Empty;
        var type = $"{Type.Name}{nstr}";
        var name = MemberName == null ? ArgumentName : $"{ArgumentName}({MemberName})";

        return $"({type}) {name}";
    }

    /// <summary>
    /// The type of this argument.
    /// </summary>
    public ITypeSymbol Type { get; }

    /// <summary>
    /// The name of this argument.
    /// </summary>
    public string ArgumentName { get; }

    /// <summary>
    /// The associated member name, or null if this instance is not an init argument.
    /// </summary>
    public string? MemberName { get; }

    /// <summary>
    /// Whether to clone the value referred by this instance, or not.
    /// </summary>
    public bool IsCloneable { get; }

    /// <summary>
    /// Determines if the value referred by this instance is a nullable one, or not.
    /// </summary>
    public bool IsNullable => Type.NullableAnnotation == NullableAnnotation.Annotated;

    // ----------------------------------------------------

    /// <summary>
    /// Returns a string with the code to obtain the value referred by this instance, not ending
    /// with any punctuation symbol.
    /// </summary>
    /// <returns></returns>
    public string Generate()
    {
        if (IsCloneable)
        {
            var nstr = IsNullable ? "?" : string.Empty;
            var type = Type.FullyQualifiedName();
            if (IsNullable && !type.EndsWith("?")) type += "?";

            return $"({type}){ArgumentName}{nstr}.Clone()";
        }
        else return ArgumentName;
    }
}