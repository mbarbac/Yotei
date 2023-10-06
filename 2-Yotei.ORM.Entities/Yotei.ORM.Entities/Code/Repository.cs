#pragma warning disable CS0436

namespace Yotei.ORM.Entities.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IRepository"/>
/// </summary>
[WithGenerator("this")]
public partial class Repository : DisposableClass, IRepository
{
    /// <summary>
    /// Initializes a new instance with the connection obtained from the given delegate. The life
    /// cycle of the obtained connection instance will be managed by the framework, and shall not
    /// be managed by client code.
    /// </summary>
    /// <param name="connection"></param>
    public Repository(Func<IConnection> connection)
    {
        ArgumentNullException.ThrowIfNull(connection);
        Connection = connection() ?? throw new InvalidOperationException(
            "Cannot obtain a valid connection for this repository.");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { Connection.Dispose(); }
        catch { }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    /// <returns></returns>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { await Connection.DisposeAsync(); }
        catch { }
    }

    /// <summary>
    /// <inheritdoc cref="ICloneable.Clone"/>
    /// </summary>
    /// <returns></returns>
    public virtual Repository Clone() => new(() => Connection.Clone());
    IRepository IRepository.Clone() => Clone();
    IConnection IConnection.Clone() => Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"Repository({Connection})";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IConnection Connection { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine => Connection.Engine;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Retries
    {
        get => Connection.Retries;
        set => Connection.Retries = value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public TimeSpan RetryInterval
    {
        get => Connection.RetryInterval;
        set => Connection.RetryInterval = value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Locale Locale
    {
        get => Connection.Locale;
        set => Connection.Locale = value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsOpen => Connection.IsOpen;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Open() => Connection.Open();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask OpenAsync(CancellationToken token = default) => Connection.OpenAsync(token);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Close() => Connection.Close();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public ValueTask CloseAsync() => Connection.CloseAsync();

    // <summary>
    /// <inheritdoc/>
    /// </summary>
    public ITransaction Transaction => Connection.Transaction;
}