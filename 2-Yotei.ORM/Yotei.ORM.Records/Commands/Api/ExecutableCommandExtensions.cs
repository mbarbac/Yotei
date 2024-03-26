namespace Yotei.ORM.Records;

// ========================================================
public static class ExecutableCommandExtensions
{
    /// <summary>
    /// Executes the associated command and returns the integer produced by that execution.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static int Execute(this IExecutableCommand command)
    {
        command.ThrowWhenNull();

        var iter = command.GetExecutor();
        return iter.Execute();
    }

    /// <summary>
    /// Executes the associated command and returns the integer produced by that execution.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<int> Execute(this IExecutableCommand command, CancellationToken token = default)
    {
        command.ThrowWhenNull();

        var iter = command.GetExecutor();
        return await iter.ExecuteAsync(token).ConfigureAwait(false);
    }
}