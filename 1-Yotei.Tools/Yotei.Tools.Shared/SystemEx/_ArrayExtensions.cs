#if YOTEI_TOOLS_COREGENERATOR
namespace Yotei.Tools.CoreGenerator;
#else
namespace Yotei.Tools;
#endif

// NOTE: we are not using extension blocks because there some problems when a generic 'T[]'
// array is being extended: 'CS9293: Cannot use an extension parameter in this context'.

// NOTE: the list-alike extension methods will always return a new instance, even if no changes
// were made. The reason is that T[] is itself mutable, and the convention here is that once you
// get the result, the original instance will remain intact.

// ========================================================
#if YOTEI_TOOLS_COREGENERATOR
public
#else
internal
#endif
static class ArrayExtensions
{
}