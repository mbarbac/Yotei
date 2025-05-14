namespace Yotei.ORM.Records;

// ========================================================
public static partial class EnumerableCommandExtensions
{
    /// <summary>
    /// Returns a list with the results produced by the execution of the command, which can be
    /// an empty one if no results where produced.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static List<IRecord> ToList(this IEnumerableCommand command)
    {
        command.ThrowWhenNull();

        var list = new List<IRecord>();

        using var iter = command.GetEnumerator();
        while (iter.MoveNext())
        {
            var item = iter.Current;
            if (item != null) list.Add(item);
        }

        return list;
    }

    /// <summary>
    /// Returns a list with the results produced by the execution of the command, which can be
    /// an empty one if no results where produced.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<List<IRecord>> ToListAsync(
        this IEnumerableCommand command,
        CancellationToken token = default)
    {
        command.ThrowWhenNull();

        var list = new List<IRecord>();

        await using var iter = command.GetAsyncEnumerator(token);
        while (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var item = iter.Current;
            if (item != null) list.Add(item);
        }

        return list;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns an array with the results produced by the execution of the command, which can be
    /// an empty one if no results where produced.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static IRecord[] ToArray(this IEnumerableCommand command)
    {
        return command.ToList().ToArray();
    }

    /// <summary>
    /// Returns an array with the results produced by the execution of the command, which can be
    /// an empty one if no results where produced.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<IRecord[]> ToArrayAsync(
        this IEnumerableCommand command,
        CancellationToken token = default)
    {
        var list = await command.ToListAsync(token).ConfigureAwait(false);
        return list.ToArray();
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

        // Using 'while' to prevent head null records...
        while (iter!.MoveNext())
        {
            var item = iter.Current;
            if (item != null) return item;
        }

        return null;
    }

    /// <summary>
    /// Returns the first result produced by the execution of the command, or <c>null</c> if any.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<IRecord?> FirstAsync(
        this IEnumerableCommand command,
        CancellationToken token = default)
    {
        command.ThrowWhenNull();
        
        await using var iter = command.GetAsyncEnumerator(token);

        // Using 'while' to prevent head null records...
        while (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var item = iter.Current;
            if (item != null) return item;
        }

        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the last result produced by the execution of the command, or <c>null</c> if any.
    /// <br/> This method is provided as a fall-back mechanism because it iterates through all
    /// possible results, discarding them until the last one is found. The recommended approach
    /// is to modify the logic of the commad, if such is possible.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static IRecord? Last(this IEnumerableCommand command)
    {
        command.ThrowWhenNull();

        using var iter = command.GetEnumerator();
        IRecord? record = null;

        // Using 'while' to prevent head null records...
        while (iter!.MoveNext())
        {
            var item = iter.Current;
            if (item != null) record = item;
        }

        return record;
    }

    /// <summary>
    /// Returns the last result produced by the execution of the command, or <c>null</c> if any.
    /// <br/> This method is provided as a fall-back mechanism because it iterates through all
    /// possible results, discarding them until the last one is found. The recommended approach
    /// is to modify the logic of the commad, if such is possible.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<IRecord?> LastAsync(
        this IEnumerableCommand command,
        CancellationToken token = default)
    {
        command.ThrowWhenNull();

        await using var iter = command.GetAsyncEnumerator(token);
        IRecord? record = null;

        // Using 'while' to prevent head null records...
        while (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            var item = iter.Current;
            if (item != null) record = item;
        }

        return record;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to give the scalar produced by the execution of the command, defined as the value
    /// of the unique column of the first and unique record returned, if any. If so, then the
    /// given action is invoked to extract the requested value.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static bool TryGetScalar(
        this IEnumerableCommand command,
        Action<object?> action)
    {
        command.ThrowWhenNull();
        action.ThrowWhenNull();

        var temp = command.WithTake(2);
        var list = temp.ToList();
        if (list.Count == 1)
        {
            var record = list[0];
            if (record.Count == 1)
            {
                var value = record[0];
                action(value);
            }
        }

        return false;
    }

    /// <summary>
    /// Tries to give the scalar produced by the execution of the command, defined as the value
    /// of the unique column of the first and unique record returned, if any. If so, then the
    /// given action is invoked to extract the requested value.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static bool TryGetScalar(
        this IEnumerableCommand command,
        Action<object?, ISchemaEntry?> action)
    {
        command.ThrowWhenNull();
        action.ThrowWhenNull();

        var temp = command.WithTake(2);
        var list = temp.ToList();
        if (list.Count == 1)
        {
            var record = list[0];
            if (record.Count == 1)
            {
                var value = record[0];
                var entry = record.Schema?[0];
                action(value, entry);
            }
        }

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to give the scalar produced by the execution of the command, defined as the value
    /// of the unique column of the first and unique record returned, if any. If so, then the
    /// given action is invoked to extract the requested value.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="action"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<bool> TryGetScalar(
        this IEnumerableCommand command,
        Action<object?> action,
        CancellationToken token = default)
    {
        command.ThrowWhenNull();
        action.ThrowWhenNull();

        var temp = command.WithTake(2);
        var list = await temp.ToListAsync(token).ConfigureAwait(false);
        if (list.Count == 1)
        {
            var record = list[0];
            if (record.Count == 1)
            {
                var value = record[0];
                action(value);
            }
        }

        return false;
    }

    /// <summary>
    /// Tries to give the scalar produced by the execution of the command, defined as the value
    /// of the unique column of the first and unique record returned, if any. If so, then the
    /// given action is invoked to extract the requested value.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="action"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<bool> TryGetScalar(
        this IEnumerableCommand command,
        Action<object?, ISchemaEntry?> action,
        CancellationToken token = default)
    {
        command.ThrowWhenNull();
        action.ThrowWhenNull();

        var temp = command.WithTake(2);
        var list = await temp.ToListAsync(token).ConfigureAwait(false);
        if (list.Count == 1)
        {
            var record = list[0];
            if (record.Count == 1)
            {
                var value = record[0];
                var entry = record.Schema?[0];
                action(value, entry);
            }
        }

        return false;
    }
}