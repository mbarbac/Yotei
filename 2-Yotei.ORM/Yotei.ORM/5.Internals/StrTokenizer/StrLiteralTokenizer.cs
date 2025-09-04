namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents the ability of extracting <see cref="StrTokenLiteral"/> tokens from a given source.
/// </summary>
public record StrLiteralTokenizer : StrTextTokenizer
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="target"></param>
    public StrLiteralTokenizer(string target) : base(target) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="target"></param>
    public StrLiteralTokenizer(char target) : base(target) { }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a new token to carry the target that has been found.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected override IStrToken CreateToken(string value) => new StrTokenLiteral(value);
}