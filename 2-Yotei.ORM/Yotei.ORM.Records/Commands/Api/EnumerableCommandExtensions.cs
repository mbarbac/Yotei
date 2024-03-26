using RecordEx = Yotei.ORM.Records.Code.RecordEx;

namespace Yotei.ORM.Records;

// ========================================================
public static class EnumerableCommandExtensions
{
    /// <summary>
    /// Returns a list with the results produced by the execution of the given command.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static List<IRecordEx> ToList(this IEnumerableCommand command)
    {
        command.ThrowWhenNull();

        var list = new List<IRecordEx>();
        using var iter = command.GetEnumerator();

        while (iter.MoveNext())
        {
            list.Add(new RecordEx(iter.Current!, iter.Schema!));
        }
        return list;
    }

    /// <summary>
    /// Returns a list with the results produced by the execution of the given command.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<List<IRecordEx>> ToListAsync(
        this IEnumerableCommand command,
        CancellationToken token = default)
    {
        command.ThrowWhenNull();

        var list = new List<IRecordEx>();
        await using var iter = command.GetAsyncEnumerator(token);

        while (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            list.Add(new RecordEx(iter.Current!, iter.Schema!));
        }
        return list;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns an array with the results produced by the execution of the given command.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static IRecordEx[] ToArray(this IEnumerableCommand command)
    {
        command.ThrowWhenNull();
        return command.ToList().ToArray();
    }

    /// <summary>
    /// Returns an array with the results produced by the execution of the given command.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<IRecordEx[]> ToArrayAsync(
        this IEnumerableCommand command,
        CancellationToken token = default)
    {
        command.ThrowWhenNull();

        var list = await command.ToListAsync(token).ConfigureAwait(false);
        return list.ToArray();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the first result produced by the execution of the command, or null if any.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static IRecordEx? First(this IEnumerableCommand command)
    {
        command.ThrowWhenNull();

        using var iter = command.GetEnumerator();

        if (iter.MoveNext()) return new RecordEx(iter.Current!, iter.Schema!);
        return null;
    }

    /// <summary>
    /// Returns the first result produced by the execution of the command, or null if any.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<IRecordEx?> FirstAsync(
        this IEnumerableCommand command,
        CancellationToken token = default)
    {
        command.ThrowWhenNull();

        await using var iter = command.GetAsyncEnumerator(token);

        if (await iter.MoveNextAsync().ConfigureAwait(false))
            return new RecordEx(iter.Current!, iter.Schema!);

        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the last result produced by the execution of the command, or null if any.
    /// <br/> This method is provided as a fall-back mechanism, as it iterates through all the
    /// records produced discarding them until the last one is found. It is recommended callers
    /// to consider modifying the logic of the command to prevent using this method.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static IRecordEx? Last(this IEnumerableCommand command)
    {
        command.ThrowWhenNull();

        IRecordEx? r = null;
        using var iter = command.GetEnumerator();

        while (iter.MoveNext()) r = new RecordEx(iter.Current!, iter.Schema!);
        return r;
    }

    /// <summary>
    /// Returns the last result produced by the execution of the command, or null if any.
    /// <br/> This method is provided as a fall-back mechanism, as it iterates through all the
    /// records produced discarding them until the last one is found. It is recommended callers
    /// to consider modifying the logic of the command to prevent using this method.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<IRecordEx?> LastAsync(
        this IEnumerableCommand command,
        CancellationToken token = default)
    {
        command.ThrowWhenNull();

        IRecordEx? r = null;
        await using var iter = command.GetAsyncEnumerator(token);

        while (await iter.MoveNextAsync().ConfigureAwait(false))
            r = new RecordEx(iter.Current!, iter.Schema!);

        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Executes the given action if an scalar can be obtained from the execution of the given
    /// command, where the scalar is defined as the value of the first column of the first record
    /// obtained, if any.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="action"></param>
    public static void GetScalar(
        this IEnumerableCommand command,
        Action<object?> action)
    {
        command.ThrowWhenNull();
        action.ThrowWhenNull();

        var meta = command.First();
        if (meta != null)
        {
            if (meta.Record.Count == 0) throw new NotFoundException(
                "The first record is an empty one, no scalar can be obtained.");

            var value = meta.Record[0];
            action(value);
        }
    }

    /// <summary>
    /// Executes the given action if an scalar can be obtained from the execution of the given
    /// command, where the scalar is defined as the value of the first column of the first record
    /// obtained, if any.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="action"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async ValueTask GetScalarAsync(
        this IEnumerableCommand command,
        Action<object?> action,
        CancellationToken token = default)
    {
        command.ThrowWhenNull();
        action.ThrowWhenNull();

        var meta = await command.FirstAsync(token).ConfigureAwait(false);
        if (meta != null)
        {
            if (meta.Record.Count == 0) throw new NotFoundException(
                "The first record is an empty one, no scalar can be obtained.");

            var value = meta.Record[0];
            action(value);
        }
    }
}