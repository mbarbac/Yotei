namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents the ability of extracting text tokens as <see cref="IStrTokenLiteral"/> ones.
/// </summary>
[InheritWiths]
public partial class StrTokenizerLiteral : StrTokenizerText
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

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected StrTokenizerLiteral(StrTokenizerLiteral source) : base(source) { }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a token for the found value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected override IStrToken CreateToken(string value) => new StrTokenLiteral(value);
}