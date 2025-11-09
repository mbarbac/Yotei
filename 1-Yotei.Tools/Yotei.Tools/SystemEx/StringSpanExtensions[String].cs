namespace Yotei.Tools;

// ========================================================
public static class StringSpanExtensions_StringTarget
{
    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static int IndexOf(this StringSpan source, StringSpan value, bool ignoreCase)
    {
        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0) return -1;
        if (value.Length == 0) return 0;

        for (int i = 0; i < source.Length; i++)
        {
            var found = false;
            for (int k = 0; k < value.Length; k++)
            {
                if ((i + k) >= source.Length) break;

                var s = source[i + k];
                var v = value[k];
                if (s.Equals(v, ignoreCase)) found = true;
                else { found = false; break; }
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
            var found = false;
            for (int k = 0; k < value.Length; k++)
            {
                if ((i + k) >= source.Length) break;

                var s = source[i + k];
                var v = value[k];
                if (s.Equals(v, comparer)) found = true;
                else { found = false; break; }
            }
            if (found) return i;
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
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static int LastIndexOf(this StringSpan source, StringSpan value, bool ignoreCase)
    {
        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0) return -1;
        if (value.Length == 0) return 0;

        for (int i = source.Length - 1; i >= 0; i--)
        {
            var found = false;
            for (int k = 0; k < value.Length; k++)
            {
                if ((i + k) >= source.Length) break;

                var s = source[i + k];
                var v = value[k];
                if (s.Equals(v, ignoreCase)) found = true;
                else { found = false; break; }
            }
            if (found) return i;
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
    /// HACK: Solves that StrinSpan.Equals(SpanTarget, CharComparer) doesn't invoke the comparer.
    public static int LastIndexOf(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
    {
        comparer.ThrowWhenNull();

        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0) return -1;
        if (value.Length == 0) return 0;

        for (int i = source.Length - 1; i >= 0; i--)
        {
            var found = false;
            for (int k = 0; k < value.Length; k++)
            {
                if ((i + k) >= source.Length) break;

                var s = source[i + k];
                var v = value[k];
                if (s.Equals(v, comparer)) found = true;
                else { found = false; break; }
            }
            if (found) return i;
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
    public static int LastIndexOf(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
    {
        comparer.ThrowWhenNull();

        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0) return -1;
        if (value.Length == 0) return 0;

        for (int i = source.Length - 1; i >= 0; i--)
        {
            var found = false;
            for (int k = 0; k < value.Length; k++)
            {
                if ((i + k) >= source.Length) break;

                var s = source[i + k];
                var v = value[k];
                if (s.Equals(v, comparer)) found = true;
                else { found = false; break; }
            }
            if (found) return i;
        }
        return -1;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the indexes of all the ocurrences of the given value in the source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<int> AllIndexesOf(this StringSpan source, StringSpan value)
    {
        if (source.Length == 0 && value.Length == 0) return [0];
        if (source.Length == 0) return [];
        if (value.Length == 0) return [0];

        List<int> list = [];

        for (int i = 0; i < source.Length; i++)
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
    public static List<int> AllIndexesOf(this StringSpan source, StringSpan value, bool ignoreCase)
    {
        if (source.Length == 0 && value.Length == 0) return [0];
        if (source.Length == 0) return [];
        if (value.Length == 0) return [0];

        List<int> list = [];

        for (int i = 0; i < source.Length; i++)
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
    public static List<int> AllIndexesOf(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
    {
        if (source.Length == 0 && value.Length == 0) return [0];
        if (source.Length == 0) return [];
        if (value.Length == 0) return [0];

        List<int> list = [];

        for (int i = 0; i < source.Length; i++)
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
    public static List<int> AllIndexesOf(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
    {
        if (source.Length == 0 && value.Length == 0) return [0];
        if (source.Length == 0) return [];
        if (value.Length == 0) return [0];

        List<int> list = [];

        for (int i = 0; i < source.Length; i++)
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
    public static List<int> AllIndexesOf(
        this StringSpan source, StringSpan value, StringComparison comparison)
    {
        if (source.Length == 0 && value.Length == 0) return [0];
        if (source.Length == 0) return [];
        if (value.Length == 0) return [0];

        List<int> list = [];

        for (int i = 0; i < source.Length; i++)
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

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => source.IndexOf(value, ignoreCase) == 0;

    /// <summary>
    /// Determines if the source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => source.IndexOf(value, comparer) == 0;

    // ----------------------------------------------------

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
        if (source.Length == 0) return false;
        if (value.Length == 0) return true;

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
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
    {
        if (source.Length == 0 && value.Length == 0) return true;
        if (source.Length == 0) return false;
        if (value.Length == 0) return true;

        return source.LastIndexOf(value, comparer) == source.Length - value.Length;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns either a new instance where all the characters from the given index to the end
    /// have been removed, or the original one if no characters are removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static StringSpan Remove(this StringSpan source, int index)
    {
        if (index < 0) throw new ArgumentException("Index cannot be negative").WithData(index);

        if (index >= source.Length &&
            index != 0)
            throw new ArgumentException("Index is too big.").WithData(index).WithData(source.ToString());

        var count = source.Length - index;
        return source.Remove(index, count);
    }

    /// <summary>
    /// Returns either a new instance where the given number of characters, starting from the
    /// given index, have been removed, or the original one if no characters are removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, int index, int count) => source.Remove(index, count, out _);

    public static StringSpan Remove(this StringSpan source, int index, int count, out bool removed)
    {
        if (index < 0) throw new ArgumentException("Index cannot be negative").WithData(index);
        if (count < 0) throw new ArgumentException("Count cannot be negative").WithData(index);

        removed = false;
        if (index == 0 && count == source.Length) return [];
        if (index >= source.Length &&
            index != 0) throw new ArgumentException("Index is too big.").WithData(index).WithData(source.ToString());
        if ((index + count) > source.Length) throw new ArgumentException("Index + Count is too big.").WithData(index).WithData(count).WithData(source.ToString());

        if (source.Length == 0) return source;
        if (count == 0) return source;

        removed = true;
        StringSpan temp;
        var array = new char[source.Length - count];
        temp = source[..index]; temp.CopyTo(array);

        var dest = array.AsSpan(index);
        temp = source[(index + count)..]; temp.CopyTo(dest);
        return array.AsSpan();
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
}