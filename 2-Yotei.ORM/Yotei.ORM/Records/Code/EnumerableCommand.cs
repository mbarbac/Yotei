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
    public abstract ICommandEnumerator GetEnumerator();
    IEnumerator<IRecord?> IEnumerable<IRecord?>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public abstract ICommandEnumerator GetAsyncEnumerator(CancellationToken token = default);
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