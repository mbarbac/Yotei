#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

namespace Yotei.Tools;

// ========================================================
#if NETSTANDARD2_0
internal
#else
public
#endif
static class StringComparerExtensions
{
    extension(StringComparer source)
    {
        /// <summary>
        /// Tries to obtain a comparison value based upon this comparer instance.
        /// </summary>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public bool TryGetComparison([NotNullWhen(true)] out StringComparison? comparison)
        {
            comparison = source switch
            {
                var c when c == StringComparer.CurrentCulture => StringComparison.CurrentCulture,
                var c when c == StringComparer.CurrentCultureIgnoreCase => StringComparison.CurrentCultureIgnoreCase,
                var c when c == StringComparer.InvariantCulture => StringComparison.InvariantCulture,
                var c when c == StringComparer.InvariantCultureIgnoreCase => StringComparison.InvariantCultureIgnoreCase,
                var c when c == StringComparer.Ordinal => StringComparison.Ordinal,
                var c when c == StringComparer.OrdinalIgnoreCase => StringComparison.OrdinalIgnoreCase,
                _ => null
            };

            return comparison is not null;
        }
    }
}