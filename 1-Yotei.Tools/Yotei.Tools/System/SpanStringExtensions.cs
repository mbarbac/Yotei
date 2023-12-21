using Span = System.ReadOnlySpan<char>;

namespace Yotei.Tools;

// ========================================================
public static class SpanStringExtensions
{
    /// <summary>
    /// Determines if the given source contains the given char, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public static bool Contains(this Span source, char c) => source.IndexOf(c) >= 0;

    /// <summary>
    /// Determines if the given source contains the given char, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static bool Contains(this Span source, char c, bool caseSensitive) => source.IndexOf(c, caseSensitive) >= 0;

    /// <summary>
    /// Determines if the given source contains the given char, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static bool Contains(this Span source, char c, IEqualityComparer<char> comparer) => source.IndexOf(c, comparer) >= 0;

    /// <summary>
    /// Determines if the given source contains the given char, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static bool Contains(this Span source, char c, IEqualityComparer<string> comparer) => source.IndexOf(c, comparer) >= 0;

    /// <summary>
    /// Determines if the given source contains the given char, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool Contains(this Span source, char c, StringComparison comparison) => source.IndexOf(c, comparison) >= 0;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given source contains any char from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="array"></param>
    /// <returns></returns>
    public static bool ContainsAny(this Span source, IEnumerable<char> array)
    {
        array.ThrowWhenNull();

        foreach(var c in array) if (source.IndexOf(c) >= 0) return true;
        return false;
    }

    /// <summary>
    /// Determines if the given source contains any char from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="array"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static bool ContainsAny(this Span source, IEnumerable<char> array, bool caseSensitive)
    {
        array.ThrowWhenNull();

        foreach (var c in array) if (source.IndexOf(c, caseSensitive) >= 0) return true;
        return false;
    }

    /// <summary>
    /// Determines if the given source contains any char from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="array"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(this Span source, IEnumerable<char> array, IEqualityComparer<char> comparer)
    {
        array.ThrowWhenNull();

        foreach (var c in array) if (source.IndexOf(c, comparer) >= 0) return true;
        return false;
    }

    /// <summary>
    /// Determines if the given source contains any char from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="array"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(this Span source, IEnumerable<char> array, IEqualityComparer<string> comparer)
    {
        array.ThrowWhenNull();

        foreach (var c in array) if (source.IndexOf(c, comparer) >= 0) return true;
        return false;
    }

