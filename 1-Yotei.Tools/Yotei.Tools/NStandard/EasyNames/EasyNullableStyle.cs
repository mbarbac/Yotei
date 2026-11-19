#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Describes how to use nullable annotations, including the <see cref="IsNullable{T}"/> and the
/// <see cref="IsNullableAttribute"/> types, when obtaining easy names.
/// </summary>
#if NETSTANDARD2_0
internal
#else
public
#endif
enum EasyNullableStyle
{
    /// <summary>
    /// Do not use nullable annotations.
    /// </summary>
    None,

    /// <summary>
    /// Annotate the easy name with the nullability information, if any and possible.
    /// </summary>
    Annotate,

    /// <summary>
    /// Keep nullable wrappers or, if nullable, annotate the easy name with the nullability
    /// information, if any and possible.
    /// </summary>
    KeepWrappers,
}