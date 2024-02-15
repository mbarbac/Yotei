namespace Yotei.Tools.InheritGenerator;

// ========================================================
/// <summary>
/// Describes a type to inherit from.
/// </summary>
internal class TypeElement(ITypeSymbol type, ImmutableArray<TypeArgument> arguments, bool changeProperties)
{
    /// <summary>
    /// Returns the short C#-alike name of the type to inherit from.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Reduced;

    string ToString(bool expanded)
    {
        var sb = new StringBuilder();

        sb.Append(Type.EasyName(new EasyNameOptions(
            typeFullName: expanded,
            typeGenerics: false,
            typeNullable: false,
            typeNullableGenerics: true)));

        if (TypeArguments.Length > 0)
        {
            sb.Append('<'); for (int i = 0; i < TypeArguments.Length; i++)
            {
                if (i != 0) sb.Append(", ");
                sb.Append(expanded
                    ? TypeArguments[i].Expanded
                    : TypeArguments[i].Reduced);
            }
            sb.Append('>');
        }

        return sb.ToString();
    }

    /// <summary>
    /// The expanded string representation of this instance.
    /// </summary>
    public string Expanded => _Expanded ??= ToString(expanded: true);
    string? _Expanded;

    /// <summary>
    /// The reduced string representation of this instance.
    /// </summary>
    public string Reduced => _Reduced ??= ToString(expanded: false);
    string? _Reduced;

    /// <summary>
    /// Detremines if this instance is the same as the given symbol.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public bool SameAs(ITypeSymbol symbol)
    {
        symbol.ThrowWhenNull();

        var name = symbol.EasyName(Options);
        return name == Expanded;
    }

    static readonly EasyNameOptions Options = new(
        typeFullName: true,
        typeGenerics: true,
        typeNullable: false,
        typeNullableGenerics: true);

    // ----------------------------------------------------

    /// <summary>
    /// The type to inherit from.
    /// </summary>
    public ITypeSymbol Type { get; } = type.ThrowWhenNull();

    /// <summary>
    /// The type arguments carried by the provider type, or an empty one if any.
    /// </summary>
    public ImmutableArray<TypeArgument> TypeArguments { get; } = arguments.ThrowWhenNull();

    /// <summary>
    /// Whether or not to upcast the properties of the type being inherited.
    /// </summary>
    public bool ChangeProperties { get; } = changeProperties;
}