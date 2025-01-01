using StringSpan = System.ReadOnlySpan<char>;

namespace Yotei.Tools;

// ========================================================
public static class StringSpanExtensions
{
    // ----------------------------------------------------
    // int IndexOf(char)

    /// <summary>
    /// Returns the index of the first ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static int IndexOf(this StringSpan source, char c, bool caseSensitive)
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
    public static int IndexOf(this StringSpan source, char c, IEqualityComparer<char> comparer)
    {
        comparer.ThrowWhenNull();

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
    public static int IndexOf(this StringSpan source, char c, IEqualityComparer<string> comparer)
    {
        comparer.ThrowWhenNull();

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
    public static int IndexOf(this StringSpan source, char c, StringComparison comparison)
    {
        for (int i = 0; i < source.Length; i++)
            if (source[i].Equals(c, comparison)) return i;

        return -1;
    }

    // ----------------------------------------------------
    // int LastIndexOf(char)

    /// <summary>
    /// Returns the index of the last ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static int LastIndexOf(this StringSpan source, char c, bool caseSensitive)
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
    public static int LastIndexOf(this StringSpan source, char c, IEqualityComparer<char> comparer)
    {
        comparer.ThrowWhenNull();

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
    public static int LastIndexOf(this StringSpan source, char c, IEqualityComparer<string> comparer)
    {
        comparer.ThrowWhenNull();

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
    public static int LastIndexOf(this StringSpan source, char c, StringComparison comparison)
    {
        for (int i = source.Length - 1; i >= 0; i--)
            if (source[i].Equals(c, comparison)) return i;

        return -1;
    }

    // ----------------------------------------------------
    // int IndexOf(CharSpan)
    // int IndexOf(CharSpan, StringComparison)

    /// <summary>
    /// Returns the index of the first ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static int IndexOf(this StringSpan source, StringSpan value, bool caseSensitive)
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
    public static int IndexOf(this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
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
    public static int IndexOf(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
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

    // ----------------------------------------------------
    // int LastIndexOf(CharSpan)
    // int LastIndexOf(CharSpan, StringComparison)

    /// <summary>
    /// Returns the index of the last ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static int LastIndexOf(this StringSpan source, StringSpan value, bool caseSensitive)
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
    public static int LastIndexOf(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
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
    public static int LastIndexOf(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
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

    // ----------------------------------------------------
    // bool Contains(value)

    /// <summary>
    /// Determines if the given source contains the given char, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, char c, bool caseSensitive) => source.IndexOf(c, caseSensitive) >= 0;

    /// <summary>
    /// Determines if the given source contains the given char, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, char c, IEqualityComparer<char> comparer)
        => source.IndexOf(c, comparer) >= 0;

    /// <summary>
    /// Determines if the given source contains the given char, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, char c, IEqualityComparer<string> comparer)
        => source.IndexOf(c, comparer) >= 0;

    /// <summary>
    /// Determines if the given source contains the given char, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, char c, StringComparison comparison)
        => source.IndexOf(c, comparison) >= 0;

    // ----------------------------------------------------
    // bool Contains(CharSpan, StringComparison)

    /// <summary>
    /// Determines if the given source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, StringSpan value, bool caseSensitive)
        => source.IndexOf(value, caseSensitive) >= 0;

    /// <summary>
    /// Determines if the given source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => source.IndexOf(value, comparer) >= 0;

    /// <summary>
    /// Determines if the given source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => source.IndexOf(value, comparer) >= 0;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given source contains any char from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="array"></param>
    /// <returns></returns>
    public static bool ContainsAny(this StringSpan source, IEnumerable<char> array)
    {
        array.ThrowWhenNull();

        foreach (var c in array) if (source.IndexOf(c) >= 0) return true;
        return false;
    }

    /// <summary>
    /// Determines if the given source contains any char from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="array"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static bool ContainsAny(this StringSpan source, IEnumerable<char> array, bool caseSensitive)
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
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<char> array, IEqualityComparer<char> comparer)
    {
        array.ThrowWhenNull();
        comparer.ThrowWhenNull();

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
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<char> array, IEqualityComparer<string> comparer)
    {
        array.ThrowWhenNull();
        comparer.ThrowWhenNull();

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
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<char> array, StringComparison comparison)
    {
        array.ThrowWhenNull();

        foreach (var c in array) if (source.IndexOf(c, comparison) >= 0) return true;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given source the first ocurrence of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public static StringSpan Remove(this StringSpan source, char c) => Remove(source, c, out _);

    internal static StringSpan Remove(this StringSpan source, char c, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.IndexOf(c);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == 1) return StringSpan.Empty;

        var array = new char[source.Length - 1];
        source[..index].CopyTo(array);
        var dest = array.AsSpan(index);
        var ini = index + 1;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the first ocurrence of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, char c, bool caseSensitive)
        => Remove(source, c, caseSensitive, out _);

    internal static StringSpan Remove(
        this StringSpan source, char c, bool caseSensitive, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.IndexOf(c, caseSensitive);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == 1) return StringSpan.Empty;

        var array = new char[source.Length - 1];
        source[..index].CopyTo(array);
        var dest = array.AsSpan(index);
        var ini = index + 1;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the first ocurrence of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, char c, IEqualityComparer<char> comparer)
        => Remove(source, c, comparer, out _);

    internal static StringSpan Remove(
        this StringSpan source, char c, IEqualityComparer<char> comparer, out bool removed)
    {
        comparer.ThrowWhenNull();

        removed = false;
        if (source.Length == 0) return source;

        var index = source.IndexOf(c, comparer);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == 1) return StringSpan.Empty;

        var array = new char[source.Length - 1];
        source[..index].CopyTo(array);
        var dest = array.AsSpan(index);
        var ini = index + 1;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the first ocurrence of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, char c, IEqualityComparer<string> comparer)
        => Remove(source, c, comparer, out _);

    internal static StringSpan Remove(
        this StringSpan source, char c, IEqualityComparer<string> comparer, out bool removed)
    {
        comparer.ThrowWhenNull();

        removed = false;
        if (source.Length == 0) return source;

        var index = source.IndexOf(c, comparer);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == 1) return StringSpan.Empty;

        var array = new char[source.Length - 1];
        source[..index].CopyTo(array);
        var dest = array.AsSpan(index);
        var ini = index + 1;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the first ocurrence of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, char c, StringComparison comparison)
        => Remove(source, c, comparison, out _);

    internal static StringSpan Remove(
        this StringSpan source, char c, StringComparison comparison, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.IndexOf(c, comparison);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == 1) return StringSpan.Empty;

        var array = new char[source.Length - 1];
        source[..index].CopyTo(array);
        var dest = array.AsSpan(index);
        var ini = index + 1;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given source the last ocurrence of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, char c) => RemoveLast(source, c, out _);

    internal static StringSpan RemoveLast(this StringSpan source, char c, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.LastIndexOf(c);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == 1) return StringSpan.Empty;

        var array = new char[source.Length - 1];
        source[..index].CopyTo(array);
        var dest = array.AsSpan(index);
        var ini = index + 1;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the last ocurrence of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, char c, bool caseSensitive)
        => RemoveLast(source, c, caseSensitive, out _);

    internal static StringSpan RemoveLast(
        this StringSpan source, char c, bool caseSensitive, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.LastIndexOf(c, caseSensitive);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == 1) return StringSpan.Empty;

        var array = new char[source.Length - 1];
        source[..index].CopyTo(array);
        var dest = array.AsSpan(index);
        var ini = index + 1;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the last ocurrence of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, char c, IEqualityComparer<char> comparer)
        => RemoveLast(source, c, comparer, out _);

    internal static StringSpan RemoveLast(
        this StringSpan source, char c, IEqualityComparer<char> comparer, out bool removed)
    {
        comparer.ThrowWhenNull();

        removed = false;
        if (source.Length == 0) return source;

        var index = source.LastIndexOf(c, comparer);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == 1) return StringSpan.Empty;

        var array = new char[source.Length - 1];
        source[..index].CopyTo(array);
        var dest = array.AsSpan(index);
        var ini = index + 1;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the last ocurrence of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, char c, IEqualityComparer<string> comparer)
        => RemoveLast(source, c, comparer, out _);

