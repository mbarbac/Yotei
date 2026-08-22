namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents an arbitrary token in a database expression.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public partial interface IDbToken : IEquatable<IDbToken>
{
    /// <summary>
    /// Returns the dynamic argument this instance is ultimately associated with, or null if it
    /// cannot be determined.
    /// </summary>
    /// <returns></returns>
    DbTokenArgument? GetArgument();
}