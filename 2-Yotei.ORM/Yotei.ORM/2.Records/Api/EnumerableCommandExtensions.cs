namespace Yotei.ORM.Records;

// ========================================================
public static class EnumerableCommandExtensions
{
    /// <summary>
    /// Returns a list with the results produced by the execution of this command.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static List<IRecord> ToList(this IEnumerableCommand command)
    {
        command.ThrowWhenNull();

        var list = new List<IRecord>();

        using var iter = command.GetEnumerator();
        while (iter.MoveNext()) if (iter.Current != null) list.Add(iter.Current);

        return list;
    }

    /// <summary>
    /// Returns a list with the results produced by the execution of this command.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<List<IRecord>> ToListAsync(this IEnumerableCommand command, CancellationToken token = default)
    {
        command.ThrowWhenNull();

        var list = new List<IRecord>();

        await using var iter = command.GetAsyncEnumerator(token);

        while (await iter.MoveNextAsync().ConfigureAwait(false))
            if (iter.Current != null) list.Add(iter.Current);

        return list;
    }

    /// <summary>
    /// Returns an array with the results produced by the execution of this command.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0305")]
    public static IRecord[] ToArray(this IEnumerableCommand command)
    {
        return command.ToList().ToArray();
    }

    /// <summary>
    /// Returns an array with the results produced by the execution of this command.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0305")]
    public async static ValueTask<IRecord[]> ToArrayAsync(this IEnumerableCommand command, CancellationToken token = default)
    {
        command.ThrowWhenNull();

        var list = await command.ToListAsync(token).ConfigureAwait(false);
        return list.ToArray();
    }

    /// <summary>
    /// Returns the first result produced by the execution of this command, or null if no records
    /// were available.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static IRecord? First(this IEnumerableCommand command)
    {
        command.ThrowWhenNull();

        using var iter = command.GetEnumerator();

        if (iter.MoveNext()) return iter.Current;
        return null;
    }

    /// <summary>
    /// Returns the first result produced by the execution of this command, or null if no records
    /// were available.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<IRecord?> FirstAsync(this IEnumerableCommand command, CancellationToken token = default)
    {
        command.ThrowWhenNull();

        await using var iter = command.GetAsyncEnumerator(token);

        if (await iter.MoveNextAsync().ConfigureAwait(false)) return iter.Current;
        return null;
    }

    /// <summary>
    /// Returns the last result produced by the execution of this command, or null if no records
    /// were available.
    /// <br/> This method is just provided as a fall-back mechanism, as it iterated through the
    /// records produced, discarding them until the last one is found.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static IRecord? Last(this IEnumerableCommand command)
    {
        command.ThrowWhenNull();

        IRecord? result = null;
        using var iter = command.GetEnumerator();

        while (iter.MoveNext()) result = iter.Current;
        return result;
    }

    /// <summary>
    /// Returns the last result produced by the execution of this command, or null if no records
    /// were available.
    /// <br/> This method is just provided as a fall-back mechanism, as it iterated through the
    /// records produced, discarding them until the last one is found.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<IRecord?> LastAsync(this IEnumerableCommand command, CancellationToken token = default)
    {
        command.ThrowWhenNull();

        IRecord? result = null;
        await using var iter = command.GetAsyncEnumerator(token);

        while (await iter.MoveNextAsync().ConfigureAwait(false)) result = iter.Current;
        return result;
    }
}