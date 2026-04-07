namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to include the nullable annotations in the display string.
/// </summary>
public enum EasyNullableStyle
{
    /// <summary>
    /// Do not use nullable annotations at all.
    /// </summary>
    None,

    /// <summary>
    /// Use nullable annotations, if possible, intercepting nullable wrappers.
    /// </summary>
    UseAnnotations,

    /// <summary>
    /// Use nullable annotations, if possible, but keep nullable wrappers.
    /// </summary>
    KeepWrappers,
}