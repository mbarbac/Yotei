namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a token that carries an arbitrary payload between its head and tail sequences.
/// <para>Instances of this class are intended to be immutable ones.</para>
/// </summary>
public interface IStrTokenWrapped : IStrToken
{
    /// <summary>
    /// The head sequence.
    /// </summary>
    string Head { get; }

    /// <summary>
    /// The actual payload carried by this instance.
    /// </summary>
    new IStrToken Payload { get; }

    /// <summary>
    /// The tail sequence.
    /// </summary>
    string Tail { get; }
}