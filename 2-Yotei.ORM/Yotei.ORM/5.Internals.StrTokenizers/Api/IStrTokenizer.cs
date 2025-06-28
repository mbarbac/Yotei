namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents the ability of extracting string tokens.
/// </summary>
public partial interface IStrTokenizer
{
    /// <summary>
    /// The comparison to use when comparing two string sequences.
    /// </summary>
    [With] StringComparison Comparison { get; }

    /// <summary>
    /// Tokenizes the given source according to the rules of this instance, and optionally
    /// reducing the result if possible and requested.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    IStrToken Tokenize(string source, bool reduce = true);

    /// <summary>
    /// Tokenizes the given source according to the rules of this instance, and optionally
    /// reducing the result if possible and requested.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    IStrToken Tokenize(IStrToken source, bool reduce = true);
}