using IHost = Yotei.ORM.Internal.IStrTokenChain;
using IItem = Yotei.ORM.Internal.IStrToken;

namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents an immutable flat collection of arbitrary tokens.
/// <para>Instances of this class are intended to be immutable ones.</para>
/// </summary>
[IInvariantList<IItem>]
public partial interface IStrTokenChain : IItem
{
    /// <summary>
    /// Gets a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder GetBuilder();

    /// <summary>
    /// The actual flat-collection of tokens payload carried by this instance.
    /// </summary>
    new IEnumerable<IItem> Payload { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Reduces this instance to a simpler form by combining adjacent text elements in this
    /// collection, if possible, and then reducing the resulting chain.
    /// </summary>
    /// <returns></returns>
    IStrToken ReduceText();
}