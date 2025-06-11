using IHost = Yotei.ORM.IIdentifierChain;
using IItem = Yotei.ORM.IIdentifierPart;
using TKey = string;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a multi-part identifier.
/// <para>Instances of this class are intended to be immutable ones.</para>
/// </summary>
[IInvariantList<AsNullable<TKey>, IItem>] // AsNullable<TKey> to mimic 'TKey?'...
public partial interface IIdentifierChain : IIdentifier
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();
    
    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the element at the given index has been replaced by the
    /// ones obtained from the given value.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost Replace(int index, string? value);

    /// <summary>
    /// Returns a new instance where where the elements obtained from the given value have been
    /// added to the original collection.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost Add(string? value);

    /// <summary>
    /// Returns a new instance where where the elements obtained from the given range of values
    /// have been added to the original collection.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost AddRange(IEnumerable<string?> range);

    /// <summary>
    /// Returns a new instance where where the elements obtained from the given value have been
    /// inserted into the original collection, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost Insert(int index, string? value);

    /// <summary>
    /// Returns a new instance where where the elements obtained from the given range of values
    /// have been inserted into the original collection, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost InsertRange(int index, IEnumerable<string?> range);
}