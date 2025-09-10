namespace Yotei.ORM.Records;

// ========================================================
public static class IEnumerableCommandExtensions
{
    /// <summary>
    /// Returns a list with the records produced by the execution of the command.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static List<IRecord> ToList(this IEnumerableCommand command)
    {
        command.ThrowWhenNull();

        List<IRecord> list = [];
        foreach (var item in command)
        {
            if (item is not null) list.Add(item);
        }
        return list;
    }

    /// <summary>
    /// Returns a list with the records produced by the execution of the command.
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
        await foreach (var item in command.WithCancellation(token).ConfigureAwait(false))
        {
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
    /// Returns a list with the results produced by the execution of this command.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async ValueTask<IRecord[]> ToArrayAsync(
        this IEnumerableCommand command, CancellationToken token = default)
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

        foreach (var item in command)
        {
            if (item is not null) return item;
        }
        return null;
    }

    /// <summary>
    /// Returns the first result produced by the execution of the command, or <c>null</c> if any.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static async ValueTask<IRecord?> FirstAsync(
        this IEnumerableCommand command, CancellationToken token = default)
    {
        command.ThrowWhenNull();

        await foreach (var item in command.WithCancellation(token).ConfigureAwait(false))
        {
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

        IRecord? record = null;
        foreach (var item in command)
        {
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

        IRecord? record = null;
        await foreach (var item in command.WithCancellation(token).ConfigureAwait(false))
        {
            if (item is not null) record = item;
        }
        return record;
    }
}