    /// <summary>
    /// Determines if the given source contains any char from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="array"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool ContainsAny(this Span source, IEnumerable<char> array, StringComparison comparison)
    {
        array.ThrowWhenNull();

        foreach (var c in array) if (source.IndexOf(c, comparison) >= 0) return true;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the first ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static int IndexOf(this Span source, char c, bool caseSensitive)
    {
        for (int i = 0; i < source.Length; i++)
            if (source[i].Equals(c, caseSensitive)) return i;

        return -1;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(this Span source, char c, IEqualityComparer<char> comparer)
    {
        for (int i = 0; i < source.Length; i++)
            if (source[i].Equals(c, comparer)) return i;

        return -1;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(this Span source, char c, IEqualityComparer<string> comparer)
    {
        for (int i = 0; i < source.Length; i++)
            if (source[i].Equals(c, comparer)) return i;

        return -1;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int IndexOf(this Span source, char c, StringComparison comparison)
    {
        for (int i = 0; i < source.Length; i++)
            if (source[i].Equals(c, comparison)) return i;

        return -1;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the last ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static int LastIndexOf(this Span source, char c, bool caseSensitive)
    {
        for (int i = source.Length - 1; i >= 0; i--)
            if (source[i].Equals(c, caseSensitive)) return i;

        return -1;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(this Span source, char c, IEqualityComparer<char> comparer)
    {
        for (int i = source.Length - 1; i >= 0; i--)
            if (source[i].Equals(c, comparer)) return i;

        return -1;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(this Span source, char c, IEqualityComparer<string> comparer)
    {
        for (int i = source.Length - 1; i >= 0; i--)
            if (source[i].Equals(c, comparer)) return i;

        return -1;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int LastIndexOf(this Span source, char c, StringComparison comparison)
    {
        for (int i = source.Length - 1; i >= 0; i--)
            if (source[i].Equals(c, comparison)) return i;

        return -1;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the first ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static int IndexOf(this Span source, Span value, bool caseSensitive)
    {
        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0) return -1;
        if (value.Length == 0) return 0;

        for (int i = 0; i < source.Length; i++)
        {
            var found = true; for (int j = 0; j < value.Length && (i + j) < source.Length; j++)
            {
                var s = source[i + j];
                var v = value[j];
                if (!s.Equals(v, caseSensitive)) { found = false; break; }
            }
            if (found) return i;
        }
        return -1;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(this Span source, Span value, IEqualityComparer<char> comparer)
    {
        comparer.ThrowWhenNull();

        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0) return -1;
        if (value.Length == 0) return 0;

        for (int i = 0; i < source.Length; i++)
        {
            var found = true; for (int j = 0; j < value.Length && (i + j) < source.Length; j++)
            {
                var s = source[i + j];
                var v = value[j];
                if (!s.Equals(v, comparer)) { found = false; break; }
            }
            if (found) return i;
        }
        return -1;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(this Span source, Span value, IEqualityComparer<string> comparer)
    {
        comparer.ThrowWhenNull();

        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0) return -1;
        if (value.Length == 0) return 0;

        for (int i = 0; i < source.Length; i++)
        {
            var found = true; for (int j = 0; j < value.Length && (i + j) < source.Length; j++)
            {
                var s = source[i + j];
                var v = value[j];
                if (!s.Equals(v, comparer)) { found = false; break; }
            }
            if (found) return i;
        }
        return -1;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int IndexOf(this Span source, Span value, StringComparison comparison)
    {
        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0) return -1;
        if (value.Length == 0) return 0;

        for (int i = 0; i < source.Length; i++)
        {
            var found = true; for (int j = 0; j < value.Length && (i + j) < source.Length; j++)
            {
                var s = source[i + j];
                var v = value[j];
                if (!s.Equals(v, comparison)) { found = false; break; }
            }
            if (found) return i;
        }
        return -1;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the last ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static int LastIndexOf(this Span source, Span value, bool caseSensitive)
    {
        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0) return -1;
        if (value.Length == 0) return 0;

        for (int i = source.Length - 1; i >= 0; i--)
        {
            var found = true; for (int j = 0; j < value.Length && (i + j) < source.Length; j++)
            {
                var s = source[i + j];
                var v = value[j];
                if (!s.Equals(v, caseSensitive)) { found = false; break; }
            }
            if (found) return i;
        }
        return -1;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(this Span source, Span value, IEqualityComparer<char> comparer)
    {
        comparer.ThrowWhenNull();

        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0) return -1;
        if (value.Length == 0) return 0;

        for (int i = source.Length - 1; i >= 0; i--)
        {
            var found = true; for (int j = 0; j < value.Length && (i + j) < source.Length; j++)
            {
                var s = source[i + j];
                var v = value[j];
                if (!s.Equals(v, comparer)) { found = false; break; }
            }
            if (found) return i;
        }
        return -1;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(this Span source, Span value, IEqualityComparer<string> comparer)
    {
        comparer.ThrowWhenNull();

        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0) return -1;
        if (value.Length == 0) return 0;

        for (int i = source.Length - 1; i >= 0; i--)
        {
            var found = true; for (int j = 0; j < value.Length && (i + j) < source.Length; j++)
            {
                var s = source[i + j];
                var v = value[j];
                if (!s.Equals(v, comparer)) { found = false; break; }
            }
            if (found) return i;
        }
        return -1;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int LastIndexOf(this Span source, Span value, StringComparison comparison)
    {
        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0) return -1;
        if (value.Length == 0) return 0;

        for (int i = source.Length - 1; i >= 0; i--)
        {
            var found = true; for (int j = 0; j < value.Length && (i + j) < source.Length; j++)
            {
                var s = source[i + j];
                var v = value[j];
                if (!s.Equals(v, comparison)) { found = false; break; }
            }
            if (found) return i;
        }
        return -1;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static Span Remove(this Span source, Span value, out bool removed)
    {
        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.IndexOf(value);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return Span.Empty;

        var array = new char[source.Length - value.Length];
        source.Slice(0, index).CopyTo(array);
        source.Slice(index + value.Length).CopyTo(array.AsSpan(value.Length));
        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Span Remove(this Span source, Span value) => Remove(source, value, out _);

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static Span Remove(this Span source, Span value, bool caseSensitive, out bool removed)
    {
        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.IndexOf(value, caseSensitive);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return Span.Empty;

        var array = new char[source.Length - value.Length];
        source.Slice(0, index).CopyTo(array);
        source.Slice(index + value.Length).CopyTo(array.AsSpan(value.Length));
        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static Span Remove(this Span source, Span value, bool caseSensitive) => Remove(source, value, caseSensitive, out _);

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static Span Remove(this Span source, Span value, IEqualityComparer<char> comparer, out bool removed)
    {
        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.IndexOf(value, comparer);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return Span.Empty;

        var array = new char[source.Length - value.Length];
        source.Slice(0, index).CopyTo(array);
        source.Slice(index + value.Length).CopyTo(array.AsSpan(value.Length));
        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static Span Remove(this Span source, Span value, IEqualityComparer<char> comparer) => Remove(source, value, comparer, out _);

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static Span Remove(this Span source, Span value, IEqualityComparer<string> comparer, out bool removed)
    {
        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.IndexOf(value, comparer);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return Span.Empty;

        var array = new char[source.Length - value.Length];
        source.Slice(0, index).CopyTo(array);
        source.Slice(index + value.Length).CopyTo(array.AsSpan(value.Length));
        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static Span Remove(this Span source, Span value, IEqualityComparer<string> comparer) => Remove(source, value, comparer, out _);

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static Span Remove(this Span source, Span value, StringComparison comparison, out bool removed)
    {
        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.IndexOf(value, comparison);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return Span.Empty;

        var array = new char[source.Length - value.Length];
        source.Slice(0, index).CopyTo(array);
        source.Slice(index + value.Length).CopyTo(array.AsSpan(value.Length));
        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static Span Remove(this Span source, Span value, StringComparison comparison) => Remove(source, value, comparison, out _);

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static Span RemoveLast(this Span source, Span value, out bool removed)
    {
        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.LastIndexOf(value);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return Span.Empty;

        var array = new char[source.Length - value.Length];
        source.Slice(0, index).CopyTo(array);
        source.Slice(index + value.Length).CopyTo(array.AsSpan(value.Length));
        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Span RemoveLast(this Span source, Span value) => RemoveLast(source, value, out _);

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static Span RemoveLast(this Span source, Span value, bool caseSensitive, out bool removed)
    {
        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.LastIndexOf(value, caseSensitive);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return Span.Empty;

        var array = new char[source.Length - value.Length];
        source.Slice(0, index).CopyTo(array);
        source.Slice(index + value.Length).CopyTo(array.AsSpan(value.Length));
        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static Span RemoveLast(this Span source, Span value, bool caseSensitive) => RemoveLast(source, value, caseSensitive, out _);

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static Span RemoveLast(this Span source, Span value, IEqualityComparer<char> comparer, out bool removed)
    {
        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.LastIndexOf(value, comparer);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return Span.Empty;

        var array = new char[source.Length - value.Length];
        source.Slice(0, index).CopyTo(array);
        source.Slice(index + value.Length).CopyTo(array.AsSpan(value.Length));
        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static Span RemoveLast(this Span source, Span value, IEqualityComparer<char> comparer) => RemoveLast(source, value, comparer, out _);

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static Span RemoveLast(this Span source, Span value, IEqualityComparer<string> comparer, out bool removed)
    {
        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.LastIndexOf(value, comparer);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return Span.Empty;

        var array = new char[source.Length - value.Length];
        source.Slice(0, index).CopyTo(array);
        source.Slice(index + value.Length).CopyTo(array.AsSpan(value.Length));
        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static Span RemoveLast(this Span source, Span value, IEqualityComparer<string> comparer) => RemoveLast(source, value, comparer, out _);

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static Span RemoveLast(this Span source, Span value, StringComparison comparison, out bool removed)
    {
        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.LastIndexOf(value, comparison);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return Span.Empty;

        var array = new char[source.Length - value.Length];
        source.Slice(0, index).CopyTo(array);
        source.Slice(index + value.Length).CopyTo(array.AsSpan(value.Length));
        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static Span RemoveLast(this Span source, Span value, StringComparison comparison) => RemoveLast(source, value, comparison, out _);

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static Span RemoveAll(this Span source, Span value, out bool removed)
    {
        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, out var temp); if (!temp) break;
            removed = true;
        }
        return source;
    }

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Span RemoveAll(this Span source, Span value) => RemoveAll(source, value, out _);

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static Span RemoveAll(this Span source, Span value, bool caseSensitive, out bool removed)
    {
        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, caseSensitive, out var temp); if (!temp) break;
            removed = true;
        }
        return source;
    }

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static Span RemoveAll(this Span source, Span value, bool caseSensitive) => RemoveAll(source, value, caseSensitive, out _);

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static Span RemoveAll(this Span source, Span value, IEqualityComparer<char> comparer, out bool removed)
    {
        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, comparer, out var temp); if (!temp) break;
            removed = true;
        }
        return source;
    }

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static Span RemoveAll(this Span source, Span value, IEqualityComparer<char> comparer) => RemoveAll(source, value, comparer, out _);

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static Span RemoveAll(this Span source, Span value, IEqualityComparer<string> comparer, out bool removed)
    {
        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, comparer, out var temp); if (!temp) break;
            removed = true;
        }
        return source;
    }

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static Span RemoveAll(this Span source, Span value, IEqualityComparer<string> comparer) => RemoveAll(source, value, comparer, out _);

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static Span RemoveAll(this Span source, Span value, StringComparison comparison, out bool removed)
    {
        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, comparison, out var temp); if (!temp) break;
            removed = true;
        }
        return source;
    }

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static Span RemoveAll(this Span source, Span value, StringComparison comparison) => RemoveAll(source, value, comparison, out _);
}