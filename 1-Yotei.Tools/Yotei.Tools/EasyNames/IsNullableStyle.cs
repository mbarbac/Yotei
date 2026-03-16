namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Determines the style to use with nullable annotations.
/// </summary>
public enum IsNullableStyle
{
    /// <summary>
    /// Do not use nullable annotations at all.
    /// </summary>
    None,

    /// <summary>
    /// Use nullable annotations, intercepting nullable wrappers.
    /// </summary>
    UseAnnotations,

    /// <summary>
    /// Use nullable annotations if possible, but keep nullable wrappers.
    /// </summary>
    KeepWrappers,
}