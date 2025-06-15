namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Provides access to the record-oriented capabilities of the associated connection.
/// </summary>
public interface IRecordsGate
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Factory method to create an object with the ability of parsing db-token chains returning
    /// the <see cref="ICommandInfo"/> object that represents that chain for the underlying
    /// database engine.
    /// </summary>
    /// <param name="locale"></param>
    /// <param name="useNullString"></param>
    /// <param name="captureValues"></param>
    /// <param name="convertValues"></param>
    /// <param name="useQuotes"></param>
    /// <param name="useTerminators"></param>
    /// <param name="rangeSeparator"></param>
    /// <returns></returns>
    DbTokenVisitor CreateDbTokenVisitor(
        Locale? locale = null,
        bool useNullString = DbTokenVisitor.USENULLSTRING,
        bool captureValues = DbTokenVisitor.CAPTUREVALUES,
        bool convertValues = DbTokenVisitor.CONVERTVALUES,
        bool useQuotes = DbTokenVisitor.USEQUOTES,
        bool useTerminators = DbTokenVisitor.USETERMINATORS,
        string? rangeSeparator = DbTokenVisitor.RANGESEPARATOR);
}