namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents an arbitrary token in a database expression.
/// </summary>
[Cloneable]
public partial interface IDbToken : IEquatable<IDbToken>
{
    /// <summary>
    /// Returns the dynamic argument ultimately associated with this instance, or <c>null</c>
    /// if it cannot be determined.
    /// </summary>
    /// <returns></returns>
    DbTokenArgument? GetArgument();
}