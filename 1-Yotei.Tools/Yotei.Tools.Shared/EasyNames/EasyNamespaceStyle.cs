#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.Generators;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// Describes how to obtain the display string of a namespace element.
/// </summary>
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
enum EasyNamespaceStyle
{
    /// <summary>
    /// Do not use namespaces.
    /// </summary>
    None,

    /// <summary>
    /// Use the complete default chain.
    /// </summary>
    Default,

    /// <summary>
    /// Use the complete namespace chain, preceeded by the 'global::' prefix.
    /// </summary>
    UseGlobal,
}