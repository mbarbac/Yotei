namespace Yotei.ORM.Records.Internal;

// ========================================================
/// <summary>
/// Represents an arbitrary token in a database expression.
/// </summary>
public abstract class Token
{
    /// <summary>
    /// Returns the dynamic argument ultimately associated with this instance, or null if it
    /// cannot be determined.
    /// </summary>
    /// <returns></returns>
    public abstract TokenArgument? GetArgument();
}