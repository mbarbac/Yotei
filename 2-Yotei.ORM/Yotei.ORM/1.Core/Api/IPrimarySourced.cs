namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an object that is univocally associated with a primary source.
/// </summary>
public interface IPrimarySourced
{
    /// <summary>
    /// The identifier of the primary source this instance is univocally associated with.
    /// </summary>
    IIdentifier PrimarySource { get; }
}