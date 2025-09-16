namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents an arbitrary token in a database expression.
/// </summary>
public interface IDbToken : IEquatable<IDbToken>
{
    /// <inheritdoc cref="ICloneable.Clone"/>
    IDbToken Clone();
}