    internal static StringSpan RemoveLast(
        this StringSpan source, char c, IEqualityComparer<string> comparer, out bool removed)
    {
        comparer.ThrowWhenNull();

        removed = false;
        if (source.Length == 0) return source;

        var index = source.LastIndexOf(c, comparer);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == 1) return StringSpan.Empty;

        var array = new char[source.Length - 1];
        source[..index].CopyTo(array);
        var dest = array.AsSpan(index);
        var ini = index + 1;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the last ocurrence of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, char c, StringComparison comparison)
        => RemoveLast(source, c, comparison, out _);

    internal static StringSpan RemoveLast(
        this StringSpan source, char c, StringComparison comparison, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.LastIndexOf(c, comparison);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == 1) return StringSpan.Empty;

        var array = new char[source.Length - 1];
        source[..index].CopyTo(array);
        var dest = array.AsSpan(index);
        var ini = index + 1;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given source all ocurrences of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(this StringSpan source, char c) => RemoveAll(source, c, out _);

    internal static StringSpan RemoveAll(this StringSpan source, char c, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        while (true)
        {
            source = source.Remove(c, out var temp); if (!temp) break;
            removed = true;
        }
        return source;
    }

    /// <summary>
    /// Removes from the given source all ocurrences of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, char c, bool caseSensitive)
        => RemoveAll(source, c, caseSensitive, out _);

    internal static StringSpan RemoveAll(
        this StringSpan source, char c, bool caseSensitive, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        while (true)
        {
            source = source.Remove(c, caseSensitive, out var temp); if (!temp) break;
            removed = true;
        }
        return source;
    }

    /// <summary>
    /// Removes from the given source all ocurrences of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, char c, IEqualityComparer<char> comparer)
        => RemoveAll(source, c, comparer, out _);

    internal static StringSpan RemoveAll(
        this StringSpan source, char c, IEqualityComparer<char> comparer, out bool removed)
    {
        comparer.ThrowWhenNull();

        removed = false;
        if (source.Length == 0) return source;

        while (true)
        {
            source = source.Remove(c, comparer, out var temp); if (!temp) break;
            removed = true;
        }
        return source;
    }

    /// <summary>
    /// Removes from the given source all ocurrences of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, char c, IEqualityComparer<string> comparer)
        => RemoveAll(source, c, comparer, out _);

    internal static StringSpan RemoveAll(
        this StringSpan source, char c, IEqualityComparer<string> comparer, out bool removed)
    {
        comparer.ThrowWhenNull();

        removed = false;
        if (source.Length == 0) return source;

        while (true)
        {
            source = source.Remove(c, comparer, out var temp); if (!temp) break;
            removed = true;
        }
        return source;
    }

    /// <summary>
    /// Removes from the given source all ocurrences of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, char c, StringComparison comparison)
        => RemoveAll(source, c, comparison, out _);

    internal static StringSpan RemoveAll(
        this StringSpan source, char c, StringComparison comparison, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        while (true)
        {
            source = source.Remove(c, comparison, out var temp); if (!temp) break;
            removed = true;
        }
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value)
        => Remove(source, value, out _);

    internal static StringSpan Remove(this StringSpan source, StringSpan value, out bool removed)
    {
        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.IndexOf(value);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return StringSpan.Empty;

        var array = new char[source.Length - value.Length];
        source[..index].CopyTo(array);
        var dest = array.AsSpan(index);
        var ini = index + value.Length;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value, bool caseSensitive)
        => Remove(source, value, caseSensitive, out _);

    internal static StringSpan Remove(
        this StringSpan source, StringSpan value, bool caseSensitive, out bool removed)
    {
        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.IndexOf(value, caseSensitive);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return StringSpan.Empty;

        var array = new char[source.Length - value.Length];
        source[..index].CopyTo(array);
        var dest = array.AsSpan(index);
        var ini = index + value.Length;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => Remove(source, value, comparer, out _);

    internal static StringSpan Remove(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer, out bool removed)
    {
        comparer.ThrowWhenNull();

        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.IndexOf(value, comparer);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return StringSpan.Empty;

        var array = new char[source.Length - value.Length];
        source[..index].CopyTo(array);
        var dest = array.AsSpan(index);
        var ini = index + value.Length;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => Remove(source, value, comparer, out _);

    internal static StringSpan Remove(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer, out bool removed)
    {
        comparer.ThrowWhenNull();

        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.IndexOf(value, comparer);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return StringSpan.Empty;

        var array = new char[source.Length - value.Length];
        source[..index].CopyTo(array);
        var dest = array.AsSpan(index);
        var ini = index + value.Length;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value, StringComparison comparison)
        => Remove(source, value, comparison, out _);

    internal static StringSpan Remove(
        this StringSpan source, StringSpan value, StringComparison comparison, out bool removed)
    {
        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.IndexOf(value, comparison);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return StringSpan.Empty;

        var array = new char[source.Length - value.Length];
        source[..index].CopyTo(array);
        var dest = array.AsSpan(index);
        var ini = index + value.Length;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, StringSpan value)
        => RemoveLast(source, value, out _);

    internal static StringSpan RemoveLast(this StringSpan source, StringSpan value, out bool removed)
    {
        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.LastIndexOf(value);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return StringSpan.Empty;

        var array = new char[source.Length - value.Length];
        source[..index].CopyTo(array);
        var dest = array.AsSpan(index);
        var ini = index + value.Length;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, bool caseSensitive)
        => RemoveLast(source, value, caseSensitive, out _);

    internal static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, bool caseSensitive, out bool removed)
    {
        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.LastIndexOf(value, caseSensitive);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return StringSpan.Empty;

        var array = new char[source.Length - value.Length];
        source[..index].CopyTo(array);
        var dest = array.AsSpan(index);
        var ini = index + value.Length;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => RemoveLast(source, value, comparer, out _);

    internal static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer, out bool removed)
    {
        comparer.ThrowWhenNull();

        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.LastIndexOf(value, comparer);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return StringSpan.Empty;

        var array = new char[source.Length - value.Length];
        source[..index].CopyTo(array);
        var dest = array.AsSpan(index);
        var ini = index + value.Length;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => RemoveLast(source, value, comparer, out _);

    internal static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer, out bool removed)
    {
        comparer.ThrowWhenNull();

        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.LastIndexOf(value, comparer);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return StringSpan.Empty;

        var array = new char[source.Length - value.Length];
        source[..index].CopyTo(array);
        var dest = array.AsSpan(index);
        var ini = index + value.Length;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, StringComparison comparison)
        => RemoveLast(source, value, comparison, out _);

    internal static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, StringComparison comparison, out bool removed)
    {
        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.LastIndexOf(value, comparison);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return StringSpan.Empty;

        var array = new char[source.Length - value.Length];
        source[..index].CopyTo(array);
        var dest = array.AsSpan(index);
        var ini = index + value.Length;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, StringSpan value) => RemoveAll(source, value, out _);

    internal static StringSpan RemoveAll(this StringSpan source, StringSpan value, out bool removed)
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
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, bool caseSensitive)
        => RemoveAll(source, value, caseSensitive, out _);

    internal static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, bool caseSensitive, out bool removed)
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
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => RemoveAll(source, value, comparer, out _);

    internal static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer, out bool removed)
    {
        comparer.ThrowWhenNull();

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
    public static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => RemoveAll(source, value, comparer, out _);

    internal static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer, out bool removed)
    {
        comparer.ThrowWhenNull();

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
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, StringComparison comparison)
        => RemoveAll(source, value, comparison, out _);

    internal static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, StringComparison comparison, out bool removed)
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
}