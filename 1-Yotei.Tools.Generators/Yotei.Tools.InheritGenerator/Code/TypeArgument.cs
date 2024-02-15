namespace Yotei.Tools.InheritGenerator;

// ========================================================
/// <summary>
/// Describes a type argument in the type to inherit from.
/// </summary>
/// <param name="type"></param>
/// <param name="isGeneric"></param>
/// <param name="genericName"></param>
internal class TypeArgument(ITypeSymbol type, bool isGeneric, string genericName)
{
    /// <summary>
    /// Returns the short C#-alike name of the type to inherit from.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => ToString(expanded: false);

    string ToString(bool expanded)
    {
        return IsGeneric
            ? genericName
            : Type.EasyName(new EasyNameOptions(
                typeFullName: expanded,
                typeGenerics: true,
                typeNullable: false,
                typeNullableGenerics: true));
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

    // ----------------------------------------------------

    /// <summary>
    /// The actual type this argument refers to.
    /// </summary>
    public ITypeSymbol Type { get; } = type.ThrowWhenNull();

    /// <summary>
    /// Whether it is a generic argument type, or not.
    /// </summary>
    public bool IsGeneric { get; } = isGeneric;

    /// <summary>
    /// The generic name to use with this argument type, or a empty one if any.
    /// </summary>
    public string GenericName { get; } = genericName == string.Empty ? string.Empty : genericName.ThrowWhenNull();
}