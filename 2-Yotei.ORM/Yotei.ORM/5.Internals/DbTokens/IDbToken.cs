namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents an arbitrary token in a datase expression.
/// <br/> Instance of this type are intended to be immutable ones.
/// </summary>
/// We need an interface because 'DbTokenChain' inherits from 'InvariantList', and so it cannot
/// be a token itself inheriting also from another class, it being 'DbToken'.
public interface IDbToken : IEquatable<IDbToken>
{
    /// <inheritdoc cref="ICloneable.Clone"/>
    IDbToken Clone();

    /// <summary>
    /// Returns the dynamic argument ultimately associated with this instance, or null if it
    /// cannot be determined.
    /// </summary>
    /// <returns></returns>
    DbTokenArgument? GetArgument();
}