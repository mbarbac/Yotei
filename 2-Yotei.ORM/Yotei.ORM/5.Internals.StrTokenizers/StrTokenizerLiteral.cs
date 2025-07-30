namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents the ability of extracting <see cref="StrTokenLiteral"/> tokens.
/// </summary>
public record StrTokenizerLiteral : StrTokenizerText
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public StrTokenizerLiteral(string value) : base(value) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public StrTokenizerLiteral(char value) : base(value) { }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a found token using the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected override IStrToken CreateToken(string value) => new StrTokenLiteral(value);
}