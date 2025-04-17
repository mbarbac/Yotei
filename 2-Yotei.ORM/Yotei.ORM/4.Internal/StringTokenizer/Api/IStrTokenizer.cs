namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents the ability of extracting string tokens.
/// </summary>
public partial interface IStrTokenizer
{
    /// <summary>
    /// The comparison to use when comparing string sequences.
    /// </summary>
    [With] StringComparison Comparison { get; }

    /// <summary>
    /// Tokenizes the given source string, by default reducing the resulting token if possible.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    IStrToken Tokenize(string source, bool reduce = true);

    /// <summary>
    /// Tokenizes the payload of the given token, by default reducing the resulting token if
    /// possible.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    IStrToken Tokenize(IStrToken token, bool reduce = true);
}