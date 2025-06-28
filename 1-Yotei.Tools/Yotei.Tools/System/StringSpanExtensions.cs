using StrSpan = System.ReadOnlySpan<char>;

namespace Yotei.Tools;

// ========================================================
public static class StringSpanExtensions
{
    // ----------------------------------------------------
    // IndexOf(char)

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static int IndexOf(this StrSpan source, char value, bool sensitive)
    {
        for (int i = 0; i < source.Length; i++)
            if (source[i].Equals(value, sensitive)) return i;

        return -1;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(this StrSpan source, char value, IEqualityComparer<char> comparer)
    {
        comparer.ThrowWhenNull();

        for (int i = 0; i < source.Length; i++)
            if (source[i].Equals(value, comparer)) return i;

        return -1;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(this StrSpan source, char value, IEqualityComparer<string> comparer)
    {
        comparer.ThrowWhenNull();

        var temp = new string([value]);
        return source.IndexOf(temp, comparer);
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int IndexOf(this StrSpan source, char value, StringComparison comparison)
    {
        for (int i = 0; i < source.Length; i++)
            if (source[i].Equals(value, comparison)) return i;

        return -1;
    }

    // ----------------------------------------------------
    // LastIndexOf(char)

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static int LastIndexOf(this StrSpan source, char value, bool sensitive)
    {
        for (int i = source.Length - 1; i >= 0; i--)
            if (source[i].Equals(value, sensitive)) return i;

        return -1;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(this StrSpan source, char value, IEqualityComparer<char> comparer)
    {
        comparer.ThrowWhenNull();

        for (int i = source.Length - 1; i >= 0; i--)
            if (source[i].Equals(value, comparer)) return i;

        return -1;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(this StrSpan source, char value, IEqualityComparer<string> comparer)
    {
        comparer.ThrowWhenNull();

        var temp = new string([value]);
        return source.LastIndexOf(temp, comparer);
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int LastIndexOf(this StrSpan source, char value, StringComparison comparison)
    {
        for (int i = source.Length - 1; i >= 0; i--)
            if (source[i].Equals(value, comparison)) return i;

        return -1;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this StrSpan source, char value)
    {
        var list = new List<int>();
        var ini = 0;

        while (ini < source.Length)
        {
            var span = source[ini..];
            var index = span.IndexOf(value);

            if (index < 0) break;
            list.Add(ini + index);
            ini += (index + 1);
        }

        return list;
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this StrSpan source, char value, bool sensitive)
    {
        var list = new List<int>();
        var ini = 0;

        while (ini < source.Length)
        {
            var span = source[ini..];
            var index = span.IndexOf(value, sensitive);

            if (index < 0) break;
            list.Add(ini + index);
            ini += (index + 1);
        }

        return list;
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this StrSpan source, char value, IEqualityComparer<char> comparer)
    {
        var list = new List<int>();
        var ini = 0;

        while (ini < source.Length)
        {
            var span = source[ini..];
            var index = span.IndexOf(value, comparer);

            if (index < 0) break;
            list.Add(ini + index);
            ini += (index + 1);
        }

        return list;
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this StrSpan source, char value, IEqualityComparer<string> comparer)
    {
        var list = new List<int>();
        var ini = 0;

        while (ini < source.Length)
        {
            var span = source[ini..];
            var index = span.IndexOf(value, comparer);

            if (index < 0) break;
            list.Add(ini + index);
            ini += (index + 1);
        }

        return list;
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this StrSpan source, char value, StringComparison comparison)
    {
        var list = new List<int>();
        var ini = 0;

        while (ini < source.Length)
        {
            var span = source[ini..];
            var index = span.IndexOf(value, comparison);

            if (index < 0) break;
            list.Add(ini + index);
            ini += (index + 1);
        }

        return list;
    }

    // ----------------------------------------------------
    // IndexOf(span)
    // IndexOf(span, comparison)

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static int IndexOf(this StrSpan source, StrSpan value, bool sensitive)
    {
        if (source.Length == 0 && value.Length == 0) return 0; // To mimic standard behavior...
        if (source.Length == 0) return -1;
        if (value.Length == 0) return -1;

        for (int i = 0; i < source.Length; i++)
        {
            var span = source[i..];
            if (span.Length < value.Length) break;

            var found = true;
            for (int k = 0; k < value.Length; k++)
            {
                var s = span[k];
                var v = value[k];
                if (!s.Equals(v, sensitive)) { found = false; break; }
            }
            if (!found) continue;

            return i;
        }

        return -1;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(this StrSpan source, StrSpan value, IEqualityComparer<char> comparer)
    {
        if (source.Length == 0 && value.Length == 0) return 0; // To mimic standard behavior...
        if (source.Length == 0) return -1;
        if (value.Length == 0) return -1;

        for (int i = 0; i < source.Length; i++)
        {
            var span = source[i..];
            if (span.Length < value.Length) break;

            var found = true;
            for (int k = 0; k < value.Length; k++)
            {
                var s = span[k];
                var v = value[k];
                if (!s.Equals(v, comparer)) { found = false; break; }
            }
            if (!found) continue;

            return i;
        }

        return -1;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(this StrSpan source, StrSpan value, IEqualityComparer<string> comparer)
    {
        if (source.Length == 0 && value.Length == 0) return 0; // To mimic standard behavior...
        if (source.Length == 0) return -1;
        if (value.Length == 0) return -1;

        for (int i = 0; i < source.Length; i++)
        {
            var span = source[i..];
            if (span.Length < value.Length) break;

            var found = true;
            for (int k = 0; k < value.Length; k++)
            {
                var s = span[k];
                var v = value[k];
                if (!s.Equals(v, comparer)) { found = false; break; }
            }
            if (!found) continue;

            return i;
        }

        return -1;
    }

    // ----------------------------------------------------
    // LastIndexOf(span)
    // LastIndexOf(span, comparison)

    // TODO: Optimize StrSpan.LastIndexOf()...

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static int LastIndexOf(this StrSpan source, StrSpan value, bool sensitive)
    {
        if (source.Length == 0 && value.Length == 0) return 0; // To mimic standard behavior...
        if (source.Length == 0) return -1;
        if (value.Length == 0) return -1;

        var r = -1;

        for (int i = 0; i < source.Length; i++)
        {
            var span = source[i..];
            if (span.Length < value.Length) break;

            var found = true;
            for (int k = 0; k < value.Length; k++)
            {
                var s = span[k];
                var v = value[k];
                if (!s.Equals(v, sensitive)) { found = false; break; }
            }
            if (!found) continue;

            r = i;
        }

        return r;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(this StrSpan source, StrSpan value, IEqualityComparer<char> comparer)
    {
        if (source.Length == 0 && value.Length == 0) return 0; // To mimic standard behavior...
        if (source.Length == 0) return -1;
        if (value.Length == 0) return -1;

        var r = -1;

        for (int i = 0; i < source.Length; i++)
        {
            var span = source[i..];
            if (span.Length < value.Length) break;

            var found = true;
            for (int k = 0; k < value.Length; k++)
            {
                var s = span[k];
                var v = value[k];
                if (!s.Equals(v, comparer)) { found = false; break; }
            }
            if (!found) continue;

            r = i;
        }

        return r;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(this StrSpan source, StrSpan value, IEqualityComparer<string> comparer)
    {
        if (source.Length == 0 && value.Length == 0) return 0; // To mimic standard behavior...
        if (source.Length == 0) return -1;
        if (value.Length == 0) return -1;

        var r = -1;

        for (int i = 0; i < source.Length; i++)
        {
            var span = source[i..];
            if (span.Length < value.Length) break;

            var found = true;
            for (int k = 0; k < value.Length; k++)
            {
                var s = span[k];
                var v = value[k];
                if (!s.Equals(v, comparer)) { found = false; break; }
            }
            if (!found) continue;

            r = i;
        }

        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this StrSpan source, StrSpan value)
    {
        if (source.Length == 0 && value.Length == 0) return []; // To mimic standard behavior...
        if (source.Length == 0) return [];
        if (value.Length == 0) return [];

        var list = new List<int>();
        var ini = 0;

        while (ini < source.Length)
        {
            var span = source[ini..];
            var index = span.IndexOf(value);

            if (index < 0) break;
            list.Add(ini + index);
            ini += (index + value.Length);
        }

        return list;
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this StrSpan source, StrSpan value, bool sensitive)
    {
        if (source.Length == 0 && value.Length == 0) return []; // To mimic standard behavior...
        if (source.Length == 0) return [];
        if (value.Length == 0) return [];

        var list = new List<int>();
        var ini = 0;

        while (ini < source.Length)
        {
            var span = source[ini..];
            var index = span.IndexOf(value, sensitive);

            if (index < 0) break;
            list.Add(ini + index);
            ini += (index + value.Length);
        }

        return list;
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StrSpan source, StrSpan value, IEqualityComparer<char> comparer)
    {
        if (source.Length == 0 && value.Length == 0) return []; // To mimic standard behavior...
        if (source.Length == 0) return [];
        if (value.Length == 0) return [];

        var list = new List<int>();
        var ini = 0;

        while (ini < source.Length)
        {
            var span = source[ini..];
            var index = span.IndexOf(value, comparer);

            if (index < 0) break;
            list.Add(ini + index);
            ini += (index + value.Length);
        }

        return list;
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StrSpan source, StrSpan value, IEqualityComparer<string> comparer)
    {
        if (source.Length == 0 && value.Length == 0) return []; // To mimic standard behavior...
        if (source.Length == 0) return [];
        if (value.Length == 0) return [];

        var list = new List<int>();
        var ini = 0;

        while (ini < source.Length)
        {
            var span = source[ini..];
            var index = span.IndexOf(value, comparer);

            if (index < 0) break;
            list.Add(ini + index);
            ini += (index + value.Length);
        }

        return list;
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this StrSpan source, StrSpan value, StringComparison comparison)
    {
        if (source.Length == 0 && value.Length == 0) return []; // To mimic standard behavior...
        if (source.Length == 0) return [];
        if (value.Length == 0) return [];

        var list = new List<int>();
        var ini = 0;

        while (ini < source.Length)
        {
            var span = source[ini..];
            var index = span.IndexOf(value, comparison);

            if (index < 0) break;
            list.Add(ini + index);
            ini += (index + value.Length);
        }

        return list;
    }

    // ----------------------------------------------------
    // Contains(char)

    /// <summary>
    /// Determines if the given source contains the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static bool Contains(
        this StrSpan source, char value, bool sensitive) => source.IndexOf(value, sensitive) >= 0;

    /// <summary>
    /// Determines if the given source contains the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains(
        this StrSpan source, char value, IEqualityComparer<char> comparer)
        => source.IndexOf(value, comparer) >= 0;

    /// <summary>
    /// Determines if the given source contains the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains(
        this StrSpan source, char value, IEqualityComparer<string> comparer)
        => source.IndexOf(value, comparer) >= 0;

    /// <summary>
    /// Determines if the given source contains the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool Contains(
        this StrSpan source, char value, StringComparison comparison)
        => source.IndexOf(value, comparison) >= 0;

    // ----------------------------------------------------
    // Contains(span, comparison)

    /// <summary>
    /// Determines if the given source contains the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool Contains(
        this StrSpan source, StrSpan value) => source.IndexOf(value) >= 0;

    /// <summary>
    /// Determines if the given source contains the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static bool Contains(
        this StrSpan source, StrSpan value, bool sensitive) => source.IndexOf(value, sensitive) >= 0;

    /// <summary>
    /// Determines if the given source contains the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains(
        this StrSpan source, StrSpan value, IEqualityComparer<char> comparer)
        => source.IndexOf(value, comparer) >= 0;

    /// <summary>
    /// Determines if the given source contains the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains(
        this StrSpan source, StrSpan value, IEqualityComparer<string> comparer)
        => source.IndexOf(value, comparer) >= 0;

    // ----------------------------------------------------
    // ContainsAny(char, char)
    // ContainsAny(char, char, char)

    /// <summary>
    /// Determines if the given source contains any of the given values or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool ContainsAny(this StrSpan source, IEnumerable<char> values)
    {
        values.ThrowWhenNull();

        foreach (var value in values) if (source.IndexOf(value) >= 0) return true;
        return false;
    }

    /// <summary>
    /// Determines if the given source span contains any character from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static bool ContainsAny(this StrSpan source, IEnumerable<char> values, bool sensitive)
    {
        values.ThrowWhenNull();

        foreach (var value in values) if (source.IndexOf(value, sensitive) >= 0) return true;
        return false;
    }

    /// <summary>
    /// Determines if the given source span contains any character from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(this StrSpan source, IEnumerable<char> values, IEqualityComparer<char> comparer)
    {
        values.ThrowWhenNull();

        foreach (var value in values) if (source.IndexOf(value, comparer) >= 0) return true;
        return false;
    }

    /// <summary>
    /// Determines if the given source span contains any character from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(this StrSpan source, IEnumerable<char> values, IEqualityComparer<string> comparer)
    {
        values.ThrowWhenNull();

        foreach (var value in values) if (source.IndexOf(value, comparer) >= 0) return true;
        return false;
    }

    /// <summary>
    /// Determines if the given source span contains any character from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool ContainsAny(this StrSpan source, IEnumerable<char> values, StringComparison comparison)
    {
        values.ThrowWhenNull();

        foreach (var value in values) if (source.IndexOf(value, comparison) >= 0) return true;
        return false;
    }

    // ----------------------------------------------------
    // ContainsAny(span)

    /// <summary>
    /// Determines if the given source span contains any character from the given span, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static bool ContainsAny(this StrSpan source, in StrSpan values, bool sensitive)
    {
        if (source.Length == 0) return false;
        if (values.Length == 0) return false;

        foreach (var value in values) if (source.Contains(value, sensitive)) return true;
        return false;
    }

    /// <summary>
    /// Determines if the given source span contains any character from the given span, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(this StrSpan source, in StrSpan values, IEqualityComparer<char> comparer)
    {
        if (source.Length == 0) return false;
        if (values.Length == 0) return false;

        foreach (var value in values) if (source.Contains(value, comparer)) return true;
        return false;
    }

    /// <summary>
    /// Determines if the given source span contains any character from the given span, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(this StrSpan source, in StrSpan values, IEqualityComparer<string> comparer)
    {
        if (source.Length == 0) return false;
        if (values.Length == 0) return false;

        foreach (var value in values) if (source.Contains(value, comparer)) return true;
        return false;
    }

    /// <summary>
    /// Determines if the given source span contains any character from the given span, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool ContainsAny(this StrSpan source, in StrSpan values, StringComparison comparison)
    {
        if (source.Length == 0) return false;
        if (values.Length == 0) return false;

        foreach (var value in values) if (source.Contains(value, comparison)) return true;
        return false;
    }

    // ----------------------------------------------------
    // StartsWith(char)

    /// <summary>
    /// Determines if the given source starts with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static bool StartsWith(this StrSpan source, char value, bool sensitive)
    {
        if (source.Length > 0)
        {
            var index = source.IndexOf(value, sensitive);
            if (index == 0) return true;
        }
        return false;
    }

    /// <summary>
    /// Determines if the given source starts with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(this StrSpan source, char value, IEqualityComparer<char> comparer)
    {
        if (source.Length > 0)
        {
            var index = source.IndexOf(value, comparer);
            if (index == 0) return true;
        }
        return false;
    }

    /// <summary>
    /// Determines if the given source starts with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(this StrSpan source, char value, IEqualityComparer<string> comparer)
    {
        if (source.Length > 0)
        {
            var index = source.IndexOf(value, comparer);
            if (index == 0) return true;
        }
        return false;
    }

    /// <summary>
    /// Determines if the given source starts with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool StartsWith(this StrSpan source, char value, StringComparison comparison)
    {
        if (source.Length > 0)
        {
            var index = source.IndexOf(value, comparison);
            if (index == 0) return true;
        }
        return false;
    }

    // ----------------------------------------------------
    // StartsWith(span)
    // StartsWith(span, comparison)

    /// <summary>
    /// Determines if the given source starts with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static bool StartsWith(this StrSpan source, StrSpan value, bool sensitive)
    {
        if (source.Length == 0 && value.Length == 0) return true; // Mimics standard behavior...
        if (source.Length > 0 && value.Length > 0)
        {
            var index = source.IndexOf(value, sensitive);
            return index == 0;
        }
        return false;
    }

    /// <summary>
    /// Determines if the given source starts with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(this StrSpan source, StrSpan value, IEqualityComparer<char> comparer)
    {
        if (source.Length == 0 && value.Length == 0) return true; // Mimics standard behavior...
        if (source.Length > 0 && value.Length > 0)
        {
            var index = source.IndexOf(value, comparer);
            return index == 0;
        }
        return false;
    }

    /// <summary>
    /// Determines if the given source starts with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(this StrSpan source, StrSpan value, IEqualityComparer<string> comparer)
    {
        if (source.Length == 0 && value.Length == 0) return true; // Mimics standard behavior...
        if (source.Length > 0 && value.Length > 0)
        {
            var index = source.IndexOf(value, comparer);
            return index == 0;
        }
        return false;
    }

    // ----------------------------------------------------
    // EndsWith(char)

    /// <summary>
    /// Determines if the given source ends with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static bool EndsWith(this StrSpan source, char value, bool sensitive)
    {
        if (source.Length > 0)
        {
            var max = source.Length - 1;

            var index = source.IndexOf(value, sensitive);
            if (index == max) return true;
        }
        return false;
    }

    /// <summary>
    /// Determines if the given source ends with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(this StrSpan source, char value, IEqualityComparer<char> comparer)
    {
        if (source.Length > 0)
        {
            var max = source.Length - 1;

            var index = source.IndexOf(value, comparer);
            if (index == max) return true;
        }
        return false;
    }

    /// <summary>
    /// Determines if the given source ends with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(this StrSpan source, char value, IEqualityComparer<string> comparer)
    {
        if (source.Length > 0)
        {
            var max = source.Length - 1;

            var index = source.IndexOf(value, comparer);
            if (index == max) return true;
        }
        return false;
    }

    /// <summary>
    /// Determines if the given source ends with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool EndsWith(this StrSpan source, char value, StringComparison comparison)
    {
        if (source.Length > 0)
        {
            var max = source.Length - 1;

            var index = source.IndexOf(value, comparison);
            if (index == max) return true;
        }
        return false;
    }

    // ----------------------------------------------------
    // EndsWith(span)
    // EndsWith(span, comparison)

    /// <summary>
    /// Determines if the given source ends with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static bool EndsWith(this StrSpan source, StrSpan value, bool sensitive)
    {
        if (source.Length == 0 && value.Length == 0) return true; // Mimics standard behavior...
        if (source.Length > 0 && value.Length > 0)
        {
            var max = source.Length - value.Length;

            var index = source.LastIndexOf(value, sensitive);
            if (index == max) return true;
        }
        return false;
    }

    /// <summary>
    /// Determines if the given source ends with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(this StrSpan source, StrSpan value, IEqualityComparer<char> comparer)
    {
        if (source.Length == 0 && value.Length == 0) return true; // Mimics standard behavior...
        if (source.Length > 0 && value.Length > 0)
        {
            var max = source.Length - value.Length;

            var index = source.LastIndexOf(value, comparer);
            if (index == max) return true;
        }
        return false;
    }

    /// <summary>
    /// Determines if the given source ends with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(this StrSpan source, StrSpan value, IEqualityComparer<string> comparer)
    {
        if (source.Length == 0 && value.Length == 0) return true; // Mimics standard behavior...
        if (source.Length > 0 && value.Length > 0)
        {
            var max = source.Length - value.Length;

            var index = source.LastIndexOf(value, comparer);
            if (index == max) return true;
        }
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, char value, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.IndexOf(value);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == 1) return StrSpan.Empty;

        var array = new char[source.Length - 1];
        source[..index].CopyTo(array);

        var dest = array.AsSpan(index);
        var ini = index + 1;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, char value, bool sensitive, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.IndexOf(value, sensitive);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == 1) return StrSpan.Empty;

        var array = new char[source.Length - 1];
        source[..index].CopyTo(array);

        var dest = array.AsSpan(index);
        var ini = index + 1;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, char value, IEqualityComparer<char> comparer, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.IndexOf(value, comparer);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == 1) return StrSpan.Empty;

