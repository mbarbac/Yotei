#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.CoreGenerator;
#else
namespace Yotei.Tools;
#endif

// ========================================================
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
static class StringComparerExtensions
{
    extension(StringComparer source)
    {
        /// <summary>
        /// Returns the <see cref="StringComparison"/> value that corresponds to this instance,
        /// provided it is one of the standard ones. If not, <see langword="null"/> is then
        /// returned.
        /// </summary>
        /// <returns></returns>
        public StringComparison? ToComparison() => source switch
        {
            var c when c == StringComparer.CurrentCulture => StringComparison.CurrentCulture,
            var c when c == StringComparer.CurrentCultureIgnoreCase => StringComparison.CurrentCultureIgnoreCase,
            var c when c == StringComparer.InvariantCulture => StringComparison.InvariantCulture,
            var c when c == StringComparer.InvariantCultureIgnoreCase => StringComparison.InvariantCultureIgnoreCase,
            var c when c == StringComparer.Ordinal => StringComparison.Ordinal,
            var c when c == StringComparer.OrdinalIgnoreCase => StringComparison.OrdinalIgnoreCase,
            _ => null
        };
    }
}