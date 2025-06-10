using IHost = Yotei.ORM.Internals.IStrTokenChain;
using IItem = Yotei.ORM.Internals.IStrToken;

namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a list-alike collection of elements..
/// <para>Instances of this class are intended to be immutable ones.</para>
/// </summary>
[IInvariantList<IItem>]
public partial interface IStrTokenChain : IItem
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

    // ----------------------------------------------------

    /// <summary>
    /// The actual flat collection of tokens carried by this instance.
    /// </summary>
    new IEnumerable<IItem> Payload { get; }

    /// <summary>
    /// Reduces this instance to a simpler form by combining adjacent text elements in this
    /// collection, if possible.
    /// </summary>
    /// <returns></returns>
    IItem ReduceText();
}