        var array = new char[source.Length - 1];
        source[..index].CopyTo(array);

        var dest = array.AsSpan(index);
        var ini = index + 1;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, char value, IEqualityComparer<string> comparer, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.IndexOf(value, comparer);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == 1) return StrSpan.Empty;

        var array = new char[source.Length - 1];
        source[..index].CopyTo(array);

        var dest = array.AsSpan(index);
        var ini = index + 1;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, char value, StringComparison comparison, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.IndexOf(value, comparison);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == 1) return StrSpan.Empty;

        var array = new char[source.Length - 1];
        source[..index].CopyTo(array);

        var dest = array.AsSpan(index);
        var ini = index + 1;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StrSpan Remove(
        this StrSpan source, char value) => source.Remove(value, out _);

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static StrSpan Remove(
        this StrSpan source, char value, bool sensitive) => source.Remove(value, sensitive, out _);

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan Remove(
        this StrSpan source, char value, IEqualityComparer<char> comparer)
        => source.Remove(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan Remove(
        this StrSpan source, char value, IEqualityComparer<string> comparer)
        => source.Remove(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StrSpan Remove(
        this StrSpan source, char value, StringComparison comparison)
        => source.Remove(value, comparison, out _);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, char value, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.LastIndexOf(value);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == 1) return StrSpan.Empty;

        var array = new char[source.Length - 1];
        source[..index].CopyTo(array);

        var dest = array.AsSpan(index);
        var ini = index + 1;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, char value, bool sensitive, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.LastIndexOf(value, sensitive);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == 1) return StrSpan.Empty;

        var array = new char[source.Length - 1];
        source[..index].CopyTo(array);

        var dest = array.AsSpan(index);
        var ini = index + 1;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, char value, IEqualityComparer<char> comparer, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.LastIndexOf(value, comparer);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == 1) return StrSpan.Empty;

        var array = new char[source.Length - 1];
        source[..index].CopyTo(array);

        var dest = array.AsSpan(index);
        var ini = index + 1;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, char value, IEqualityComparer<string> comparer, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.LastIndexOf(value, comparer);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == 1) return StrSpan.Empty;

        var array = new char[source.Length - 1];
        source[..index].CopyTo(array);

        var dest = array.AsSpan(index);
        var ini = index + 1;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, char value, StringComparison comparison, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.LastIndexOf(value, comparison);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == 1) return StrSpan.Empty;

        var array = new char[source.Length - 1];
        source[..index].CopyTo(array);

        var dest = array.AsSpan(index);
        var ini = index + 1;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(
        this StrSpan source, char value) => source.RemoveLast(value, out _);

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(
        this StrSpan source, char value, bool sensitive) => source.RemoveLast(value, sensitive, out _);

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(
        this StrSpan source, char value, IEqualityComparer<char> comparer)
        => source.RemoveLast(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(
        this StrSpan source, char value, IEqualityComparer<string> comparer)
        => source.RemoveLast(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(
        this StrSpan source, char value, StringComparison comparison)
        => source.RemoveLast(value, comparison, out _);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, char value, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, out var temp);
            if (!temp) break;
            removed = true;
        }
        return source;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, char value, bool sensitive, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, sensitive, out var temp);
            if (!temp) break;
            removed = true;
        }
        return source;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, char value, IEqualityComparer<char> comparer, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, comparer, out var temp);
            if (!temp) break;
            removed = true;
        }
        return source;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, char value, IEqualityComparer<string> comparer, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, comparer, out var temp);
            if (!temp) break;
            removed = true;
        }
        return source;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, char value, StringComparison comparison, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, comparison, out var temp);
            if (!temp) break;
            removed = true;
        }
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(
        this StrSpan source, char value) => source.RemoveAll(value, out _);

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(
        this StrSpan source, char value, bool sensitive) => source.RemoveAll(value, sensitive, out _);

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(
        this StrSpan source, char value, IEqualityComparer<char> comparer)
        => source.RemoveAll(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(
        this StrSpan source, char value, IEqualityComparer<string> comparer)
        => source.RemoveAll(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(
        this StrSpan source, char value, StringComparison comparison)
        => source.RemoveAll(value, comparison, out _);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, StrSpan value, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.IndexOf(value);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return StrSpan.Empty;

        var array = new char[source.Length - value.Length];
        source[..index].CopyTo(array);

        var dest = array.AsSpan(index);
        var ini = index + value.Length;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, StrSpan value, bool sensitive, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.IndexOf(value, sensitive);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return StrSpan.Empty;

        var array = new char[source.Length - value.Length];
        source[..index].CopyTo(array);

        var dest = array.AsSpan(index);
        var ini = index + value.Length;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, StrSpan value, IEqualityComparer<char> comparer, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.IndexOf(value, comparer);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return StrSpan.Empty;

        var array = new char[source.Length - value.Length];
        source[..index].CopyTo(array);

        var dest = array.AsSpan(index);
        var ini = index + value.Length;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, StrSpan value, IEqualityComparer<string> comparer, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.IndexOf(value, comparer);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return StrSpan.Empty;

        var array = new char[source.Length - value.Length];
        source[..index].CopyTo(array);

        var dest = array.AsSpan(index);
        var ini = index + value.Length;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, StrSpan value, StringComparison comparison, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.IndexOf(value, comparison);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return StrSpan.Empty;

        var array = new char[source.Length - value.Length];
        source[..index].CopyTo(array);

        var dest = array.AsSpan(index);
        var ini = index + value.Length;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StrSpan Remove(
        this StrSpan source, StrSpan value) => source.Remove(value, out _);

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static StrSpan Remove(
        this StrSpan source, StrSpan value, bool sensitive) => source.Remove(value, sensitive, out _);

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan Remove(
        this StrSpan source, StrSpan value, IEqualityComparer<char> comparer)
        => source.Remove(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan Remove(
        this StrSpan source, StrSpan value, IEqualityComparer<string> comparer)
        => source.Remove(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StrSpan Remove(
        this StrSpan source, StrSpan value, StringComparison comparison)
        => source.Remove(value, comparison, out _);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, StrSpan value, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.LastIndexOf(value);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return StrSpan.Empty;

        var array = new char[source.Length - value.Length];
        source[..index].CopyTo(array);

        var dest = array.AsSpan(index);
        var ini = index + value.Length;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, StrSpan value, bool sensitive, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.LastIndexOf(value, sensitive);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return StrSpan.Empty;

        var array = new char[source.Length - value.Length];
        source[..index].CopyTo(array);

        var dest = array.AsSpan(index);
        var ini = index + value.Length;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, StrSpan value, IEqualityComparer<char> comparer, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.LastIndexOf(value, comparer);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return StrSpan.Empty;

        var array = new char[source.Length - value.Length];
        source[..index].CopyTo(array);

        var dest = array.AsSpan(index);
        var ini = index + value.Length;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, StrSpan value, IEqualityComparer<string> comparer, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.LastIndexOf(value, comparer);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return StrSpan.Empty;

        var array = new char[source.Length - value.Length];
        source[..index].CopyTo(array);

        var dest = array.AsSpan(index);
        var ini = index + value.Length;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, StrSpan value, StringComparison comparison, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.LastIndexOf(value, comparison);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return StrSpan.Empty;

        var array = new char[source.Length - value.Length];
        source[..index].CopyTo(array);

        var dest = array.AsSpan(index);
        var ini = index + value.Length;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(
        this StrSpan source, StrSpan value) => source.RemoveLast(value, out _);

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(
        this StrSpan source, StrSpan value, bool sensitive)
        => source.RemoveLast(value, sensitive, out _);

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(
        this StrSpan source, StrSpan value, IEqualityComparer<char> comparer)
        => source.RemoveLast(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(
        this StrSpan source, StrSpan value, IEqualityComparer<string> comparer)
        => source.RemoveLast(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(
        this StrSpan source, StrSpan value, StringComparison comparison)
        => source.RemoveLast(value, comparison, out _);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, StrSpan value, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, out var temp);
            if (!temp) break;
            removed = true;
        }
        return source;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, StrSpan value, bool sensitive, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, sensitive, out var temp);
            if (!temp) break;
            removed = true;
        }
        return source;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, StrSpan value, IEqualityComparer<char> comparer, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, comparer, out var temp);
            if (!temp) break;
            removed = true;
        }
        return source;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, StrSpan value, IEqualityComparer<string> comparer, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, comparer, out var temp);
            if (!temp) break;
            removed = true;
        }
        return source;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, StrSpan value, StringComparison comparison, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, comparison, out var temp);
            if (!temp) break;
            removed = true;
        }
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(
        this StrSpan source, StrSpan value) => source.RemoveAll(value, out _);

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(
        this StrSpan source, StrSpan value, bool sensitive)
        => source.RemoveAll(value, sensitive, out _);

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(
        this StrSpan source, StrSpan value, IEqualityComparer<char> comparer)
        => source.RemoveAll(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(
        this StrSpan source, StrSpan value, IEqualityComparer<string> comparer)
        => source.RemoveAll(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(
        this StrSpan source, StrSpan value, StringComparison comparison)
        => source.RemoveAll(value, comparison, out _);
}