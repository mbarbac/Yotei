using StringSpan = System.ReadOnlySpan<char>;

namespace Yotei.Tools;

// ========================================================
public static partial class StringBuilderExtensions
{
    extension(StringBuilder source)
    {
        bool Equals(StringBuilder other, Func<char, char, bool> predicate)
        {
            if (source.Length != other.Length) return false;

            for (int i = 0; i < source.Length; i++)
            {
                var xsource = source[i];
                var xother = other[i];
                if (!predicate(xsource, xother)) return false;
            }

            return true;
        }

        /// <summary>
        /// Determines if this instance is equal to the other given one.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public bool Equals(
            StringBuilder other, bool ignoreCase)
            => Equals(source, other, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Determines if this instance is equal to the other given one.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Equals(
            StringBuilder other, IEqualityComparer<char> comparer)
            => Equals(source, other, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Determines if this instance is equal to the other given one.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Equals(
            StringBuilder other, IEqualityComparer<string> comparer)
            => Equals(source, other, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Determines if this instance is equal to the other given one.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public bool Equals(
            StringBuilder other, StringComparer comparison)
            => Equals(source, other, (x, y) => x.Equals(y, comparison));

        // ------------------------------------------------

        bool Equals(StringSpan other, Func<char, char, bool> predicate)
        {
            if (source.Length != other.Length) return false;

            for (int i = 0; i < source.Length; i++)
            {
                var xsource = source[i];
                var xother = other[i];
                if (!predicate(xsource, xother)) return false;
            }

            return true;
        }

        /// <summary>
        /// Determines if this instance is equal to the other given one.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public bool Equals(
            StringSpan other, bool ignoreCase)
            => Equals(source, other, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Determines if this instance is equal to the other given one.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Equals(
            StringSpan other, IEqualityComparer<char> comparer)
            => Equals(source, other, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Determines if this instance is equal to the other given one.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Equals(
            StringSpan other, IEqualityComparer<string> comparer)
            => Equals(source, other, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Determines if this instance is equal to the other given one.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public bool Equals(
            StringSpan other, StringComparer comparison)
            => Equals(source, other, (x, y) => x.Equals(y, comparison));
    }
}