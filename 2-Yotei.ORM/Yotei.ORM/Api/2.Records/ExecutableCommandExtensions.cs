namespace Yotei.ORM;

// ========================================================
public static class ExecutableCommandExtensions
{
    /// <summary>
    /// Returns the integer produced by the execution of the associated command.
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
    /// Returns the integer produced by the execution of the associated command.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async ValueTask<int> ExecuteAsync(
        this IExecutableCommand command, CancellationToken token = default)
    {
        command.ThrowWhenNull();

        var iter = command.GetExecutor();
        return await iter.ExecuteAsync(token).ConfigureAwait(false);
    }
}