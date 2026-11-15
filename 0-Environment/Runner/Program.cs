using System.Runtime.InteropServices.Marshalling;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
internal class Program
{
    public static readonly bool ToDebug = true;
    public static readonly TimeSpan Timeout = TimeSpan.FromMinutes(10);
    public static readonly string FatSeparator = new('*', 50);
    public static readonly string SlimSeparator = new('-', 35);

    // ----------------------------------------------------

    /// <summary>
    /// Program entry point.
    /// </summary>
    [SuppressMessage("", "IDE0079")]
    [SuppressMessage("", "IDE0210")]
    static void Main()
    {
        Trace.Listeners.EnsureConsoleListener();
        Debug.IndentSize = 2;
        Debug.AutoFlush = true;

        var menu = new ConsoleMenu { ToDebug = ToDebug, Timeout = Timeout }
        .Add(new("Exit"))
        .Add(new("Execute Tests"))
        .Add(new("Manage Disk Artifacts"))
        .Add(new("Manage Package Versions"));

        int position = 0; do
        {
            Console.PreventBufferOverflow(menu.Count + 5);
            Console.WriteLineEx(true);
            Console.WriteLineEx(true, Green, FatSeparator);
            Console.WriteLineEx(true, Green, "Main Menu");
            Console.WriteLineEx(true);
            position = menu.Run(position);
        }
        while (position > 0);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the path's full name to the solution's directory...
    /// </summary>
    static internal string GetSolutionDirectory()
    {
        var path = AppContext.BaseDirectory;
        var dir = new DirectoryInfo(path);

        while (true)
        {
            var files = dir.GetFiles("*.slnx");
            if (files.Length > 0) return dir.FullName;

            dir = dir.Parent!;
            if (dir == null) Environment.FailFast("Cannot find solutions's directory.");
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Edits in the console the given directory's path. Returns <c>null</c> if it was cancelled
    /// or timeout expired. Retries while not a valid one is entered.
    /// </summary>
    static internal string? EditDirectory(string? path, string? description = null)
    {
        while (true)
        {
            if (description != null) Console.WriteEx(true, Green, description);

            path = Console.EditLineEx(true, path, Timeout);
            path = path.NullWhenEmpty(trim: true);
            if (path is null) return null;

            try
            {
                var dir = new DirectoryInfo(path);
                if (dir.Exists) return dir.FullName;
                Console.WriteLineEx(true, Red, "Invalid path, please retry.");
            }
            catch (FileNotFoundException) { }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Captures the desired build mode, starting from the given value.
    /// </summary>
    static internal bool CaptureBuildMode(ref BuildMode mode)
    {
        var menu = new ConsoleMenu { ToDebug = ToDebug, Timeout = Timeout }
        .Add(new("Exit"))
        .Add(new("Debug"))
        .Add(new("Local"))
        .Add(new("Release"));

        var position = mode switch
        {
            BuildMode.Debug => 1,
            BuildMode.Local => 2,
            BuildMode.Release => 3,
            _ => 0
        };

        position = menu.Run(position); switch (position)
        {
            case 1: mode = BuildMode.Debug; return true;
            case 2: mode = BuildMode.Local; return true;
            case 3: mode = BuildMode.Release; return true;
        }
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a collection with the project files found at the given root directory and its
    /// subfolders, provided they are not part of the exclusion branch.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="exclude"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    static internal List<ProjectFile> FindProjects(
        string root,
        string? exclude = null,
        bool ignoreCase = true)
    {
        root = root.NotNullNotEmpty(trim: true);
        exclude = exclude?.NotNullNotEmpty(trim: true);

        var list = new List<ProjectFile>();
        Populate(root);
        return list;

        /// <summary>
        /// Recursively populates the list of projects, from the given path.
        /// </summary>
        void Populate(string path)
        {
            if (path.Contains(".git", ignoreCase)) return;
            if (path.Contains(".vs", ignoreCase)) return;
            if (path.Contains("\\bin\\", ignoreCase)) return;
            if (path.Contains("\\obj\\", ignoreCase)) return;
            if (exclude is not null && path.StartsWith(exclude, ignoreCase)) return;

            var dir = new DirectoryInfo(path);
            if (!dir.Exists) return;

            var files = dir.GetFiles("*.csproj");
            foreach (var file in files)
            {
                var name = file.FullName;
                var index = list.FindIndex(x => string.Compare(x.FullName, name, ignoreCase) == 0);
                if (index < 0) list.Add(new(file.FullName));
            }

            var dirs = dir.GetDirectories();
            foreach (var temp in dirs) Populate(temp.FullName);
        }
    }
}
