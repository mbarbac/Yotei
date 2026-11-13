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
static partial class StringBuilderExtensions
{
    extension(StringBuilder source)
    {
        bool Equals(StringBuilder value, Func<char, char, bool> predicate)
        {
            if (source.Length != value.Length) return false;

            for (int i = 0; i < source.Length; i++)
            {
                var xsource = source[i];
                var xvalue = value[i];
                if (!predicate(xsource, xvalue)) return false;
            }
            return true;
        }

        /// <summary>
        /// Determines if the contents of this instance are the same as the other given one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public bool Equals(
            StringBuilder value, bool ignoreCase)
            => source.Equals(value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Determines if the contents of this instance are the same as the other given one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Equals(
            StringBuilder value, IEqualityComparer<char> comparer)
            => source.Equals(value, (x, y) => comparer.Equals(x, y));

        /// <summary>
        /// Determines if the contents of this instance are the same as the other given one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Equals(
            StringBuilder value, IEqualityComparer<string> comparer)
            => source.Equals(value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Determines if the contents of this instance are the same as the other given one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public bool Equals(
            StringBuilder value, StringComparison comparison)
            => source.Equals(value, (x, y) => x.Equals(y, comparison));
    }
}