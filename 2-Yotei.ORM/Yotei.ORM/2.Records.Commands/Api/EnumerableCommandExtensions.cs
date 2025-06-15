namespace Yotei.ORM;

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

        List<IRecord> list = [];

        using var iter = command.GetEnumerator();
        while (iter.MoveNext())
        {
            var item = iter.Current;
            if (item is not null) list.Add(item);
        }

        return list;
    }

    /// <summary>
    /// Returns a list with the results produced by the execution of this command.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async ValueTask<List<IRecord>> ToListAsync(
        this IEnumerableCommand command,
        CancellationToken token = default)
    {
        command.ThrowWhenNull();

        List<IRecord> list = [];

        await using var iter = command.GetAsyncEnumerator(token);
        while (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var item = iter.Current;
            if (item is not null) list.Add(item);
        }

        return list;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns an array with the results produced by the execution of this command.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static IRecord[] ToArray(this IEnumerableCommand command)
    {
        var items = command.ToList();
        return items.ToArray();
    }

    /// <summary>
    /// Returns an array with the results produced by the execution of this command.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async ValueTask<IRecord[]> ToArrayAsync(
        this IEnumerableCommand command,
        CancellationToken token = default)
    {
        var items = await command.ToListAsync(token).ConfigureAwait(false);
        return items.ToArray();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the first result produced by the execution of the command, or <c>null</c> if any.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static IRecord? First(this IEnumerableCommand command)
    {
        command.ThrowWhenNull();

        using var iter = command.GetEnumerator();
        while (iter.MoveNext())
        {
            var item = iter.Current;
            if (item is not null) return item;
        }

        return null;
    }

    /// <summary>
    /// Returns the first result produced by the execution of the command, or <c>null</c> if any.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async ValueTask<IRecord?> FirstAsync(
        this IEnumerableCommand command,
        CancellationToken token = default)
    {
        command.ThrowWhenNull();

        await using var iter = command.GetAsyncEnumerator(token);
        while (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var item = iter.Current;
            if (item is not null) return item;
        }

        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the last result produced by the execution of the command, or <c>null</c> if any.
    /// <para>
    /// This method is provided as a fall-back mechanism because it iterates through all possible
    /// results, discarding them until the last one is found. The recommended approach is to modify
    /// the logic of the original command, if such is possible.
    /// </para>
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static IRecord? Last(this IEnumerableCommand command)
    {
        command.ThrowWhenNull();

        using var iter = command.GetEnumerator();
        IRecord? record = null;

        while (iter.MoveNext())
        {
            var item = iter.Current;
            if (item is not null) record = item;
        }

        return record;
    }

    /// <summary>
    /// Returns the last result produced by the execution of the command, or <c>null</c> if any.
    /// <para>
    /// This method is provided as a fall-back mechanism because it iterates through all possible
    /// results, discarding them until the last one is found. The recommended approach is to modify
    /// the logic of the original command, if such is possible.
    /// </para>
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async ValueTask<IRecord?> LastAsync(
        this IEnumerableCommand command,
        CancellationToken token = default)
    {
        command.ThrowWhenNull();

        await using var iter = command.GetAsyncEnumerator(token);
        IRecord? record = null;

        while (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var item = iter.Current;
            if (item is not null) record = item;
        }

        return record;
    }
}