namespace Yotei.ORM.Records;

// ========================================================
public static class IEnumerableCommandExtensions
{
    /// <summary>
    /// Returns a new instance that wraps the source command but that, when enumerated, returns
    /// the results obtained from converting the original records using the given delegate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <returns></returns>
    public static IEnumerableCommand<T> WithConverter<T>(
        this IEnumerableCommand command,
        Func<IRecord, T> converter)
    {
        command.ThrowWhenNull();
        converter.ThrowWhenNull();

        return new EnumerableCommand<T>(command, converter);
    }
}