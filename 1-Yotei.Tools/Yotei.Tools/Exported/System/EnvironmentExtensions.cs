#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.Generators;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// Represents the ocurrence of a duplicated element when it was not supposed to happen.
/// </summary>
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
static class EnvironmentExtensions
{
    extension(Environment)
    {
        /// <summary>
        /// Determines if compilation has been performed in debug mode.
        /// </summary>
        public static bool IsDebug =>
#if DEBUG
            true;
#else
            false;
#endif
    }
}