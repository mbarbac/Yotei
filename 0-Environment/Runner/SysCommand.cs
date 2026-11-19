namespace Runner;

// ========================================================
/// <summary>
/// Used to execute system commands.
/// </summary>
public static class SysCommand
{
    /// <summary>
    /// Executes the given command using the optional arguments and working directory, if any.
    /// Returns the integer resulting from that execution.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="arguments"></param>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static int Execute(
        string command,
        string? arguments = null,
        string? directory = null)
    {
        command = command.NotNullNotEmpty(trim: true);
        arguments = arguments.NullWhenEmpty(trim: true);
        directory = directory.NullWhenEmpty(trim: true);

        var p = new Process();
        p.StartInfo.FileName = command;
        if (arguments != null) p.StartInfo.Arguments = arguments;
        if (directory != null) p.StartInfo.WorkingDirectory = directory;

        p.Start();
        p.WaitForExit();
        return p.ExitCode;
    }
}