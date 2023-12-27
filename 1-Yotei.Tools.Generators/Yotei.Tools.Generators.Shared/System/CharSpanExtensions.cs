using CharSpan = System.ReadOnlySpan<char>;

namespace Yotei.Tools.Generators.Shared;

// ========================================================
internal static class CharSpanExtensions
{
    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static CharSpan Remove(this CharSpan source, CharSpan value) => Remove(source, value, out _);
    internal static CharSpan Remove(this CharSpan source, CharSpan value, out bool removed)
    {
        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.IndexOf(value);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return CharSpan.Empty;

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
    public static CharSpan RemoveLast(this CharSpan source, CharSpan value) => RemoveLast(source, value, out _);
    internal static CharSpan RemoveLast(this CharSpan source, CharSpan value, out bool removed)
    {
        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.LastIndexOf(value);
        if (index < 0) return source;

        removed = true;
        if (index == 0 && source.Length == value.Length) return CharSpan.Empty;

        var array = new char[source.Length - value.Length];
        source[..index].CopyTo(array);
        var dest = array.AsSpan(index);
        var ini = index + value.Length;
        source[ini..].CopyTo(dest);

        return array.AsSpan();
    }
}