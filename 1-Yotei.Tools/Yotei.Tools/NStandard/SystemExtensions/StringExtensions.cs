#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

using System.ComponentModel.DataAnnotations;

namespace Yotei.Tools;

// ========================================================
#if NETSTANDARD2_0
internal
#else
public
#endif
static class StringExtensions
{
    extension(string source)
    {
        /// <summary>
        /// Returns a new string with all the diacritic marks removed.
        /// </summary>
        /// <returns></returns>
        public string RemoveDiacritics()
        {
            source = source.Normalize(NormalizationForm.FormD);

            var sb = new StringBuilder(); foreach (var c in source)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark) sb.Append(c);
            }
            return sb.ToString();
        }
    }

    extension(string? source)
    {
        // ------------------------------------------------

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
            source.ThrowWhenNull(name);

            source = source.NullWhenEmpty(trim);
            return source is null ? throw new EmptyException(name!) : source;
        }

        // ------------------------------------------------

        /// <summary>
        /// Wraps this string, provided it is not null and not empty (after trimming it if such
        /// is requested), with the given head and tail characters, using the given comparer.
        /// </summary>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <param name="trim"></param>
        /// <returns></returns>
        public string? Wrap(char head, char tail, bool trim, IEqualityComparer<char> comparer)
        {
            comparer.ThrowWhenNull();

            if (source is not null)
            {
                var span = source.AsSpan();
                if (trim) span = span.Trim();

                if (span.Length > 0 &&
                    !comparer.Equals(head, source[0]) &&
                    !comparer.Equals(tail, source[^1]))
                {
                    var temp = span.ToString();
                    return $"{head}{temp}{tail}";
                }

                if (span.Length != source.Length) return span.ToString();
            }
            return source;
        }

        /// <summary>
        /// Wraps this string, provided it is not null and not empty (after trimming it if such
        /// is requested), with the given character.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="trim"></param>
        /// <returns></returns>
        public string? Wrap(
            char c, bool trim, IEqualityComparer<char> comparer)
            => source.Wrap(c, c, trim, comparer);

        /// <summary>
        /// Wraps this string, provided it is not null and not empty (after trimming it if such
        /// is requested), with the given head and tail characters.
        /// </summary>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <param name="trim"></param>
        /// <returns></returns>
        public string? Wrap(
            char head, char tail, bool trim) => source.Wrap(head, tail, trim, char.CharComparer());

        /// <summary>
        /// Wraps this string, provided it is not null and not empty (after trimming it if such
        /// is requested), with the given character.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="trim"></param>
        /// <returns></returns>
        public string? Wrap(char c, bool trim) => source.Wrap(c, c, trim);

        // ------------------------------------------------

        /// <summary>
        /// Removes from this string, after trimming it if such is requested, the given head and
        /// tail characters, and provided they appear paired. Recursion is requested by default.
        /// </summary>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <param name="trim"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public string? UnWrap(
            char head, char tail, bool trim, bool recursive = true)
            => source.UnWrap(head, tail, trim, char.CharComparer(), recursive);

        /// <summary>
        /// Removes from this string, after trimming it if such is requested, the given head and
        /// tail characters, using the given comparer, and provided they appear paired. Recursion
        /// is requested by default.
        /// </summary>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <param name="trim"></param>
        /// <param name="comparer"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public string? UnWrap(
            char head, char tail, bool trim, IEqualityComparer<char> comparer,
            bool recursive = true)
        {
            comparer.ThrowWhenNull();

            if (source is not null)
            {
                var span = source.AsSpan(); while (true)
                {
                    if (trim) span = span.Trim();
                    if (span.Length < 2) break;
                    if (!comparer.Equals(head, span[0]) ||
                        !comparer.Equals(tail, span[^1]))
                        break;

                    span = span[1..^1];
                    if (!recursive) break;
                }

                if (span.Length != source.Length) return span.ToString();
            }
            return source;
        }

        /// <summary>
        /// Removes from this string, after trimming it if such is requested, the given character,
        /// provided it appears paired at its head and tail. Recursion is requested by default.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="trim"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public string? UnWrap(
            char c, bool trim, bool recursive = true)
            => source.UnWrap(c, c, trim, Char.CharComparer(), recursive);

        /// <summary>
        /// Removes from this string, after trimming it if such is requested, the given character
        /// using the given comparer, provided it appears paired at its head and tail. Recursion
        /// is requested by default.
        /// </summary>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <param name="trim"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public string? UnWrap(
            char c, bool trim, IEqualityComparer<char> comparer, bool recursive = true)
            => source.UnWrap(c, c, trim, comparer, recursive);
    }
}