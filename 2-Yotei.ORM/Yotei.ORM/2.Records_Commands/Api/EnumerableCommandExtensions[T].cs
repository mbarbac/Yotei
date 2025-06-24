namespace Yotei.ORM.Records;

// ========================================================
public static class EnumerableCommandExtensions_T
{
    /// <summary>
    /// Returns a list with the results produced by the execution of this command.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <returns></returns>
    public static List<T> ToList<T>(
        this IEnumerableCommand command,
        Func<IRecord, T> converter)
    {
        command.ThrowWhenNull();
        converter.ThrowWhenNull();

        List<T> list = [];

        using var iter = command.SelectItems<T>(converter);
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
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async ValueTask<List<T>> ToListAsync<T>(
        this IEnumerableCommand command,
        Func<IRecord, T> converter, CancellationToken token = default)
    {
        command.ThrowWhenNull();
        converter.ThrowWhenNull();

        List<T> list = [];

        await using var iter = command.SelectItemsAsync<T>(converter, token);
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
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <returns></returns>
    public static T[] ToArray<T>(
        this IEnumerableCommand command,
        Func<IRecord, T> converter)
    {
        var items = command.ToList(converter);
        return items.ToArray();
    }

    /// <summary>
    /// Returns an array with the results produced by the execution of this command.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async ValueTask<T[]> ToArrayAsync<T>(
        this IEnumerableCommand command,
        Func<IRecord, T> converter, CancellationToken token = default)
    {
        var items = await command.ToListAsync(converter, token).ConfigureAwait(false);
        return items.ToArray();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the first result produced by the execution of the command, or <c>null</c> if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <returns></returns>
    public static T? First<T>(
        this IEnumerableCommand command,
        Func<IRecord, T> converter)
    {
        command.ThrowWhenNull();
        converter.ThrowWhenNull();

        using var iter = command.SelectItems<T>(converter);
        while (iter.MoveNext())
        {
            var item = iter.Current;
            if (item is not null) return item;
        }

        return default;
    }

    /// <summary>
    /// Returns the first result produced by the execution of the command, or <c>null</c> if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async ValueTask<T?> FirstAsync<T>(
        this IEnumerableCommand command,
        Func<IRecord, T> converter, CancellationToken token = default)
    {
        command.ThrowWhenNull();
        converter.ThrowWhenNull();

        await using var iter = command.SelectItemsAsync<T>(converter, token);
        while (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var item = iter.Current;
            if (item is not null) return item;
        }

        return default;
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
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <returns></returns>
    public static T? Last<T>(
        this IEnumerableCommand command,
        Func<IRecord, T> converter)
    {
        command.ThrowWhenNull();
        converter.ThrowWhenNull();

        using var iter = command.SelectItems<T>(converter);
        T? record = default;

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
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async ValueTask<T?> LastAsync<T>(
        this IEnumerableCommand command,
        Func<IRecord, T> converter, CancellationToken token = default)
    {
        command.ThrowWhenNull();
        converter.ThrowWhenNull();

        await using var iter = command.SelectItemsAsync<T>(converter, token);
        T? record = default;

        while (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var item = iter.Current;
            if (item is not null) record = item;
        }

        return record;
    }
}