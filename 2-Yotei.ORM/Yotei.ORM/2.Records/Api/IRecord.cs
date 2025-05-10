namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the ordered collection of related values to be retrieved or to be persisted into
/// an underlying database.
/// </summary>
[Cloneable]
public partial interface IRecord : IEnumerable<object?>, IEquatable<IRecord>
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder GetBuilder();
}