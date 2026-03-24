#if NSSHAREDTOOLS_YOTEI_TOOLS
namespace Yotei.Tools;
#elif NSSHAREDTOOLS_YOTEI_COREGENERATOR
namespace Yotei.Tools.CoreGenerator;
#else
namespace Yotei.Unknown;
#endif

// ========================================================
#if !NET20

#if NSSHAREDTOOLS_YOTEI_TOOLS
public
#else
internal
#endif
static class ArgumentNullExceptionExtensions
{
}

#endif