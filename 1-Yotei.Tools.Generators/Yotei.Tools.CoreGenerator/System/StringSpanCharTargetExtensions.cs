using StringSpan = System.ReadOnlySpan<char>;

namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class StringSpanCharTargetExtensions
{
    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static int IndexOf(this StringSpan source, char value, bool caseSensitive)
    {
        if (source.Length > 0)
        {
            for (int i = 0; i < source.Length; i++)
            {
                var temp = source[i];
                if (temp.Equals(value, caseSensitive)) return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(this StringSpan source, char value, IEqualityComparer<char> comparer)
    {
        comparer.ThrowWhenNull();

        if (source.Length > 0)
        {
            for (int i = 0; i < source.Length; i++)
            {
                var temp = source[i];
                if (temp.Equals(value, comparer)) return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    /// <exception cref="System.NullReferenceException"></exception>
    public static int IndexOf(this StringSpan source, char value, IEqualityComparer<string> comparer)
    {
        comparer.ThrowWhenNull();

        if (source.Length > 0)
        {
            for (int i = 0; i < source.Length; i++)
            {
                var temp = source[i];
                if (temp.Equals(value, comparer)) return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int IndexOf(this StringSpan source, char value, StringComparison comparison)
    {
        if (source.Length > 0)
        {
            for (int i = 0; i < source.Length; i++)
            {
                var temp = source[i];
                if (temp.Equals(value, comparison)) return i;
            }
        }
        return -1;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static int LastIndexOf(this StringSpan source, char value, bool caseSensitive)
    {
        if (source.Length > 0)
        {
            for (int i = source.Length - 1; i >= 0; i--)
            {
                var temp = source[i];
                if (temp.Equals(value, caseSensitive)) return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(this StringSpan source, char value, IEqualityComparer<char> comparer)
    {
        comparer.ThrowWhenNull();

        if (source.Length > 0)
        {
            for (int i = source.Length - 1; i >= 0; i--)
            {
                var temp = source[i];
                if (temp.Equals(value, comparer)) return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    /// <exception cref="System.NullReferenceException"></exception>
    public static int LastIndexOf(this StringSpan source, char value, IEqualityComparer<string> comparer)
    {
        comparer.ThrowWhenNull();

        if (source.Length > 0)
        {
            for (int i = source.Length - 1; i >= 0; i--)
            {
                var temp = source[i];
                if (temp.Equals(value, comparer)) return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int LastIndexOf(this StringSpan source, char value, StringComparison comparison)
    {
        if (source.Length > 0)
        {
            for (int i = source.Length - 1; i >= 0; i--)
            {
                var temp = source[i];
                if (temp.Equals(value, comparison)) return i;
            }
        }
        return -1;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool Contains(this StringSpan source, char value) => source.IndexOf(value) >= 0;

    /// <summary>
    /// Determines if the source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, char value, bool caseSensitive)
        => source.IndexOf(value, caseSensitive) >= 0;

    /// <summary>
    /// Determines if the source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
        => source.IndexOf(value, comparer) >= 0;

    /// <summary>
    /// Determines if the source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => source.IndexOf(value, comparer) >= 0;

    /// <summary>
    /// Determines if the source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, char value, StringComparison comparison)
        => source.IndexOf(value, comparison) >= 0;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the source contains any of the given characters, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="chars"></param>
    /// <returns></returns>
    public static bool ContainsAny(this StringSpan source, IEnumerable<char> chars)
    {
        chars.ThrowWhenNull();

        foreach (var c in chars) if (source.Contains(c)) return true;
        return false;
    }

    /// <summary>
    /// Determines if the source contains any of the given characters, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="chars"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<char> chars, bool caseSensitive)
    {
        chars.ThrowWhenNull();

        foreach (var c in chars) if (source.Contains(c, caseSensitive)) return true;
        return false;
    }

    /// <summary>
    /// Determines if the source contains any of the given characters, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="chars"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<char> chars, IEqualityComparer<char> comparer)
    {
        chars.ThrowWhenNull();

        foreach (var c in chars) if (source.Contains(c, comparer)) return true;
        return false;
    }

    /// <summary>
    /// Determines if the source contains any of the given characters, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="chars"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<char> chars, IEqualityComparer<string> comparer)
    {
        chars.ThrowWhenNull();

        foreach (var c in chars) if (source.Contains(c, comparer)) return true;
        return false;
    }

    /// <summary>
    /// Determines if the source contains any of the given characters, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="chars"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    /// <exception cref="System.NullReferenceException"></exception>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<char> chars, StringComparison comparison)
    {
        chars.ThrowWhenNull();

        foreach (var c in chars) if (source.Contains(c, comparison)) return true;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, char value) => source.Length > 0 && source[0].Equals(value);

    /// <summary>
    /// Determines if the source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, char value, bool caseSensitive)
        => source.Length > 0 && source[0].Equals(value, caseSensitive);

    /// <summary>
    /// Determines if the source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
        => source.Length > 0 && source[0].Equals(value, comparer);

    /// <summary>
    /// Determines if the source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => source.Length > 0 && source[0].Equals(value, comparer);

    /// <summary>
    /// Determines if the source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, char value, StringComparison comparison)
        => source.Length > 0 && source[0].Equals(value, comparison);

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the source ends with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, char value) => source.Length > 0 && source[^1].Equals(value);

    /// <summary>
    /// Determines if the source ends with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, char value, bool caseSensitive)
        => source.Length > 0 && source[^1].Equals(value, caseSensitive);

    /// <summary>
    /// Determines if the source ends with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
        => source.Length > 0 && source[^1].Equals(value, comparer);

    /// <summary>
    /// Determines if the source ends with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => source.Length > 0 && source[^1].Equals(value, comparer);

    /// <summary>
    /// Determines if the source ends with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, char value, StringComparison comparison)
        => source.Length > 0 && source[^1].Equals(value, comparison);

    // ----------------------------------------------------

    /// <summary>
    /// Returns either a new instance where the first ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, char value) => source.Remove(value, out _);

    public static StringSpan Remove(
        this StringSpan source, char value, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.IndexOf(value); if (index < 0) return source;

        return source.Remove(index, 1, out removed);
    }

    /// <summary>
    /// Returns either a new instance where the first ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, char value, bool caseSensitive)
        => source.Remove(value, caseSensitive, out _);

    public static StringSpan Remove(
        this StringSpan source, char value, bool caseSensitive, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.IndexOf(value, caseSensitive); if (index < 0) return source;

        return source.Remove(index, 1, out removed);
    }

    /// <summary>
    /// Returns either a new instance where the first ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
        => source.Remove(value, comparer, out _);

    public static StringSpan Remove(
        this StringSpan source, char value, IEqualityComparer<char> comparer,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.IndexOf(value, comparer); if (index < 0) return source;

        return source.Remove(index, 1, out removed);
    }

    /// <summary>
    /// Returns either a new instance where the first ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => source.Remove(value, comparer, out _);

    public static StringSpan Remove(
        this StringSpan source, char value, IEqualityComparer<string> comparer,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.IndexOf(value, comparer); if (index < 0) return source;

        return source.Remove(index, 1, out removed);
    }

    /// <summary>
    /// Returns either a new instance where the first ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, char value, StringComparison comparison)
        => source.Remove(value, comparison, out _);

    public static StringSpan Remove(
        this StringSpan source, char value, StringComparison comparison,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.IndexOf(value, comparison); if (index < 0) return source;

        return source.Remove(index, 1, out removed);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns either a new instance where the last ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, char value) => source.RemoveLast(value, out _);

    public static StringSpan RemoveLast(
        this StringSpan source, char value, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.LastIndexOf(value); if (index < 0) return source;

        return source.Remove(index, 1, out removed);
    }

    /// <summary>
    /// Returns either a new instance where the last ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, char value, bool caseSensitive)
        => source.RemoveLast(value, caseSensitive, out _);

    public static StringSpan RemoveLast(
        this StringSpan source, char value, bool caseSensitive, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.LastIndexOf(value, caseSensitive); if (index < 0) return source;

        return source.Remove(index, 1, out removed);
    }

    /// <summary>
    /// Returns either a new instance where the last ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
        => source.RemoveLast(value, comparer, out _);

    public static StringSpan RemoveLast(
        this StringSpan source, char value, IEqualityComparer<char> comparer,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.LastIndexOf(value, comparer); if (index < 0) return source;

        return source.Remove(index, 1, out removed);
    }

    /// <summary>
    /// Returns either a new instance where the last ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => source.RemoveLast(value, comparer, out _);

    public static StringSpan RemoveLast(
        this StringSpan source, char value, IEqualityComparer<string> comparer,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.LastIndexOf(value, comparer); if (index < 0) return source;

        return source.Remove(index, 1, out removed);
    }

    /// <summary>
    /// Returns either a new instance where the last ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, char value, StringComparison comparison)
        => source.RemoveLast(value, comparison, out _);

    public static StringSpan RemoveLast(
        this StringSpan source, char value, StringComparison comparison,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.LastIndexOf(value, comparison); if (index < 0) return source;

        return source.Remove(index, 1, out removed);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns either a new instance where all the ocurrences of the given value have been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, char value) => source.RemoveAll(value, out _);

    public static StringSpan RemoveAll(
        this StringSpan source, char value, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, out var temp); if (!temp) break;
            removed = true;
        }
        return source;
    }

    /// <summary>
    /// Returns either a new instance where all the ocurrences of the given value have been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, char value, bool caseSensitive)
        => source.RemoveAll(value, caseSensitive, out _);

    public static StringSpan RemoveAll(
        this StringSpan source, char value, bool caseSensitive, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, caseSensitive, out var temp); if (!temp) break;
            removed = true;
        }
        return source;
    }

    /// <summary>
    /// Returns either a new instance where all the ocurrences of the given value have been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
        => source.RemoveAll(value, comparer, out _);

    public static StringSpan RemoveAll(
        this StringSpan source, char value, IEqualityComparer<char> comparer,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, comparer, out var temp); if (!temp) break;
            removed = true;
        }
        return source;
    }

    /// <summary>
    /// Returns either a new instance where all the ocurrences of the given value have been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => source.RemoveAll(value, comparer, out _);

    public static StringSpan RemoveAll(
        this StringSpan source, char value, IEqualityComparer<string> comparer,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, comparer, out var temp); if (!temp) break;
            removed = true;
        }
        return source;
    }

    /// <summary>
    /// Returns either a new instance where all the ocurrences of the given value have been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, char value, StringComparison comparison)
        => source.RemoveAll(value, comparison, out _);

    public static StringSpan RemoveAll(
        this StringSpan source, char value, StringComparison comparison,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, comparison, out var temp); if (!temp) break;
            removed = true;
        }
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns either a new instance where the head ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found or did not
    /// appear at the head.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StringSpan RemoveHead(
        this StringSpan source, char value) => source.RemoveHead(value, out _);

    public static StringSpan RemoveHead(
        this StringSpan source, char value, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.IndexOf(value); if (index != 0) return source;

        return source.Remove(index, 1, out removed);
    }

    /// <summary>
    /// Returns either a new instance where the head ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found or did not
    /// appear at the head.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static StringSpan RemoveHead(
        this StringSpan source, char value, bool caseSensitive)
        => source.RemoveHead(value, caseSensitive, out _);

    public static StringSpan RemoveHead(
        this StringSpan source, char value, bool caseSensitive, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.IndexOf(value, caseSensitive); if (index != 0) return source;

        return source.Remove(index, 1, out removed);
    }

    /// <summary>
    /// Returns either a new instance where the head ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found or did not
    /// appear at the head.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveHead(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
        => source.RemoveHead(value, comparer, out _);

    public static StringSpan RemoveHead(
        this StringSpan source, char value, IEqualityComparer<char> comparer,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.IndexOf(value, comparer); if (index != 0) return source;

        return source.Remove(index, 1, out removed);
    }

    /// <summary>
    /// Returns either a new instance where the head ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found or did not
    /// appear at the head.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveHead(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => source.RemoveHead(value, comparer, out _);

    public static StringSpan RemoveHead(
        this StringSpan source, char value, IEqualityComparer<string> comparer,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.IndexOf(value, comparer); if (index != 0) return source;

        return source.Remove(index, 1, out removed);
    }

    /// <summary>
    /// Returns either a new instance where the head ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found or did not
    /// appear at the head.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StringSpan RemoveHead(
        this StringSpan source, char value, StringComparison comparison)
        => source.RemoveHead(value, comparison, out _);

    public static StringSpan RemoveHead(
        this StringSpan source, char value, StringComparison comparison,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = source.IndexOf(value, comparison); if (index != 0) return source;

        return source.Remove(index, 1, out removed);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns either a new instance where the tail ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found or did not
    /// appear at the tail.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StringSpan RemoveTail(
        this StringSpan source, char value) => source.RemoveTail(value, out _);

    public static StringSpan RemoveTail(
        this StringSpan source, char value, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var num = source.Length - 1;
        var index = source.LastIndexOf(value); if (index != num) return source;

        return source.Remove(index, 1, out removed);
    }

    /// <summary>
    /// Returns either a new instance where the tail ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found or did not
    /// appear at the tail.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static StringSpan RemoveTail(
        this StringSpan source, char value, bool caseSensitive)
        => source.RemoveTail(value, caseSensitive, out _);

    public static StringSpan RemoveTail(
        this StringSpan source, char value, bool caseSensitive, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var num = source.Length - 1;
        var index = source.LastIndexOf(value, caseSensitive); if (index != num) return source;

        return source.Remove(index, 1, out removed);
    }

    /// <summary>
    /// Returns either a new instance where the tail ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found or did not
    /// appear at the tail.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveTail(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
        => source.RemoveTail(value, comparer, out _);

    public static StringSpan RemoveTail(
        this StringSpan source, char value, IEqualityComparer<char> comparer,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var num = source.Length - 1;
        var index = source.LastIndexOf(value, comparer); if (index != num) return source;

        return source.Remove(index, 1, out removed);
    }

    /// <summary>
    /// Returns either a new instance where the tail ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found or did not
    /// appear at the tail.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveTail(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => source.RemoveTail(value, comparer, out _);

    public static StringSpan RemoveTail(
        this StringSpan source, char value, IEqualityComparer<string> comparer,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var num = source.Length - 1;
        var index = source.LastIndexOf(value, comparer); if (index != num) return source;

        return source.Remove(index, 1, out removed);
    }

    /// <summary>
    /// Returns either a new instance where the tail ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found or did not
    /// appear at the tail.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StringSpan RemoveTail(
        this StringSpan source, char value, StringComparison comparison)
        => source.RemoveTail(value, comparison, out _);

    public static StringSpan RemoveTail(
        this StringSpan source, char value, StringComparison comparison,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var num = source.Length - 1;
        var index = source.LastIndexOf(value, comparison); if (index != num) return source;

        return source.Remove(index, 1, out removed);
    }
}