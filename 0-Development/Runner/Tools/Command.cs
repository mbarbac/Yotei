namespace Runner;

// ========================================================
/// <summary>
/// Facilitates the execution of system commands.
/// </summary>
public static class Command
{
    /// <summary>
    /// Executes a system command with the given optional arguments and working directory.
    /// Returns an integer with the result of that execution.
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