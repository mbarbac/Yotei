namespace Yotei.ORM;

// ========================================================
public static class IExecutableCommandExtensions
{
    /// <summary>
    /// Executes this command, using its current state, and returns the integer produced by that
    /// execution.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static int Execute(this Records.IExecutableCommand command)
    {
        command.ThrowWhenNull();

        var iter = command.GetExecutor();
        return iter.Execute();
    }

    /// <summary>
    /// Executes this command, using its current state, and returns the integer produced by that
    /// execution.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async static ValueTask<int> ExecuteAsync(this Records.IExecutableCommand command, CancellationToken token = default)
    {
        command.ThrowWhenNull();

        var iter = command.GetExecutor();
        return await iter.ExecuteAsync(token).ConfigureAwait(false);
    }
}