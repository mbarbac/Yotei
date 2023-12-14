using Yotei.ORM.Records;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a connection with an underlying database.
/// </summary>
public interface IConnection : IBaseDisposable
{
    /// <summary>
    /// The object that describes the underlying database engine.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// Determines if this connection is open or not.
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    /// Opens the connection with the underlying database.
    /// </summary>
    void Open();

    /// <summary>
    /// Opens the connection with the underlying database.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask OpenAsync(CancellationToken token = default);

    /// <summary>
    /// Closes the connection with the underlying database.
    /// </summary>
    void Close();

    /// <summary>
    /// Closes the connection with the underlying database.
    /// </summary>
    /// <param name="token"></param>
    ValueTask CloseAsync(CancellationToken token = default);

    // ----------------------------------------------------

    /// <summary>
    /// Obtains a transaction associated with this instance.
    /// </summary>
    /// <returns></returns>
    ITransaction GetTransaction();

    /// <summary>
    /// Provides access to the record-oriented capabilities of this connection.
    /// </summary>
    IRecordsGate Records { get; }
}