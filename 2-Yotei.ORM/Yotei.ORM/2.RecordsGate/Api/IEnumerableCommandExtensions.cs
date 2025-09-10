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
        using var iter = command.GetEnumerator();

        while (iter.MoveNext())
        {
            var item = iter.Current;
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

        await foreach (var item in command.WithCancellation(token))
            if (item is not null) list.Add(item);

        return list;
    }
    //{
    //    //

    //    //

    //    //await foreach (var item in command.WithCancellation(token))
    //    //{
    //    //}

    //    throw null;
    //    //await using var iter = command.GetAsyncEnumerator(token);
    //    //while (await iter.MoveNextAsync().with)
    //    //{
    //    //    var item = iter.Current;
    //    //    if (item is not null) list.Add(item);
    //    //}
    //    //return list;
    //}
}