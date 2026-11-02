namespace Yotei.Tools;

// ========================================================
public static class EnvironmentExtensions
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