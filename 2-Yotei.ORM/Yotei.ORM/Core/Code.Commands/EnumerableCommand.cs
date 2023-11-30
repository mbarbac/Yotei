namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IEnumerableCommand{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class EnumerableCommand<T> : Command, IEnumerableCommand<T>
    where T : class
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    public EnumerableCommand(IConnection connection) : base(connection) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public abstract ICommandEnumerator<T> GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public abstract ICommandEnumerator<T> GetAsyncEnumerator(CancellationToken token = default);
    IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(
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
    public int Skip { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Take { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<T> ToList()
    {
        using var iter = GetEnumerator();
        return iter.ToList();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask<List<T>> ToListAsync(CancellationToken token = default)
    {
        using var iter = GetAsyncEnumerator(token);
        return await iter.ToListAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public T[] ToArray()
    {
        using var iter = GetEnumerator();
        return iter.ToArray();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask<T[]> ToArrayAsync(CancellationToken token = default)
    {
        using var iter = GetAsyncEnumerator(token);
        return await iter.ToArrayAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public T? First()
    {
        using var iter = GetEnumerator();
        return iter.First();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask<T?> FirstAsync(CancellationToken token = default)
    {
        using var iter = GetAsyncEnumerator(token);
        return await iter.FirstAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public T? Last()
    {
        using var iter = GetEnumerator();
        return iter.Last();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask<T?> LastAsync(CancellationToken token = default)
    {
        using var iter = GetAsyncEnumerator(token);
        return await iter.LastAsync().ConfigureAwait(false);
    }
}