namespace Yotei.Tools;

// ========================================================
public static class EnvironmentExtensions
{
    extension(Environment)
    {
        /// <summary>
        /// Determines if DEBUG mode is active, or not.
        /// </summary>
        public static bool IsDebug =>
#if DEBUG
            true;
#else
            false;
#endif
    }
}