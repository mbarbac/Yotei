namespace Yotei.Tools;

// ========================================================
public static class StringSplitterExtensions
{
    /// <summary>
    /// Returns a new splitter for the given source string and separators.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separators"></param>
    /// <returns></returns>
    public static StringSplitter Splitter(this string source, params IEnumerable<string> separators)
    {
        return new StringSplitter(source, separators);
    }

    /// <summary>
    /// Returns a new splitter for the given source string and separators.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separators"></param>
    /// <returns></returns>
    public static StringSplitter Splitter(this string source, params IEnumerable<char> separators)
    {
        return new StringSplitter(source, separators);
    }
}