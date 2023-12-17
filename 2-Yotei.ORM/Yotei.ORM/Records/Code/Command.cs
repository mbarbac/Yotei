namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ORM.ICommand"/>
/// </summary>
public abstract class Command : ORM.ICommand
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    public Command(IConnection connection) => Connection = connection.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var str = GetText(false, out var parameters);
        if (parameters.Count > 0) str += $"; -- [{string.Join(", ", parameters)}]";
        return str;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IConnection Connection { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public abstract string GetText(out IParameterList parameters);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="iterable"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public abstract string GetText(bool iterable, out IParameterList parameters);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public abstract ICommandEnumerator GetEnumerator();
    ORM.ICommandEnumerator ORM.ICommand.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public abstract ICommandEnumerator GetAsyncEnumerator(CancellationToken token = default);
    IAsyncEnumerator<object> IAsyncEnumerable<object>.GetAsyncEnumerator(CancellationToken token) => GetAsyncEnumerator(token);
    ORM.ICommandEnumerator ORM.ICommand.GetAsyncEnumerator(CancellationToken token)=> GetAsyncEnumerator(token);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public abstract ICommandExecutor GetExecutor();
    ORM.ICommandExecutor ORM.ICommand.GetExecutor() => GetExecutor();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public int Execute() => GetExecutor().Execute();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask<int> ExecuteAsync(
        CancellationToken token = default)
        => await GetExecutor().ExecuteAsync(token).ConfigureAwait(false);

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
            if (r != null) list.Add(r);
        }
        return list;
    }
    List<object> ORM.ICommand.ToList()
    {
        var items = ToList();
        return items.Cast<object>().ToList();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask<List<IRecord>> ToListAsync(CancellationToken token = default)
    {
        await using var iter = GetAsyncEnumerator(token) as IAsyncEnumerator<IRecord>;
        var list = new List<IRecord>();

        while (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var r = iter.Current;
            if (r != null) list.Add(r);
        }
        return list;
    }
    async ValueTask<List<object>> ORM.ICommand.ToListAsync(CancellationToken token)
    {
        var items = await ToListAsync(token).ConfigureAwait(false);
        return items.Cast<object>().ToList();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IRecord[] ToArray() => ToList().ToArray();
    object[] ORM.ICommand.ToArray() => ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask<IRecord[]> ToArrayAsync(CancellationToken token = default)
    {
        var list = await ToListAsync(token).ConfigureAwait(false);
        return [.. list];
    }
    async ValueTask<object[]> ORM.ICommand.ToArrayAsync(
        CancellationToken token) => await ToArrayAsync(token).ConfigureAwait(false);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IRecord? First()
    {
        using var iter = GetEnumerator();

        if (iter.MoveNext())
        {
            var r = iter.Current;
            if (r != null) return r;
        }
        return null;
    }
    object? ORM.ICommand.First() => First();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask<IRecord?> FirstAsync(CancellationToken token = default)
    {
        await using var iter = GetAsyncEnumerator(token) as IAsyncEnumerator<IRecord>;

        if (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var r = iter.Current;
            if (r != null) return r;
        }
        return null;
    }
    async ValueTask<object?> ORM.ICommand.FirstAsync(
        CancellationToken token) => await FirstAsync(token).ConfigureAwait(false);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IRecord? Last()
    {
        using var iter = GetEnumerator();
        IRecord? item = null;

        while (iter.MoveNext())
        {
            var r = iter.Current;
            if (r != null) item = r;
        }
        return item;
    }
    object? ORM.ICommand.Last() => Last();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask<IRecord?> LastAsync(CancellationToken token = default)
    {
        await using var iter = GetAsyncEnumerator(token) as IAsyncEnumerator<IRecord>;
        IRecord? item = null;

        while (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var r = iter.Current;
            if (r != null) item = r;
        }
        return item;
    }
    async ValueTask<object?> ORM.ICommand.LastAsync(CancellationToken token)
        => await LastAsync(token).ConfigureAwait(false);
}