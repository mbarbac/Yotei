using RecordEx = Yotei.ORM.Records.Code.RecordEx;

namespace Yotei.ORM.Records;

// ========================================================
public static class EnumerableCommandExtensions_T
{
    /// <summary>
    /// Returns a list with the results produced by the execution of the given command.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <returns></returns>
    public static List<T> ToList<T>(
        this IEnumerableCommand command, Func<IRecordEx, T> converter) where T : class
    {
        command.ThrowWhenNull();
        converter.ThrowWhenNull();

        var list = new List<T>();
        using var iter = command.GetEnumerator();

        while (iter.MoveNext())
        {
            var r = converter(new RecordEx(iter.Current!, iter.Schema!));
            if (r is not null) list.Add(r);
        }
        return list;
    }

    /// <summary>
    /// Returns a list with the results produced by the execution of the given command.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<List<T>> ToListAsync<T>(
        this IEnumerableCommand command,
        Func<IRecordEx, T> converter,
        CancellationToken token = default) where T : class

    {
        command.ThrowWhenNull();
        converter.ThrowWhenNull();

        var list = new List<T>();
        await using var iter = command.GetAsyncEnumerator(token);

        while (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var r = converter(new RecordEx(iter.Current!, iter.Schema!));
            if (r is not null) list.Add(r);
        }
        return list;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns an array with the results produced by the execution of the given command.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <returns></returns>
    public static T[] ToArray<T>(
        this IEnumerableCommand command, Func<IRecordEx, T> converter) where T : class
    {
        command.ThrowWhenNull();
        converter.ThrowWhenNull();

        return command.ToList(converter).ToArray();
    }

    /// <summary>
    /// Returns an array with the results produced by the execution of the given command.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<T[]> ToArrayAsync<T>(
        this IEnumerableCommand command,
        Func<IRecordEx, T> converter,
        CancellationToken token = default) where T : class
    {
        command.ThrowWhenNull();
        converter.ThrowWhenNull();

        var list = await command.ToListAsync(converter, token).ConfigureAwait(false);
        return list.ToArray();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the first result produced by the execution of the command, or null if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <returns></returns>
    public static T? First<T>(
        this IEnumerableCommand command, Func<IRecordEx, T> converter) where T : class
    {
        command.ThrowWhenNull();
        converter.ThrowWhenNull();

        using var iter = command.GetEnumerator();

        if (iter.MoveNext())
        {
            var r = converter(new RecordEx(iter.Current!, iter.Schema!));
            if (r is not null) return r;
        }
        return null;
    }

    /// <summary>
    /// Returns the first result produced by the execution of the command, or null if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<T?> FirstAsync<T>(
        this IEnumerableCommand command,
        Func<IRecordEx, T> converter,
        CancellationToken token = default) where T : class
    {
        command.ThrowWhenNull();
        converter.ThrowWhenNull();

        await using var iter = command.GetAsyncEnumerator(token);

        if (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var r = converter(new RecordEx(iter.Current!, iter.Schema!));
            if (r is not null) return r;
        }
        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the last result produced by the execution of the command, or null if any.
    /// <br/> This method is provided as a fall-back mechanism, as it iterates through all the
    /// records produced discarding them until the last one is found. It is recommended callers
    /// to consider modifying the logic of the command to prevent using this method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <returns></returns>
    public static T? Last<T>(
        this IEnumerableCommand command, Func<IRecordEx, T> converter) where T : class
    {
        command.ThrowWhenNull();

        T? r = null;
        using var iter = command.GetEnumerator();

        while (iter.MoveNext())
        {
            var temp = converter(new RecordEx(iter.Current!, iter.Schema!));
            if (temp != null) r = temp;
        }
        return r;
    }

    /// <summary>
    /// Returns the last result produced by the execution of the command, or null if any.
    /// <br/> This method is provided as a fall-back mechanism, as it iterates through all the
    /// records produced discarding them until the last one is found. It is recommended callers
    /// to consider modifying the logic of the command to prevent using this method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async ValueTask<T?> LastAsync<T>(
        this IEnumerableCommand command,
        Func<IRecordEx, T> converter,
        CancellationToken token = default) where T : class
    {
        command.ThrowWhenNull();

        T? r = null;
        await using var iter = command.GetAsyncEnumerator(token);

        while (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var temp = converter(new RecordEx(iter.Current!, iter.Schema!));
            if (temp != null) r = temp;
        }
        return r;
    }
}