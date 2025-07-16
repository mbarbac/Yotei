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

        List<IRecord> list = [];

        using var iter = command.GetEnumerator();
        foreach (var item in iter)
            if (item is not null) list.Add(item);

        return list;
    }

    /// <summary>
    /// Returns a list with the results produced by the execution of this command.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async ValueTask<List<IRecord>> ToListAsync(
        this IEnumerableCommand command, CancellationToken token = default)
    {
        command.ThrowWhenNull();

        List<IRecord> list = [];

        await using var iter = command.GetAsyncEnumerator(token);
        await foreach (var item in iter.WithCancellation(token))
            if (item is not null) list.Add(item);

        return list;
    }
}