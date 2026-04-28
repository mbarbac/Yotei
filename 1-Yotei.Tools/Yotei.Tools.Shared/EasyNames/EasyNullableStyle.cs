namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to include the nullable annotations in the C#-alike string representations.
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
    /// Keep nullable wrappers, if possible, and in any case, use nullable annotations.
    /// </summary>
    KeepWrappers,
}