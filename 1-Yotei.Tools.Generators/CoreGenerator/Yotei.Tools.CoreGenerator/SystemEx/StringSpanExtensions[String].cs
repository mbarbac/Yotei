using StringSpan = System.ReadOnlySpan<char>;

namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class StringSpanExtensions_StringTarget
{
    // span.IndexOf(StringSpan value);
    // span.IndexOf(StringSpan value, StringComparison comparison);

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    public static int IndexOf(this StringSpan source, StringSpan value, bool ignoreCase)
    {
        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0 || value.Length == 0) return -1;

        for (int i = 0; i < source.Length; i++)
        {
            if ((i + value.Length) > source.Length) break;

            var found = false; for (int k = 0; i < value.Length; k++)
            {
                var s = source[i + k];
                var v = value[k];
                if (s.Equals(v, ignoreCase)) { found = true; continue; }
                found = false;
                break;
            }
            if (found) return i;
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
    public static int IndexOf(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
    {
        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0 || value.Length == 0) return -1;

        for (int i = 0; i < source.Length; i++)
        {
            if ((i + value.Length) > source.Length) break;

            var found = false; for (int k = 0; i < value.Length; k++)
            {
                var s = source[i + k];
                var v = value[k];
                if (s.Equals(v, comparer)) { found = true; continue; }
                found = false;
                break;
            }
            if (found) return i;
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
    public static int IndexOf(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
    {
        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0 || value.Length == 0) return -1;

        for (int i = 0; i < source.Length; i++)
        {
            if ((i + value.Length) > source.Length) break;

            var found = false; for (int k = 0; i < value.Length; k++)
            {
                var s = source[i + k];
                var v = value[k];
                if (s.Equals(v, comparer)) { found = true; continue; }
                found = false;
                break;
            }
            if (found) return i;
        }
        return -1;
    }

    // ----------------------------------------------------

    // span.LastIndexOf(StringSpan value);

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    public static int LastIndexOf(this StringSpan source, StringSpan value, bool ignoreCase)
    {
        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0 || value.Length == 0) return -1;

        var found = -1;
        for (int i = 0; i < source.Length; i++)
        {
            if ((i + value.Length) > source.Length) break;

            var temp = false; for (int k = 0; i < value.Length; k++)
            {
                var s = source[i + k];
                var v = value[k];
                if (s.Equals(v, ignoreCase)) { temp = true; continue; }
                temp = false;
                break;
            }
            if (temp) found = i;
        }
        return found;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    public static int LastIndexOf(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
    {
        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0 || value.Length == 0) return -1;

        var found = -1;
        for (int i = 0; i < source.Length; i++)
        {
            if ((i + value.Length) > source.Length) break;

            var temp = false; for (int k = 0; i < value.Length; k++)
            {
                var s = source[i + k];
                var v = value[k];
                if (s.Equals(v, comparer)) { temp = true; continue; }
                temp = false;
                break;
            }
            if (temp) found = i;
        }
        return found;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    public static int LastIndexOf(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
    {
        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0 || value.Length == 0) return -1;

        var found = -1;
        for (int i = 0; i < source.Length; i++)
        {
            if ((i + value.Length) > source.Length) break;

            var temp = false; for (int k = 0; i < value.Length; k++)
            {
                var s = source[i + k];
                var v = value[k];
                if (s.Equals(v, comparer)) { temp = true; continue; }
                temp = false;
                break;
            }
            if (temp) found = i;
        }
        return found;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    public static int LastIndexOf(
        this StringSpan source, StringSpan value, StringComparison comparison)
    {
        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0 || value.Length == 0) return -1;

        var found = -1;
        for (int i = 0; i < source.Length; i++)
        {
            if ((i + value.Length) > source.Length) break;

            var temp = false; for (int k = 0; i < value.Length; k++)
            {
                var s = source[i + k];
                var v = value[k];
                if (s.Equals(v, comparison)) { temp = true; continue; }
                temp = false;
                break;
            }
            if (temp) found = i;
        }
        return found;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the indexes of all the ocurrences of the given value in the source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this StringSpan source, StringSpan value)
    {
        if (source.Length == 0 && value.Length == 0) return [0];
        if (source.Length == 0 || value.Length == 0) return [];

        List<int> list = []; for (int i = 0; i < source.Length; i++)
        {
            var temp = source[i..];
            if (temp.StartsWith(value)) list.Add(i);
        }
        return list;
    }

    /// <summary>
    /// Returns the indexes of all the ocurrences of the given value in the source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, StringSpan value, bool ignoreCase)
    {
        if (source.Length == 0 && value.Length == 0) return [0];
        if (source.Length == 0 || value.Length == 0) return [];

        List<int> list = []; for (int i = 0; i < source.Length; i++)
        {
            var temp = source[i..];
            if (temp.StartsWith(value, ignoreCase)) list.Add(i);
        }
        return list;
    }

    /// <summary>
    /// Returns the indexes of all the ocurrences of the given value in the source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
    {
        if (source.Length == 0 && value.Length == 0) return [0];
        if (source.Length == 0 || value.Length == 0) return [];

        List<int> list = []; for (int i = 0; i < source.Length; i++)
        {
            var temp = source[i..];
            if (temp.StartsWith(value, comparer)) list.Add(i);
        }
        return list;
    }

    /// <summary>
    /// Returns the indexes of all the ocurrences of the given value in the source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
    {
        if (source.Length == 0 && value.Length == 0) return [0];
        if (source.Length == 0 || value.Length == 0) return [];

        List<int> list = []; for (int i = 0; i < source.Length; i++)
        {
            var temp = source[i..];
            if (temp.StartsWith(value, comparer)) list.Add(i);
        }
        return list;
    }

    /// <summary>
    /// Returns the indexes of all the ocurrences of the given value in the source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, StringSpan value, StringComparison comparison)
    {
        if (source.Length == 0 && value.Length == 0) return [0];
        if (source.Length == 0 || value.Length == 0) return [];

        List<int> list = []; for (int i = 0; i < source.Length; i++)
        {
            var temp = source[i..];
            if (temp.StartsWith(value, comparison)) list.Add(i);
        }
        return list;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, StringSpan value) => source.IndexOf(value) >= 0;

    /// <summary>
    /// Determines if the source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => source.IndexOf(value, ignoreCase) >= 0;

    /// <summary>
    /// Determines if the source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => source.IndexOf(value, comparer) >= 0;

    /// <summary>
    /// Determines if the source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => source.IndexOf(value, comparer) >= 0;

    /// <summary>
    /// Determines if the source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, StringSpan value, StringComparison comparison)
        => source.IndexOf(value, comparison) >= 0;

    // ----------------------------------------------------

    // span.StartsWith(StringSpan value);
    // span.StartsWith(StringSpan value, StringComparison);

    /// <summary>
    /// Determines if the source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool StartsWith(this StringSpan source, StringSpan value, bool ignoreCase)
    {
        if (source.Length == 0 && value.Length == 0) return true;
        if (source.Length == 0 || value.Length == 0) return false;
        if (value.Length > source.Length) return false;

        for (int i = 0; i < value.Length; i++)
        {
            var s = source[i];
            var v = value[i];
            if (!s.Equals(v)) return false;
        }
        return true;
    }

    /// <summary>
    /// Determines if the source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
    {
        if (source.Length == 0 && value.Length == 0) return true;
        if (source.Length == 0 || value.Length == 0) return false;
        if (value.Length > source.Length) return false;

        for (int i = 0; i < value.Length; i++)
        {
            var s = source[i];
            var v = value[i];
            if (!s.Equals(v, comparer)) return false;
        }
        return true;
    }

    /// <summary>
    /// Determines if the source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
    {
        if (source.Length == 0 && value.Length == 0) return true;
        if (source.Length == 0 || value.Length == 0) return false;
        if (value.Length > source.Length) return false;

        for (int i = 0; i < value.Length; i++)
        {
            var s = source[i];
            var v = value[i];
            if (!s.Equals(v, comparer)) return false;
        }
        return true;
    }

    // ----------------------------------------------------

    // span.EndsWith(StringSpan value);
    // span.EndsWith(StringSpan value, StringComparison);

    /// <summary>
    /// Determines if the source ends with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool EndsWith(this StringSpan source, StringSpan value, bool ignoreCase)
    {
        if (source.Length == 0 && value.Length == 0) return true;
        if (source.Length == 0 || value.Length == 0) return false;

        return source.LastIndexOf(value, ignoreCase) == source.Length - value.Length;
    }

    /// <summary>
    /// Determines if the source ends with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
    {
        if (source.Length == 0 && value.Length == 0) return true;
        if (source.Length == 0) return false;
        if (value.Length == 0) return true;

        return source.LastIndexOf(value, comparer) == source.Length - value.Length;
    }

    /// <summary>
    /// Determines if the source ends with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
    {
        if (source.Length == 0 && value.Length == 0) return true;
        if (source.Length == 0) return false;
        if (value.Length == 0) return true;

        return source.LastIndexOf(value, comparer) == source.Length - value.Length;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns either a new instance where the first ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value) => source.Remove(value, out _);

    /// <summary>
    /// Returns either a new instance where the first ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan Remove(this StringSpan source, StringSpan value, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.IndexOf(value);
        if (index >= 0) { source = source.Slice(index, value.Length); removed = true; }
        return source;
    }

    /// <summary>
    /// Returns either a new instance where the first ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => source.Remove(value, ignoreCase, out _);

    /// <summary>
    /// Returns either a new instance where the first ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value, bool ignoreCase, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.IndexOf(value, ignoreCase);
        if (index >= 0) { source = source.Slice(index, value.Length); removed = true; }
        return source;
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
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => source.Remove(value, comparer, out _);

    /// <summary>
    /// Returns either a new instance where the first ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.IndexOf(value, comparer);
        if (index >= 0) { source = source.Slice(index, value.Length); removed = true; }
        return source;
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
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => source.Remove(value, comparer, out _);

    /// <summary>
    /// Returns either a new instance where the first ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.IndexOf(value, comparer);
        if (index >= 0) { source = source.Slice(index, value.Length); removed = true; }
        return source;
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
        this StringSpan source, StringSpan value, StringComparison comparison)
        => source.Remove(value, comparison, out _);

    /// <summary>
    /// Returns either a new instance where the first ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value, StringComparison comparison, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.IndexOf(value, comparison);
        if (index >= 0) { source = source.Slice(index, value.Length); removed = true; }
        return source;
    }

    //static void EXAMPLE() { "hh".AsSpan().Remove; }
}
/*

    // ----------------------------------------------------

    /// <summary>
    /// Returns either a new instance where the first ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value) => source.Remove(value, out _);

    public static StringSpan Remove(
        this StringSpan source, StringSpan value, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.IndexOf(value); if (index < 0) return source;

        return source.Remove(index, value.Length, out removed);
    }

    /// <summary>
    /// Returns either a new instance where the first ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => source.Remove(value, ignoreCase, out _);

    public static StringSpan Remove(
        this StringSpan source, StringSpan value, bool ignoreCase, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.IndexOf(value, ignoreCase); if (index < 0) return source;

        return source.Remove(index, value.Length, out removed);
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
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => source.Remove(value, comparer, out _);

    public static StringSpan Remove(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.IndexOf(value, comparer); if (index < 0) return source;

        return source.Remove(index, value.Length, out removed);
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
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => source.Remove(value, comparer, out _);

    public static StringSpan Remove(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.IndexOf(value, comparer); if (index < 0) return source;

        return source.Remove(index, value.Length, out removed);
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
        this StringSpan source, StringSpan value, StringComparison comparison)
        => source.Remove(value, comparison, out _);

    public static StringSpan Remove(
        this StringSpan source, StringSpan value, StringComparison comparison,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.IndexOf(value, comparison); if (index < 0) return source;

        return source.Remove(index, value.Length, out removed);
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
        this StringSpan source, StringSpan value) => source.RemoveLast(value, out _);

    public static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.LastIndexOf(value); if (index < 0) return source;

        return source.Remove(index, value.Length, out removed);
    }

    /// <summary>
    /// Returns either a new instance where the last ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => source.RemoveLast(value, ignoreCase, out _);

    public static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, bool ignoreCase, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.LastIndexOf(value, ignoreCase); if (index < 0) return source;

        return source.Remove(index, value.Length, out removed);
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
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => source.RemoveLast(value, comparer, out _);

    public static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.LastIndexOf(value, comparer); if (index < 0) return source;

        return source.Remove(index, value.Length, out removed);
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
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => source.RemoveLast(value, comparer, out _);

    public static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.LastIndexOf(value, comparer); if (index < 0) return source;

        return source.Remove(index, value.Length, out removed);
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
        this StringSpan source, StringSpan value, StringComparison comparison)
        => source.RemoveLast(value, comparison, out _);

    public static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, StringComparison comparison,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.LastIndexOf(value, comparison); if (index < 0) return source;

        return source.Remove(index, value.Length, out removed);
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
        this StringSpan source, StringSpan value) => source.RemoveAll(value, out _);

    public static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

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
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => source.RemoveAll(value, ignoreCase, out _);

    public static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, bool ignoreCase, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, ignoreCase, out var temp); if (!temp) break;
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
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => source.RemoveAll(value, comparer, out _);

    public static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

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
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => source.RemoveAll(value, comparer, out _);

    public static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

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
        this StringSpan source, StringSpan value, StringComparison comparison)
        => source.RemoveAll(value, comparison, out _);

    public static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, StringComparison comparison,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

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
        this StringSpan source, StringSpan value) => source.RemoveHead(value, out _);

    public static StringSpan RemoveHead(
        this StringSpan source, StringSpan value, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.IndexOf(value); if (index != 0) return source;

        return source.Remove(index, value.Length, out removed);
    }

    /// <summary>
    /// Returns either a new instance where the head ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found or did not
    /// appear at the head.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static StringSpan RemoveHead(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => source.RemoveHead(value, ignoreCase, out _);

    public static StringSpan RemoveHead(
        this StringSpan source, StringSpan value, bool ignoreCase, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.IndexOf(value, ignoreCase); if (index != 0) return source;

        return source.Remove(index, value.Length, out removed);
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
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => source.RemoveHead(value, comparer, out _);

    public static StringSpan RemoveHead(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.IndexOf(value, comparer); if (index != 0) return source;

        return source.Remove(index, value.Length, out removed);
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
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => source.RemoveHead(value, comparer, out _);

    public static StringSpan RemoveHead(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.IndexOf(value, comparer); if (index != 0) return source;

        return source.Remove(index, value.Length, out removed);
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
        this StringSpan source, StringSpan value, StringComparison comparison)
        => source.RemoveHead(value, comparison, out _);

    public static StringSpan RemoveHead(
        this StringSpan source, StringSpan value, StringComparison comparison,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var index = source.IndexOf(value, comparison); if (index != 0) return source;

        return source.Remove(index, value.Length, out removed);
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
        this StringSpan source, StringSpan value) => source.RemoveTail(value, out _);

    public static StringSpan RemoveTail(
        this StringSpan source, StringSpan value, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var num = source.Length - value.Length;
        var index = source.LastIndexOf(value); if (index != num) return source;

        return source.Remove(index, value.Length, out removed);
    }

    /// <summary>
    /// Returns either a new instance where the tail ocurrence of the given value has been
    /// removed from the source, or the source itself if the value was not found or did not
    /// appear at the tail.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static StringSpan RemoveTail(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => source.RemoveTail(value, ignoreCase, out _);

    public static StringSpan RemoveTail(
        this StringSpan source, StringSpan value, bool ignoreCase, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var num = source.Length - value.Length;
        var index = source.LastIndexOf(value, ignoreCase); if (index != num) return source;

        return source.Remove(index, value.Length, out removed);
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
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => source.RemoveTail(value, comparer, out _);

    public static StringSpan RemoveTail(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var num = source.Length - value.Length;
        var index = source.LastIndexOf(value, comparer); if (index != num) return source;

        return source.Remove(index, value.Length, out removed);
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
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => source.RemoveTail(value, comparer, out _);

    public static StringSpan RemoveTail(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var num = source.Length - value.Length;
        var index = source.LastIndexOf(value, comparer); if (index != num) return source;

        return source.Remove(index, value.Length, out removed);
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
        this StringSpan source, StringSpan value, StringComparison comparison)
        => source.RemoveTail(value, comparison, out _);

    public static StringSpan RemoveTail(
        this StringSpan source, StringSpan value, StringComparison comparison,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;

        var num = source.Length - value.Length;
        var index = source.LastIndexOf(value, comparison); if (index != num) return source;

        return source.Remove(index, value.Length, out removed);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the source starts with the given value as if the source contains no head
    /// spaces. If so, returns the actual index at with the value appears after those spaces. If
    /// not, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int HeadIndex(this StringSpan source, StringSpan value)
    {
        var index = source.IndexOf(value);
        if (index >= 0)
        {
            for (int i = 0; i < index; i++) if (source[i] != ' ') return -1;
            return index;
        }
        return -1;
    }

    /// <summary>
    /// Determines if the source starts with the given value as if the source contains no head
    /// spaces. If so, returns the actual index at with the value appears after those spaces. If
    /// not, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static int HeadIndex(this StringSpan source, StringSpan value, bool ignoreCase)
    {
        var index = source.IndexOf(value, ignoreCase);
        if (index >= 0)
        {
            for (int i = 0; i < index; i++) if (source[i] != ' ') return -1;
            return index;
        }
        return -1;
    }

    /// <summary>
    /// Determines if the source starts with the given value as if the source contains no head
    /// spaces. If so, returns the actual index at with the value appears after those spaces. If
    /// not, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int HeadIndex(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
    {
        var index = source.IndexOf(value, comparer);
        if (index >= 0)
        {
            for (int i = 0; i < index; i++) if (source[i] != ' ') return -1;
            return index;
        }
        return -1;
    }

    /// <summary>
    /// Determines if the source starts with the given value as if the source contains no head
    /// spaces. If so, returns the actual index at with the value appears after those spaces. If
    /// not, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int HeadIndex(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
    {
        var index = source.IndexOf(value, comparer);
        if (index >= 0)
        {
            for (int i = 0; i < index; i++) if (source[i] != ' ') return -1;
            return index;
        }
        return -1;
    }

    /// <summary>
    /// Determines if the source starts with the given value as if the source contains no head
    /// spaces. If so, returns the actual index at with the value appears after those spaces. If
    /// not, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int HeadIndex(
        this StringSpan source, StringSpan value, StringComparison comparison)
    {
        var index = source.IndexOf(value, comparison);
        if (index >= 0)
        {
            for (int i = 0; i < index; i++) if (source[i] != ' ') return -1;
            return index;
        }
        return -1;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the source ends with the given value as if the source contains no tail
    /// spaces. If so, returns the actual index at with the value appears before those spaces.
    /// If not, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int TailIndex(this StringSpan source, StringSpan value)
    {
        var index = source.LastIndexOf(value);
        if (index >= 0)
        {
            var max = index + value.Length;
            for (int i = source.Length - 1; i > max; i--) if (source[i] != ' ') return -1;
            return index;
        }
        return -1;
    }

    /// <summary>
    /// Determines if the source ends with the given value as if the source contains no tail
    /// spaces. If so, returns the actual index at with the value appears before those spaces.
    /// If not, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static int TailIndex(this StringSpan source, StringSpan value, bool ignoreCase)
    {
        var index = source.LastIndexOf(value, ignoreCase);
        if (index >= 0)
        {
            var max = index + value.Length;
            for (int i = source.Length - 1; i > max; i--) if (source[i] != ' ') return -1;
            return index;
        }
        return -1;
    }

    /// <summary>
    /// Determines if the source ends with the given value as if the source contains no tail
    /// spaces. If so, returns the actual index at with the value appears before those spaces.
    /// If not, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int TailIndex(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
    {
        var index = source.LastIndexOf(value, comparer);
        if (index >= 0)
        {
            var max = index + value.Length;
            for (int i = source.Length - 1; i > max; i--) if (source[i] != ' ') return -1;
            return index;
        }
        return -1;
    }

    /// <summary>
    /// Determines if the source ends with the given value as if the source contains no tail
    /// spaces. If so, returns the actual index at with the value appears before those spaces.
    /// If not, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int TailIndex(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
    {
        var index = source.LastIndexOf(value, comparer);
        if (index >= 0)
        {
            var max = index + value.Length;
            for (int i = source.Length - 1; i > max; i--) if (source[i] != ' ') return -1;
            return index;
        }
        return -1;
    }

    /// <summary>
    /// Determines if the source ends with the given value as if the source contains no tail
    /// spaces. If so, returns the actual index at with the value appears before those spaces.
    /// If not, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int TailIndex(
        this StringSpan source, StringSpan value, StringComparison comparison)
    {
        var index = source.LastIndexOf(value, comparison);
        if (index >= 0)
        {
            var max = index + value.Length;
            for (int i = source.Length - 1; i > max; i--) if (source[i] != ' ') return -1;
            return index;
        }
        return -1;
    }
}*/