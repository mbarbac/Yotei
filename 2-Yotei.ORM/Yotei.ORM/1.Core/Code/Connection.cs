namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IConnection"/>
/// </summary>
public abstract class Connection : DisposableClass, IConnection
{
    public const int RETRIES = 4;
    public const int RETRYINTERVALMS = 250;

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public Connection(IEngine engine)
    {
        Engine = engine.ThrowWhenNull();
        Retries = RETRIES;
        RetryInterval = TimeSpan.FromMilliseconds(RETRYINTERVALMS);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    protected override void OnDispose(bool disposing)
    {
        throw null;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    /// <returns></returns>
    protected override ValueTask OnDisposeAsync(bool disposing)
    {
        throw null;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Retries
    {
        get;
        set => field = value >= 0 ? value :
            throw new ArgumentException("Number of retries must be greater than zero.")
            .WithData(value);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public TimeSpan RetryInterval
    {
        get;
        set => field = value.Ticks >= 0 ? value :
            throw new ArgumentException("Retry interval must be equal or greater than zero.")
            .WithData(value);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract bool IsOpen { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract void Open();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public abstract ValueTask OpenAsync(CancellationToken token = default);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract void Close();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public abstract ValueTask CloseAsync();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ITransaction Transaction { get; } = null!; // HIGH: Connection's transaction.
}