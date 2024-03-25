#pragma warning disable IDE0305 // Simplify collection initialization

using MetaRecord = Yotei.ORM.Records.Code.MetaRecord;

namespace Yotei.ORM.Records;

// ========================================================
public static class IEnumerableCommandExtensions
{
    /// <summary>
    /// Returns a list with the results produced by the execution of the given command.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static List<IMetaRecord> ToList(this IEnumerableCommand command)
    {
        command.ThrowWhenNull();

        var list = new List<IMetaRecord>();
        using var iter = command.GetEnumerator();

        while (iter.MoveNext())
        {
            list.Add(new MetaRecord(iter.Current!, iter.Schema!));
        }
        return list;
    }

    /// <summary>
    /// Returns a list with the results produced by the execution of the given command.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<List<IMetaRecord>> ToListAsync(
        this IEnumerableCommand command,
        CancellationToken token = default)
    {
        command.ThrowWhenNull();

        var list = new List<IMetaRecord>();
        await using var iter = command.GetAsyncEnumerator(token);

        while (await iter.MoveNextAsync().ConfigureAwait(false))
        {
            list.Add(new MetaRecord(iter.Current!, iter.Schema!));
        }
        return list;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns an array with the results produced by the execution of the given command.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static IMetaRecord[] ToArray(this IEnumerableCommand command)
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
    public async static ValueTask<IMetaRecord[]> ToArrayAsync(
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
    public static IMetaRecord? First(this IEnumerableCommand command)
    {
        command.ThrowWhenNull();

        using var iter = command.GetEnumerator();

        if (iter.MoveNext()) return new MetaRecord(iter.Current!, iter.Schema!);
        return null;
    }

    /// <summary>
    /// Returns the first result produced by the execution of the command, or null if any.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<IMetaRecord?> FirstAsync(
        this IEnumerableCommand command,
        CancellationToken token = default)
    {
        command.ThrowWhenNull();

        await using var iter = command.GetAsyncEnumerator(token);

        if (await iter.MoveNextAsync().ConfigureAwait(false))
            return new MetaRecord(iter.Current!, iter.Schema!);

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
    public static IMetaRecord? Last(this IEnumerableCommand command)
    {
        command.ThrowWhenNull();

        IMetaRecord? r = null;
        using var iter = command.GetEnumerator();

        while (iter.MoveNext()) r = new MetaRecord(iter.Current!, iter.Schema!);
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
    public async static ValueTask<IMetaRecord?> LastAsync(
        this IEnumerableCommand command,
        CancellationToken token = default)
    {
        command.ThrowWhenNull();

        IMetaRecord? r = null;
        await using var iter = command.GetAsyncEnumerator(token);

        while (await iter.MoveNextAsync().ConfigureAwait(false))
            r = new MetaRecord(iter.Current!, iter.Schema!);

        return r;
    }
}