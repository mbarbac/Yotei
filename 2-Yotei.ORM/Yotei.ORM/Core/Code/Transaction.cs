namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ITransaction"/>
/// </summary>
public abstract class Transaction : DisposableClass, ITransaction
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    [SuppressMessage("", "IDE0290")]
    public Transaction(IConnection connection)
    {
        Connection = connection.ThrowWhenNull();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    protected override void OnDispose(bool disposing)
    {
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    /// <returns></returns>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"ORM.Transaction({Connection})";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IConnection Connection { get; }
}