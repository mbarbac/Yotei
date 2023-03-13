namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an underlying database engine.
/// </summary>
public interface IEngine
{
    /// <summary>
    /// Whether the names of database identifiers and similar elements are case sensitive or not.
    /// </summary>
    bool CaseSensitiveNames { get; }

    /// <summary>
    /// The literal that represents null values in the text of database commands.
    /// </summary>
    string NullValueString { get; }

    /// <summary>
    /// Whether the database engine supports native skip-take capabilities, or rather they need
    /// to be emulated.
    /// </summary>
    bool SupportsNativeSkipTake { get; }

    /// <summary>
    /// The literal used to prefix the names of database command parameters, when needed.
    /// </summary>
    string ParameterPrefix { get; }

    /// <summary>
    /// Whether the database engine expects its identifiers wrapped by terminator characters,
    /// or not.
    /// </summary>
    bool UseTerminators { get; }

    /// <summary>
    /// The left character terminator for database identifiers, if terminators are used.
    /// </summary>
    char LeftTerminator { get; }

    /// <summary>
    /// The right character terminator for database identifiers, if terminators are used.
    /// </summary>
    char RightTerminator { get; }
}