namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// The options to use in display string with nullable elements.
/// </summary>
internal enum NullableStyle
{
    /// <summary>
    /// Do not use any nullability information.
    /// </summary>
    None,

    /// <summary>
    /// Use nullability annotations and enforced it if the element is decorated with the
    /// <see cref="IsNullableAttribute"/>.
    /// </summary>
    UseAnnotations,

    /// <summary>
    /// Expand the <see cref="Nullable{T}"/> and <see cref="IsNullable{T}"/> wrappers instead
    /// of using the nullability annotations.
    /// </summary>
    KeepWrappers,
}