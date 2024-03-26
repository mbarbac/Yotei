namespace Yotei.ORM.Records.Internal;

// ========================================================
/// <summary>
/// Represents a coalesce operation (left ?? right).
/// </summary>
public sealed class TokenCoalesce : Token
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    public TokenCoalesce(Token left, Token right)
    {
        Left = left.ThrowWhenNull();
        Right = right.ThrowWhenNull();
    }

    /// <inheritdoc/>
    public override string ToString() => $"({Left} ?? {Right})";

    /// <inheritdoc/>
    public override TokenArgument? GetArgument() =>
        Left.GetArgument() ??
        Right.GetArgument();

    /// <summary>
    /// The left part of the coalesce operation.
    /// </summary>
    public Token Left { get; }

    /// <summary>
    /// The right part of the coalesce operation.
    /// </summary>
    public Token Right { get; }
}