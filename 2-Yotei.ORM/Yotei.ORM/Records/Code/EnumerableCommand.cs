namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IEnumerableCommand"/>
/// </summary>
public abstract class EnumerableCommand : Command, IEnumerableCommand
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
    public ICommandEnumerator GetEnumerator() => Connection.Records.CreateEnumerator(this);
    IEnumerator<IRecord> IEnumerable<IRecord>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public ICommandEnumerator GetAsyncEnumerator(
        CancellationToken token = default) => Connection.Records.CreateAsyncEnumerator(this, token);

    IAsyncEnumerator<IRecord> IAsyncEnumerable<IRecord>.GetAsyncEnumerator(
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
    public int Skip
    {
        get => _Skip;
        set => _Skip = value >= 0 ? value : 0;
    }
    int _Skip = 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Take
    {
        get => _Take;
        set => _Take = value >= 0 ? value : 0;
    }
    int _Take = 0;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<IRecord> ToList()
    {
        using var iter = GetEnumerator();
        var list = new List<IRecord>();

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
    public async ValueTask<List<IRecord>> ToListAsync(CancellationToken token = default)
    {
        await using var iter = GetAsyncEnumerator(token);
        var list = new List<IRecord>();

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
    public IRecord[] ToArray() => ToList().ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask<IRecord[]> ToArrayAsync(CancellationToken token = default)
    {
        var list = await ToListAsync(token).ConfigureAwait(false);
        return list.ToArray();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IRecord? First()
    {
        using var iter = GetEnumerator();
        IRecord? r = null;

        if (iter.MoveNext())
        {
            var temp = iter.Current;
            if (temp is not null) r = temp;
        }
        return r;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask<IRecord?> FirstAsync(CancellationToken token = default)
    {
        await using var iter = GetAsyncEnumerator(token);
        IRecord? r = null;

        if (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var temp = iter.Current;
            if (temp is not null) r = temp;
        }
        return r; ;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IRecord? Last()
    {
        using var iter = GetEnumerator();
        IRecord? r = null;

        while (iter.MoveNext())
        {
            var temp = iter.Current;
            if (temp is not null) r = temp;
        }
        return r;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask<IRecord?> LastAsync(CancellationToken token = default)
    {
        await using var iter = GetAsyncEnumerator(token);
        IRecord? r = null;

        while (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var temp = iter.Current;
            if (temp is not null) r = temp;
        }
        return r;
    }
}