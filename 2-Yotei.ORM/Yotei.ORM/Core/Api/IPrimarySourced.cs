namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an object that is univocally associated with an underlying primary source.
/// </summary>
public interface IPrimarySourced
{
    /// <summary>
    /// The identifier of the primary source this object is associated with.
    /// </summary>
    IIdentifier PrimarySource { get; }
}