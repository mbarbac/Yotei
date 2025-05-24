namespace Yotei.Tools;

// ========================================================
public static class StringWrapExtensions
{
    /// <summary>
    /// Wraps the given string with the given head and tail characters, provided that the source
    /// is not null after trimming, if such is requested, and that it was not wrapped already.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    public static string? Wrap(this string? source, char head, char tail, bool trim = true)
    {
        if (head <= 0) throw new ArgumentException("Invalid head.").WithData(head);
        if (tail <= 0) throw new ArgumentException("Invalid tail.").WithData(tail);

        if (source != null)
        {
            if (trim) source = source.Trim();

            if (source.Length > 0)
            {
                if (source[0] != head &&
                    source[^1] != tail)
                    source = $"{head}{source}{tail}";
            }
        }

        return source;
    }

    /// <summary>
    /// Wraps the given string with the given head and tail character, provided that the source
    /// is not null after trimming, if such is requested, and that it was not wrapped already.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="ch"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    public static string? Wrap(
        this string? source, char ch, bool trim = true) => source.Wrap(ch, ch, trim);

    /// <summary>
    /// Removes from the given string the given head and tail characters, provided that they are
    /// paired, that the source is not null after trimming, if such is requested. Unwrapping is
    /// performed recursively unless requested otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    /// <param name="trim"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    public static string? UnWrap(
        this string? source, char head, char tail, bool trim = true, bool recursive = true)
    {
        if (head <= 0) throw new ArgumentException("Invalid head.").WithData(head);
        if (tail <= 0) throw new ArgumentException("Invalid tail.").WithData(tail);

        if (source != null)
        {
            var span = source.AsSpan();

            while (true)
            {
                if (trim) span = span.Trim();
                if (span.Length < 2) break;
                if (span[0] != head || span[^1] != tail) break;

                span = span[1..^1];
                if (!recursive) break;
            }

            source = span.ToString();
        }

        return source;
    }

    /// <summary>
    /// Removes from the given string the given head and tail characters, provided that they are
    /// paired, that the source is not null after trimming, if such is requested. Unwrapping is
    /// performed recursively unless requested otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="ch"></param>
    /// <param name="trim"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    public static string? UnWrap(
        this string? source, char ch, bool trim = true, bool recursive = true)
        => source.UnWrap(ch, ch, trim, recursive);
}