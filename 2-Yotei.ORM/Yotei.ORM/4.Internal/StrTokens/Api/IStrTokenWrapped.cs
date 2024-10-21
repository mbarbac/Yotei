namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a token that carries an arbitrary payload wrapped between the given head and tail
/// sequences.
/// </summary>
public interface IStrTokenWrapped : IStrToken
{
    /// <summary>
    /// The head sequence.
    /// </summary>
    string Head { get; }

    /// <summary>
    /// The arbitrary payload carried by this instance.
    /// </summary>
    new IStrToken Payload { get; }

    /// <summary>
    /// The tail sequence.
    /// </summary>
    string Tail { get; }
}