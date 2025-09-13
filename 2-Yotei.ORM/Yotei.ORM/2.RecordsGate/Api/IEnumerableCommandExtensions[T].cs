namespace Yotei.ORM.Records;

// ========================================================
public static class IEnumerableCommandExtensions_T
{
    /// <summary>
    /// Returns a new command instance that can convert the records produced by the original
    /// command wrapped by this new instance into the arbitrary results obtained by invoking
    /// the given delegate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <returns></returns>
    public static IEnumerableCommand<T> ConvertInto<T>(
        this IEnumerableCommand command,
        Func<IRecord, ISchema?, T> converter)
    {
        command.ThrowWhenNull();
        converter.ThrowWhenNull();
        return new EnumerableCommand<T>(command, converter);
    }
/*
    // ----------------------------------------------------

    /// <summary>
    /// Returns a list with the records produced by the execution of the command.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <returns></returns>
    public static List<T> ToList<T>(this IEnumerableCommand<T> command)
    {
        command.ThrowWhenNull();
        List<T> list = [];
#if USE_ITER
        using var iter = command.GetEnumerator();
        while (iter.MoveNext())
        {
            var item = iter.Current;
            if (item is not null) list.Add(item);
        }
#else
        foreach (var item in command)
            if (item is not null) list.Add(item);
#endif
        return list;
    }

    /// <summary>
    /// Returns a list with the records produced by the execution of the command.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async ValueTask<List<T>> ToListAsync<T>(
        this IEnumerableCommand<T> command,
        CancellationToken token = default)
    {
        command.ThrowWhenNull();
        List<T> list = [];

#if USE_ITER
        await using var iter = command.GetAsyncEnumerator(token);
        while (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var item = iter.Current;
            if (item is not null) list.Add(item);
        }
#else
        await foreach (var item in command.WithCancellation(token).ConfigureAwait(false))
            if (item is not null) list.Add(item);
#endif
        return list;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns an array with the records produced by the execution of this command.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <returns></returns>
    public static T[] ToArray<T>(this IEnumerableCommand<T> command)
    {
        var items = command.ToList();
        return items.ToArray();
    }

    /// <summary>
    /// Returns a list with the records produced by the execution of this command.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async ValueTask<T[]> ToArrayAsync<T>(
        this IEnumerableCommand<T> command, CancellationToken token = default)
    {
        var items = await command.ToListAsync(token).ConfigureAwait(false);
        return items.ToArray();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the first record produced by the execution of the command, or <c>null</c> if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <returns></returns>
    public static T? First<T>(this IEnumerableCommand<T> command)
    {
        command.ThrowWhenNull();

#if USE_ITER
        using var iter = command.GetEnumerator();
        while (iter.MoveNext())
        {
            var item = iter.Current;
            if (item is not null) return item;
        }
#else
        foreach (var item in command)
            if (item is not null) return item;
#endif
        return default;
    }

    /// <summary>
    /// Returns the first record produced by the execution of the command, or <c>null</c> if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <returns></returns>
    public static async ValueTask<T?> FirstAsync<T>(
        this IEnumerableCommand<T> command, CancellationToken token = default)
    {
        command.ThrowWhenNull();

#if USE_ITER
        await using var iter = command.GetAsyncEnumerator(token);
        while (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var item = iter.Current;
            if (item is not null) return item;
        }
#else
        await foreach (var item in command.WithCancellation(token).ConfigureAwait(false))
            if (item is not null) return item;
#endif
        return default;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the last record produced by the execution of the command, or <c>null</c> if any.
    /// <para>
    /// This method is provided as a fall-back mechanism because it iterates through all possible
    /// records, discarding them until the last one is found. The recommended approach is to modify
    /// the logic of the original command, if such is possible.
    /// </para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <returns></returns>
    public static T? Last<T>(this IEnumerableCommand<T> command)
    {
        command.ThrowWhenNull();
        T? record = default;

#if USE_ITER
        using var iter = command.GetEnumerator();
        while (iter.MoveNext())
        {
            var item = iter.Current;
            if (item is not null) record = item;
        }
#else
        foreach (var item in command)
            if (item is not null) record = item;
#endif
        return record;
    }

    /// <summary>
    /// Returns the last record produced by the execution of the command, or <c>null</c> if any.
    /// <para>
    /// This method is provided as a fall-back mechanism because it iterates through all possible
    /// records, discarding them until the last one is found. The recommended approach is to modify
    /// the logic of the original command, if such is possible.
    /// </para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async ValueTask<T?> LastAsync<T>(
        this IEnumerableCommand<T> command,
        CancellationToken token = default)
    {
        command.ThrowWhenNull();
        T? record = default;

#if USE_ITER
        await using var iter = command.GetAsyncEnumerator(token);
        while (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var item = iter.Current;
            if (item is not null) record = item;
        }
#else
        await foreach (var item in command.WithCancellation(token).ConfigureAwait(false))
            if (item is not null) record = item;
#endif
        return record;
    }*/
}