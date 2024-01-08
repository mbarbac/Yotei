namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IEnumerableCommand"/>
/// </summary>
[WithGenerator]
public abstract partial class EnumerableCommand : ORM.Code.Command, IEnumerableCommand
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    [SuppressMessage("", "IDE0290")]
    public EnumerableCommand(IConnection connection) : base(connection) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ICommandEnumerator GetEnumerator() => Connection.Records.CommandEnumerator(this);
    IEnumerator<IRecord?> IEnumerable<IRecord?>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public virtual ICommandEnumerator GetAsyncEnumerator(CancellationToken token = default) => Connection.Records.CommandEnumerator(this, token);
    IAsyncEnumerator<IRecord?> IAsyncEnumerable<IRecord?>.GetAsyncEnumerator(
        CancellationToken token)
        => GetAsyncEnumerator(token);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract bool NativePaging { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Skip { get; set; } = -1;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Take { get; set; } = -1;
}