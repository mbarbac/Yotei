namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents the ability of extracting text tokens as <see cref="StrTokenLiteral"/> ones.
/// </summary>
[InheritWiths]
public partial class StrLiteralTokenizer : StrTextTokenizer
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public StrLiteralTokenizer(string value) : base(value) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public StrLiteralTokenizer(char value) : base(value) { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected StrLiteralTokenizer(StrLiteralTokenizer source) : base(source) { }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a token for the found value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected override IStrToken CreateToken(string value) => new StrTokenLiteral(value);
}