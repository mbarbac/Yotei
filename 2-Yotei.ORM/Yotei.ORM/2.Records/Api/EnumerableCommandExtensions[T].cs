namespace Yotei.ORM.Records;

// ========================================================
public static partial class EnumerableCommandExtensions
{
    /// <summary>
    /// Returns a list with the results produced by the execution of the command, once converted
    /// by the given delegate, which can be an empty one if no results where produced.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <returns></returns>
    public static List<T> ToList<T>(
        this IEnumerableCommand command,
        Func<IRecord, T> converter)
        where T : class
    {
        command.ThrowWhenNull();
        converter.ThrowWhenNull();

        var list = new List<T>();

        using var iter = command.GetEnumerator();
        while (iter.MoveNext())
        {
            var item = iter.Current;
            if (item != null)
            {
                var r = converter(item);
                if (r is not null) list.Add(r);
            }
        }

        return list;
    }

    /// <summary>
    /// Returns a list with the results produced by the execution of the command, once converted
    /// by the given delegate, which can be an empty one if no results where produced.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<List<T>> ToListAsync<T>(
        this IEnumerableCommand command,
        Func<IRecord, T> converter,
        CancellationToken token = default)
        where T : class
    {
        command.ThrowWhenNull();

        var list = new List<T>();

        await using var iter = command.GetAsyncEnumerator(token);
        while (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var item = iter.Current;
            if (item != null)
            {
                var r = converter(item);
                if (r is not null) list.Add(r);
            }
        }

        return list;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns an array with the results produced by the execution of the command, once converted
    /// by the given delegate, which can be an empty one if no results where produced.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <returns></returns>
    public static T[] ToArray<T>(
        this IEnumerableCommand command, Func<IRecord, T> converter)
        where T : class
    {
        return command.ToList(converter).ToArray();
    }

    /// <summary>
    /// Returns an array with the results produced by the execution of the command, once converted
    /// by the given delegate, which can be an empty one if no results where produced.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<T[]> ToArrayAsync<T>(
        this IEnumerableCommand command,
        Func<IRecord, T> converter,
        CancellationToken token = default)
        where T : class
    {
        var list = await command.ToListAsync(converter, token).ConfigureAwait(false);
        return list.ToArray();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the first result produced by the execution of the command, once converted by the
    /// given delegate, or <c>null</c> if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <returns></returns>
    public static T? First<T>(
        this IEnumerableCommand command, Func<IRecord, T> converter)
        where T : class
    {
        command.ThrowWhenNull();
        converter.ThrowWhenNull();

        using var iter = command.GetEnumerator();

        // Using 'while' to prevent head null records...
        while (iter!.MoveNext())
        {
            var item = iter.Current;
            if (item != null)
            {
                var r = converter(item);
                if (r is not null) return r;
            }
        }

        return null;
    }

    /// <summary>
    /// Returns the first result produced by the execution of the command, once converted by the
    /// given delegate, or <c>null</c> if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<T?> FirstAsync<T>(
        this IEnumerableCommand command,
        Func<IRecord, T> converter,
        CancellationToken token = default)
        where T : class
    {
        command.ThrowWhenNull();

        await using var iter = command.GetAsyncEnumerator(token);

        // Using 'while' to prevent head null records...
        while (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var item = iter.Current;
            if (item != null)
            {
                var r = converter(item);
                if (r is not null) return r;
            }
        }

        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the last result produced by the execution of the command, once converted by the
    /// given delegate, or <c>null</c> if any.
    /// <br/> This method is provided as a fall-back mechanism because it iterates through all
    /// possible results, discarding them until the last one is found. The recommended approach
    /// is to modify the logic of the commad, if such is possible.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <returns></returns>
    public static T? Last<T>(
        this IEnumerableCommand command, Func<IRecord, T> converter)
        where T : class
    {
        command.ThrowWhenNull();
        converter.ThrowWhenNull();

        using var iter = command.GetEnumerator();
        T? record = null;

        // Using 'while' to prevent head null records...
        while (iter!.MoveNext())
        {
            var item = iter.Current;
            if (item != null)
            {
                var r = converter(item);
                if (r is not null) record = r;
            }
        }

        return record;
    }

    /// <summary>
    /// Returns the last result produced by the execution of the command, once converted by the
    /// given delegate, or <c>null</c> if any.
    /// <br/> This method is provided as a fall-back mechanism because it iterates through all
    /// possible results, discarding them until the last one is found. The recommended approach
    /// is to modify the logic of the commad, if such is possible.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<T?> LastAsync<T>(
        this IEnumerableCommand command,
        Func<IRecord, T> converter,
        CancellationToken token = default)
        where T : class
    {
        command.ThrowWhenNull();

        await using var iter = command.GetAsyncEnumerator(token);
        T? record = null;

        // Using 'while' to prevent head null records...
        while (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var item = iter.Current;
            if (item != null)
            {
                var r = converter(item);
                if (r is not null) record = r;
            }
        }

        return record;
    }
}