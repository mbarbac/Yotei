namespace Runner;

// ========================================================
/// <summary>
/// Facilitates the execution of system commands.
/// </summary>
public static class Command
{
    /// <summary>
    /// Executes the given command using the given optional argument if any, and on the given
    /// working directory, if any, or the actual one otherwise. Returns an <c>int</c> with the
    /// result of that execution, which typically is <c>0</c> to indicate success.
    /// </summary>
    /// <param name="commandName"></param>
    /// <param name="arguments"></param>
    /// <param name="workingDirectory"></param>
    /// <returns></returns>
    public static int Execute(
        string commandName,
        string? arguments = null,
        string? workingDirectory = null)
    {
        commandName = commandName.NotNullNotEmpty();
        arguments = arguments.NullWhenEmpty();
        workingDirectory = workingDirectory.NullWhenEmpty();

        var p = new Process();
        p.StartInfo.FileName = commandName;
        if (arguments != null) p.StartInfo.Arguments = arguments;
        if (workingDirectory != null) p.StartInfo.WorkingDirectory = workingDirectory;

        p.Start();
        p.WaitForExit();
        return p.ExitCode;
    }
}