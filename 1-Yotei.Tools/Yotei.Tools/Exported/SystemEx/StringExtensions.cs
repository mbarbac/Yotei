#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.Generators;
#pragma warning disable CS0436
#else
namespace Yotei.Tools;
#endif

// ========================================================
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
static class StringExtensions
{
    extension(string? source)
    {
        /// <summary>
        /// Returns null if this instance is null or empty, after having been trimmed, if such
        /// was requested. Otherwise returns the trimmed or the original one.
        /// </summary>
        /// <param name="trim"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string? NullWhenEmpty(bool trim)
        {
            if (source != null && trim) source = source.Trim();
            return string.IsNullOrEmpty(source) ? null : source;
        }

        /// <summary>
        /// Guarantees that the returned string is not null and not empty, after having trimmed
        /// this instance if such is requested.
        /// </summary>
        /// <param name="trim"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="EmptyException"></exception>
        public string NotNullNotEmpty(
            bool trim,
            [CallerArgumentExpression(nameof(source))] string? name = null)
        {
            if (string.IsNullOrWhiteSpace(name)) name = nameof(source);

            ArgumentNullException.ThrowIfNull(source, name);

            source = source.NullWhenEmpty(trim);
            return source is null ? throw new EmptyException(name) : source;
        }
    }
}