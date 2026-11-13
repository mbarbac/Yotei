using StringSpan = System.ReadOnlySpan<char>;

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
        bool Equals(StringSpan value, Func<char, char, bool> predicate)
        {
            if (source.Length != value.Length) return false;

            for (int i = 0; i < source.Length; i++)
            {
                var xsource = source[i];
                var xtarget = value[i];
                if (!predicate(xsource, xtarget)) return false;
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
            StringSpan value, bool ignoreCase)
            => source.Equals(value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Determines if the contents of this instance are the same as the other given one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Equals(
            StringSpan value, IEqualityComparer<char> comparer)
            => source.Equals(value, (x, y) => comparer.Equals(x, y));

        /// <summary>
        /// Determines if the contents of this instance are the same as the other given one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Equals(
            StringSpan value, IEqualityComparer<string> comparer)
            => source.Equals(value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Determines if the contents of this instance are the same as the other given one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public bool Equals(
            StringSpan value, StringComparison comparison)
            => source.Equals(value, (x, y) => x.Equals(y, comparison));

        // ------------------------------------------------

        int IndexOf(StringSpan value, Func<char, char, bool> predicate)
        {
            if (source.Length == 0 || value.Length == 0) return -1;
            if (value.Length > source.Length) return -1;

            var max = source.Length - value.Length;
            for (int i = 0; i <= max; i++)
            {
                var match = true; for (int j = 0; j < value.Length; j++)
                {
                    var s = source[i + j];
                    var v = value[j];
                    if (!predicate(s, v)) { match = false; break; }
                    ;
                }
                if (match) return i;
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(StringSpan value) => source.IndexOf(value, (x, y) => x == y);

        /// <summary>
        /// Returns the index of the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public int IndexOf(
            StringSpan value, bool ignoreCase)
            => source.IndexOf(value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Returns the index of the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int IndexOf(
            StringSpan value, IEqualityComparer<char> comparer)
            => source.IndexOf(value, (x, y) => comparer.Equals(x, y));

        /// <summary>
        /// Returns the index of the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int IndexOf(
            StringSpan value, IEqualityComparer<string> comparer)
            => source.IndexOf(value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the index of the first ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public int IndexOf(
            StringSpan value, StringComparison comparison)
            => source.IndexOf(value, (x, y) => x.Equals(y, comparison));

        // ------------------------------------------------

        int LastIndexOf(StringSpan value, Func<char, char, bool> predicate)
        {
            if (source.Length == 0 || value.Length == 0) return -1;
            if (value.Length > source.Length) return -1;

            var max = source.Length - value.Length;
            for (int i = max; i >= 0; i--)
            {
                var match = true; for (int j = 0; j < value.Length; j++)
                {
                    var s = source[i + j];
                    var v = value[j];
                    if (!predicate(s, v)) { match = false; break; }
                    ;
                }
                if (match) return i;
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int LastIndexOf(StringSpan value) => source.LastIndexOf(value, (x, y) => x == y);

        /// <summary>
        /// Returns the index of the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public int LastIndexOf(
            StringSpan value, bool ignoreCase)
            => source.LastIndexOf(value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Returns the index of the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int LastIndexOf(
            StringSpan value, IEqualityComparer<char> comparer)
            => source.LastIndexOf(value, (x, y) => comparer.Equals(x, y));

        /// <summary>
        /// Returns the index of the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int LastIndexOf(
            StringSpan value, IEqualityComparer<string> comparer)
            => source.LastIndexOf(value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the index of the last ocurrence of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public int LastIndexOf(
            StringSpan value, StringComparison comparison)
            => source.LastIndexOf(value, (x, y) => x.Equals(y, comparison));

        // ------------------------------------------------

        List<int> IndexesOf(StringSpan value, Func<char, char, bool> predicate)
        {
            if (source.Length == 0 || value.Length == 0) return [];
            if (value.Length > source.Length) return [];

            List<int> nums = [];
            var max = source.Length - value.Length;
            for (int i = 0; i <= max; i++)
            {
                var match = true; for (int j = 0; j < value.Length; j++)
                {
                    var s = source[i + j];
                    var v = value[j];
                    if (!predicate(s, v)) { match = false; break; }
                    ;
                }
                if (match) nums.Add(i);
            }
            return nums;
        }

        /// <summary>
        /// Returns the indexes of the ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<int> IndexesOf(StringSpan value) => source.IndexesOf(value, (x, y) => x == y);

        /// <summary>
        /// Returns the indexes of the ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            StringSpan value, bool ignoreCase)
            => source.IndexesOf(value, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Returns the indexes of the ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            StringSpan value, IEqualityComparer<char> comparer)
            => source.IndexesOf(value, (x, y) => comparer.Equals(x, y));

        /// <summary>
        /// Returns the indexes of the ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            StringSpan value, IEqualityComparer<string> comparer)
            => source.IndexesOf(value, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Returns the indexes of the ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public List<int> IndexesOf(
            StringSpan value, StringComparison comparison)
            => source.IndexesOf(value, (x, y) => x.Equals(y, comparison));

        // ------------------------------------------------

        /// <summary>
        /// Determines if the builder contains the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(StringSpan value) => source.IndexOf(value) >= 0;

        /// <summary>
        /// Determines if the builder contains the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public bool Contains(
            StringSpan value, bool ignoreCase) => source.IndexOf(value, ignoreCase) >= 0;

        /// <summary>
        /// Determines if the builder contains the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Contains(
            StringSpan value, IEqualityComparer<char> comparer) => source.IndexOf(value, comparer) >= 0;

        /// <summary>
        /// Determines if the builder contains the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Contains(
            StringSpan value, IEqualityComparer<string> comparer) => source.IndexOf(value, comparer) >= 0;

        /// <summary>
        /// Determines if the builder contains the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public bool Contains(
            StringSpan value, StringComparison comparison) => source.IndexOf(value, comparison) >= 0;

        // ------------------------------------------------

        /// <summary>
        /// Determines if the span starts with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool StartsWith(StringSpan value) => source.IndexOf(value) == 0;

        /// <summary>
        /// Determines if the span starts with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public bool StartsWith(
            StringSpan value, bool ignoreCase) => source.IndexOf(value, ignoreCase) == 0;

        /// <summary>
        /// Determines if the span starts with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool StartsWith(
            StringSpan value, IEqualityComparer<char> comparer)
            => source.IndexOf(value, comparer) == 0;

        /// <summary>
        /// Determines if the span starts with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool StartsWith(
            StringSpan value, IEqualityComparer<string> comparer)
            => source.IndexOf(value, comparer) == 0;

        /// <summary>
        /// Determines if the span starts with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public bool StartsWith(
            StringSpan value, StringComparison comparison)
            => source.IndexOf(value, comparison) == 0;

        // ------------------------------------------------

        /// <summary>
        /// Determines if the span ends with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool EndsWith(StringSpan value)
            => source.Length > 0
            && value.Length > 0
            && source.IndexOf(value) == (source.Length - value.Length);

        /// <summary>
        /// Determines if the span ends with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public bool EndsWith(StringSpan value, bool ignoreCase)
            => source.Length > 0
            && value.Length > 0
            && source.IndexOf(value, ignoreCase) == (source.Length - value.Length);

        /// <summary>
        /// Determines if the span ends with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool EndsWith(StringSpan value, IEqualityComparer<char> comparer)
            => source.Length > 0
            && value.Length > 0
            && source.IndexOf(value, comparer) == (source.Length - value.Length);

        /// <summary>
        /// Determines if the span ends with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool EndsWith(StringSpan value, IEqualityComparer<string> comparer)
            => source.Length > 0
            && value.Length > 0
            && source.IndexOf(value, comparer) == (source.Length - value.Length);

        /// <summary>
        /// Determines if the span ends with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public bool EndsWith(StringSpan value, StringComparison comparison)
            => source.Length > 0
            && value.Length > 0
            && source.IndexOf(value, comparison) == (source.Length - value.Length);

        // ------------------------------------------------

        /// <summary>
        /// Removes from the builder all leading ocurrences of blank-alike characters.
        /// </summary>
        /// <returns></returns>
        public StringBuilder TrimStart() => source.TrimStart(BLANKS);

        /// <summary>
        /// Removes from the builder all trailing ocurrences of blank-alike characters.
        /// </summary>
        /// <returns></returns>
        public StringBuilder TrimEnd() => source.TrimEnd(BLANKS);

        /// <summary>
        /// Removes from the builder all leading and trailing ocurrences of blank-alike characters.
        /// </summary>
        /// <returns></returns>
        public StringBuilder Trim() => source.TrimStart(BLANKS).TrimEnd(BLANKS);

        // ------------------------------------------------

        StringBuilder TrimStart(StringSpan values, Func<char, char, bool> predicate)
        {
            while (source.Length > 0)
            {
                var match = false;
                var item = source[0]; foreach (var value in values)
                {
                    if (predicate(value, item))
                    {
                        source.Remove(0, 1);
                        match = true;
                        break;
                    }
                }
                if (!match) break;
            }
            return source;
        }

        /// <summary>
        /// Removes from the builder all leading ocurrences of the given values.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StringBuilder TrimStart(
            StringSpan values) => source.TrimStart(values, (x, y) => x == y);

        /// <summary>
        /// Removes from the builder all leading ocurrences of the given values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public StringBuilder TrimStart(
            StringSpan values, bool ignoreCase)
            => source.TrimStart(values, (x, y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Removes from the builder all leading ocurrences of the given values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder TrimStart(
            StringSpan values, IEqualityComparer<char> comparer)
            => source.TrimStart(values, (x, y) => comparer.Equals(x, y));

        /// <summary>
        /// Removes from the builder all leading ocurrences of the given values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder TrimStart(
            StringSpan values, IEqualityComparer<string> comparer)
            => source.TrimStart(values, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Removes from the builder all leading ocurrences of the given values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public StringBuilder TrimStart(
            StringSpan values, StringComparison comparison)
            => source.TrimStart(values, (x, y) => x.Equals(y, comparison));

        // ------------------------------------------------

        StringBuilder TrimEnd(StringSpan values, Func<char, char, bool> predicate)
        {
            while (source.Length > 0)
            {
                var match = false;
                var item = source[^1]; foreach (var value in values)
                {
                    if (predicate(value, item))
                    {
                        source.Length--;
                        match = true;
                        break;
                    }
                }
                if (!match) break;
            }
            return source;
        }

        /// <summary>
        /// Removes from the builder all trailing ocurrences of the given values.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StringBuilder TrimEnd(
            StringSpan values) => source.TrimEnd(values, (x, y) => x == y);

        /// <summary>
        /// Removes from the builder all trailing ocurrences of the given values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public StringBuilder TrimEnd(
            StringSpan values, bool ignoreCase)
            => source.TrimEnd(values, (x,y) => x.Equals(y, ignoreCase));

        /// <summary>
        /// Removes from the builder all trailing ocurrences of the given values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder TrimEnd(
            StringSpan values, IEqualityComparer<char> comparer)
            => source.TrimEnd(values, (x, y) => comparer.Equals(x, y));

        /// <summary>
        /// Removes from the builder all trailing ocurrences of the given values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder TrimEnd(
            StringSpan values, IEqualityComparer<string> comparer)
            => source.TrimEnd(values, (x, y) => x.Equals(y, comparer));

        /// <summary>
        /// Removes from the builder all trailing ocurrences of the given values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public StringBuilder TrimEnd(
            StringSpan values, StringComparison comparison)
            => source.TrimEnd(values, (x, y) => x.Equals(y, comparison));

        // ------------------------------------------------

        /// <summary>
        /// Removes from the builder all leading and trailing ocurrences of the given values.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StringBuilder Trim(StringSpan values)
            => source.TrimStart(values).TrimEnd(values);

        /// <summary>
        /// Removes from the builder all leading and trailing ocurrences of the given values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public StringBuilder Trim(StringSpan values, bool ignoreCase)
            => source.TrimStart(values, ignoreCase).TrimEnd(values, ignoreCase);

        /// <summary>
        /// Removes from the builder all leading and trailing ocurrences of the given values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder Trim(
            StringSpan values, IEqualityComparer<char> comparer)
            => source.TrimStart(values, comparer).TrimEnd(values, comparer);

        /// <summary>
        /// Removes from the builder all leading and trailing ocurrences of the given values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public StringBuilder Trim(
            StringSpan values, IEqualityComparer<string> comparer)
            => source.TrimStart(values, comparer).TrimEnd(values, comparer);

        /// <summary>
        /// Removes from the builder all leading and trailing ocurrences of the given values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public StringBuilder Trim(
            StringSpan values, StringComparison comparison)
            => source.TrimStart(values, comparison).TrimEnd(values, comparison);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Maintains the collection of blank-alike charr values.
    /// </summary>
    static char[] BLANKS = [
        // Space-alike ones...
        ' ',      //(\u0020) Standard space
        '\u00A0', //Espacio de no separación (No-Break Space / &nbsp;).
        '\u1680', //Marca de espacio Ogham.
        '\u2000', //Espacio En (En Quad).
        '\u2001', //Espacio Em (Em Quad).
        '\u2002', //Espacio En (En Space).
        '\u2003', //Espacio Em (Em Space).
        '\u2004', //Espacio de un tercio de Em (Three-Per-Em Space).
        '\u2005', //Espacio de un cuarto de Em (Four-Per-Em Space).
        '\u2006', //Espacio de un sexto de Em (Six-Per-Em Space).
        '\u2007', //Espacio de figura (Figure Space).
        '\u2008', //Espacio de puntuación (Punctuation Space).
        '\u2009', //Espacio fino (Thin Space).
        '\u200A', //Espacio ultrafino (Hair Space).
        '\u202F', //Espacio estrecho de no separación (Narrow No-Break Space).
        '\u205F', //Espacio matemático medio (Medium Mathematical Space).
        '\u3000', //Espacio ideográfico (usado en escritura CJK).
        // Control-alike ones...
        '\t',     //(\u0009) Tabulación horizontal (Character Tabulation).
        '\n',     //(\u000A) Salto de línea (Line Feed).
        '\v',     //(\u000B) Tabulación vertical (Line Tabulation).
        '\f',     //(\u000C) Avance de página (Form Feed).
        '\r',     //(\u000D) Retorno de carro (Carriage Return).
        '\u0085', // Siguiente línea (Next Line / NEL).
        // Others...
        '\u2028', //Separador de linea
        '\u2029', //Separador de párrafo
        ];
}