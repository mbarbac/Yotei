namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an object univocally associated with a primary source.
/// </summary>
public interface IPrimarySourced
{
    /// <summary>
    /// The identifier of the primary source this instance is univocally associated with.
    /// </summary>
    IIdentifier PrimarySource { get; }
}