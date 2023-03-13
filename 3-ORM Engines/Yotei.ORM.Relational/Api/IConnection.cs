namespace Yotei.ORM.Relational;

// ========================================================
/// <summary>
/// Represents a connection context with an underlying relational database.
/// </summary>
public interface IConnection : ORM.IConnection
{
    /// <summary>
    /// <inheritdoc cref="ICloneable.Clone"/>
    /// </summary>
    /// <returns><inheritdoc cref="ICloneable.Clone"/></returns>
    new IConnection Clone();

    /// <summary>
    /// <inheritdoc cref="ORM.IConnection.Engine"/>
    /// </summary>
    new IEngine Engine { get; }

    /// <summary>
    /// The connection string used by this instance, or null if its value is not set yet. The
    /// setter throws an exception if this connection is opened.
    /// </summary>
    string? ConnectionString { get; set; }

    /// <summary>
    /// The server this instance connects to, or null if not yet connected.
    /// </summary>
    string? Server { get; }

    /// <summary>
    /// The database this instance connects to, or null if not yet connected.
    /// </summary>
    string? Database { get; }
}