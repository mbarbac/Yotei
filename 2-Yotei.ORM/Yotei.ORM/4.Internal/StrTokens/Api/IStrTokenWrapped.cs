namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a token that carries an arbitrary payload between its head and tail sequences,
/// which are trimmed before used.
/// </summary>
public interface IStrTokenWrapped : IStrToken
{
    /// <summary>
    /// The head sequence.
    /// </summary>
    public string Head { get; }

    /// <summary>
    /// The actual payload carried by this instance.
    /// </summary>
    new IStrToken Payload { get; }

    /// <summary>
    /// The tail sequence.
    /// </summary>
    public string Tail { get; }
}