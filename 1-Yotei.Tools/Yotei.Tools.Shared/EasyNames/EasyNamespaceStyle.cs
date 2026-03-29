#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.Generators;
#else
namespace Yotei.Tools;
#endif

// ========================================================
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
    /// Use the complete namespace, but not the 'global:' one.
    /// </summary>
    Standard,

    /// <summary>
    /// Use the complete namespace, starting with the 'global:' one.
    /// </summary>
    UseGlobal,
}