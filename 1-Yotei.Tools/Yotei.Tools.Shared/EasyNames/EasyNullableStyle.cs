#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.Generators;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// Describes how to include the nullable annotations in the display string.
/// </summary>
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
enum EasyNullableStyle
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