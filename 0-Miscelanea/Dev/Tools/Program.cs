namespace Dev.Tools;

// ========================================================
/// <summary>
/// Base class for command line programs.
/// </summary>
public abstract class Program
{
    public const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;
    public const string Separator = "******************************";

    /// <summary>
    /// Presents a menu where the user can select one among the given actions to be executed.
    /// A default <see cref="RunnerNone"/> one is injected as the first one to exit the menu.
    /// </summary>
    /// <param name="actions"></param>
    public static void MenuRun(params Runner[] actions)
    {
        actions = actions.ThrowIfNull();

        var items = new List<Runner> { new RunnerNone() };
        foreach (var action in actions) items.Add(action);

        var index = 0;
        var values = new List<int>();
        for (int i = 0; i < items.Count; i++) values.Add(items[i] is RunnerSeparator ? -1 : index++);

        while (true)
        {
            WriteLine();
            WriteLine(Color.Green, Separator);
            WriteLine(Color.Green, "Please select action to execute:");
            for (int i = 0; i < items.Count; i++)
            {
                if (values[i] >= 0) Write(Color.Green, $"-[{values[i],2}]: ");
                items[i].PrintHead();
            }
            WriteLine();

            while (true)
            {
                Write(Color.Green, "Enter action's index: ");

                var str = ReadLine();
                if (str != null && str.Trim().Length > 0)
                {
                    var num = int.TryParse(str, out var temp) ? temp : -1;
                    if (num < 0) continue;
                    if (num == 0) return;

                    index = values.IndexOf(num);
                    if (index >= 0)
                    {
                        WriteLine(Color.Green, Separator);
                        items[index].Execute();
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Captures a directory from the given value, allowing editting it or not as requested,
    /// and allowing empty ones as requested.
    /// </summary>
    /// <param name="header"></param>
    /// <param name="value"></param>
    /// <param name="edit"></param>
    /// <returns></returns>
    public static Directory CaptureDirectory(string header, string value, bool edit, bool empty)
    {
        header = header.NotNullNotEmpty();
        value = value.NullWhenEmpty() ?? string.Empty;

        WriteLine();
        WriteLine(Color.Green, Separator);

        while (true)
        {
            if (edit)
            {
                Write(Color.Green, $"{header}: "); Write(value);
                if (value.Length > 0) Write(Color.Green, " ==> ");

                var str = ReadLine();
                if (str != null) value = str.Trim();
                edit = true;
            }

            if (value.Length == 0 && empty) return Directory.Empty;
            if (_Directory.Exists(value))
            {
                WriteLine(Color.Green, Separator);
                return value;
            }
        }
    }

    /// <summary>
    /// Returns the default root directory.
    /// </summary>
    /// <returns></returns>
    public static Directory DefaultRoot()
    {
        var path = AppContext.BaseDirectory;
        while (true)
        {
            var files = _Directory.GetFiles(path, "*.sln");

            if (files.Length == 1) return path;
            if (files.Length == 0)
            {
                path = _Directory.GetParent(path)?.FullName;
                if (path == null) return Directory.Empty;
            }
            else Environment.FailFast($"Too many projects at: {path}");
        }
    }

    /// <summary>
    /// Returns the default exclusion directory.
    /// </summary>
    /// <returns></returns>
    public static Directory DefaultExclude()
    {
        var path = AppContext.BaseDirectory;
        while (true)
        {
            var files = _Directory.GetFiles(path, "*.csproj");

            if (files.Length == 1) return path;
            if (files.Length == 0)
            {
                path = _Directory.GetParent(path)?.FullName;
                if (path == null) return Directory.Empty;
            }
            else Environment.FailFast($"Too many projects at: {path}");
        }
    }
}