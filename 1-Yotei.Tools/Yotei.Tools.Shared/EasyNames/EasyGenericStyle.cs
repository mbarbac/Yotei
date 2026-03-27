#if YOTEI_TOOLS_COREGENERATOR
namespace Yotei.Tools.CoreGenerator;
#else
namespace Yotei.Tools;
#endif

// ========================================================
#if YOTEI_TOOLS_COREGENERATOR
internal
#else
public
#endif
enum EasyGenericStyle
{
    /// <summary>
    /// Ignore generic arguments.
    /// </summary>
    None,

    /// <summary>
    /// Only include the generic placeholders. This value is mostly only used when there is the
    /// need of producing a generic list with only the placeholders, but not their names.
    /// </summary>
    PlaceHolders,

    /// <summary>
    /// Use the easy names of the generic arguments.
    /// </summary>
    UseEasyNames,
}