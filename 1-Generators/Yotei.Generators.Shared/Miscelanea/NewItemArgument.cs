namespace Yotei.Generators;

// ========================================================
/// <summary>
/// Represents an optional argument of the <see cref="NewItemBuilder.Print"/> method.
/// </summary>
internal class NewItemArgument
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="sourceName"></param>
    /// <param name="matchName"></param>
    /// <param name="type"></param>
    /// <param name="clone"></param>
    /// <param name="nullable"></param>
    public NewItemArgument(
        string sourceName, string matchName, ITypeSymbol type,
        bool clone = false,
        bool nullable = false)
    {
        SourceName = sourceName.NotNullNotEmpty(nameof(sourceName));
        MatchName = matchName.NotNullNotEmpty(nameof(matchName));
        Type = type.ThrowIfNull(nameof(type));
        IsCloneable = clone;
        IsNullable = nullable;
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="clone"></param>
    /// <param name="nullable"></param>
    public NewItemArgument(
        IPropertySymbol symbol,
        bool? clone = null,
        bool? nullable = null)
    {
        symbol = symbol.ThrowIfNull(nameof(symbol));

        SourceName = symbol.Name;
        MatchName = symbol.Name;
        Type = symbol.Type;
        IsCloneable = clone ?? NewItemInfo.IsCloneable(Type);
        IsNullable = nullable ?? Type.NullableAnnotation == NullableAnnotation.Annotated;
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="clone"></param>
    /// <param name="nullable"></param>
    public NewItemArgument(
        IFieldSymbol symbol,
        bool? clone = null,
        bool? nullable = null)
    {
        symbol = symbol.ThrowIfNull(nameof(symbol));

        SourceName = symbol.Name;
        MatchName = symbol.Name;
        Type = symbol.Type;
        IsCloneable = clone ?? NewItemInfo.IsCloneable(Type);
        IsNullable = nullable ?? Type.NullableAnnotation == NullableAnnotation.Annotated;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => MatchName == SourceName
        ? MatchName
        : $"{SourceName}({MatchName})";

    /// <summary>
    /// The name from where to obtain the value of this argument.
    /// </summary>
    public string SourceName { get; }

    /// <summary>
    /// The name to match against constructor parameters, and type properties and fields.
    /// </summary>
    public string MatchName { get; }

    /// <summary>
    /// The type of this argument.
    /// </summary>
    public ITypeSymbol Type { get; }

    /// <summary>
    /// Whether to obtain a clone of the value of this argument, or not.
    /// </summary>
    public bool IsCloneable { get; }

    /// <summary>
    /// Whether to consider the value of this argument as a nullable one, or not.
    /// </summary>
    public bool IsNullable { get; }

    /// <summary>
    /// Returns the code to obtain the value of this argument. The returned string does not
    /// end with a semicolon.
    /// </summary>
    /// <returns></returns>
    public string Generate()
    {
        if (IsCloneable)
        {
            var nstr = IsNullable ? "?" : string.Empty;
            var type = Type.FullyQualifiedName();
            if (IsNullable && !type.EndsWith("?")) type += "?";

            return $"({type}){SourceName}{nstr}.Clone()";
        }
        else return SourceName;
    }
}