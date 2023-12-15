namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICommandReader"/>
/// </summary>
[SuppressMessage("", "IDE0290")]
public abstract class CommandReader : DisposableClass, ICommandReader
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="commandText"></param>
    /// <param name="parameters"></param>
    public CommandReader(
        IConnection connection,
        string commandText,
        IParameterList parameters)
    {
        Connection = connection.ThrowWhenNull();
        CommandText = commandText.NotNullNotEmpty();
        Parameters = parameters.ThrowWhenNull();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IConnection Connection { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string CommandText { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IParameterList Parameters { get; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public abstract IEnumerator<object> GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public abstract IAsyncEnumerator<object> GetAsyncEnumerator(CancellationToken token = default);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<object> ToList()
    {
        using var iter = GetEnumerator();
        var list = new List<object>();

        while (iter.MoveNext())
        {
            var r = iter.Current;
            if (r is not null) list.Add(r);
        }
        return list;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask<List<object>> ToListAsync(CancellationToken token = default)
    {
        await using var iter = GetAsyncEnumerator(token);
        var list = new List<object>();

        while (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var r = iter.Current;
            if (r is not null) list.Add(r);
        }
        return list;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public object[] ToArray() => ToList().ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0305")]
    public async ValueTask<object[]> ToArrayAsync(CancellationToken token = default)
    {
        var list = await ToListAsync(token).ConfigureAwait(false);
        return list.ToArray();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public object? First()
    {
        using var iter = GetEnumerator();

        if (iter.MoveNext())
        {
            var r = iter.Current;
            if (r is not null) return r;
        }
        return null;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask<object?> FirstAsync(CancellationToken token = default)
    {
        await using var iter = GetAsyncEnumerator(token);

        if (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var r = iter.Current;
            if (r is not null) return r;
        }
        return null;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public object? Last()
    {
        using var iter = GetEnumerator();
        object? item = null;

        while (iter.MoveNext())
        {
            var r = iter.Current;
            if (r is not null) item = r;
        }
        return item;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask<object?> LastAsync(CancellationToken token = default)
    {
        await using var iter = GetAsyncEnumerator(token);
        object? item = null;

        while (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var r = iter.Current;
            if (r is not null) item = r;
        }
        return item;
    }
}