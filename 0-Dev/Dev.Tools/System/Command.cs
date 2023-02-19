namespace Dev;

// ========================================================
public static class Command
{
    /// <summary>
    /// Executes the command whose name and arguments are given, on the given working directory.
    /// Returns the exit code of that execution.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="arguments"></param>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static int Execute(string name, string? arguments = null, string? directory = null)
    {
        name = name.NotNullNotEmpty();
        arguments = arguments.NullWhenEmpty();
        directory = directory.NullWhenEmpty();

        var p = new Process();
        p.StartInfo.FileName = name;
        if (arguments != null) p.StartInfo.Arguments = arguments;
        if (directory != null) p.StartInfo.WorkingDirectory = directory;

        p.Start();
        p.WaitForExit();
        return p.ExitCode;
    }
}