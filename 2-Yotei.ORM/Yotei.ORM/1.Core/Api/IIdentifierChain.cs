using IHost = Yotei.ORM.IIdentifierChain;
using IItem = Yotei.ORM.IIdentifierUnit;
using TKey = string;
using INV = Yotei.ORM.Generators.Invariant;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a multi-part database identifier, including empty or missed ones.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[IInvariantList<INV.IsNullable<TKey>, IItem>(ReturnType = typeof(IHost))]
public partial interface IIdentifierChain : IIdentifier
{
    /// <summary>
    /// Returns a new builder base upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

    // ------------------------------------------------

    /// <summary>
    /// Returns a new instance with the element at the given index replaced by the ones
    /// obtained from the given value.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost Replace(int index, string? value);

    /// <summary>
    /// Returns a new instance with the elements obtained from the given range of values added
    /// to the collection.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost Add(string? value);

    /// <summary>
    /// Returns a new instance with the elements obtained from the given value added to the
    /// collection.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost AddRange(IEnumerable<string?> range);

    /// <summary>
    /// Returns a new instance with the elements obtained from the given value inserted into
    /// the collection at the given index.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost Insert(int index, string? value);

    /// <summary>
    /// Returns a new instance with the elements obtained from the given range of values
    /// inserted into the collection, starting at the given index.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost InsertRange(int index, IEnumerable<string?> range);
}