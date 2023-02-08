namespace Dev.Builder;

// ========================================================
/// <summary>
/// Represents this program.
/// </summary>
internal class Program
{
    public const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

    public const string FatSeparator = "******************************";
    public const string SlimSeparator = "--------------------";
    public const string NuGetExe = @"C:\Dev\Nuget.exe";
    public const string CompileExe = "dotnet";

    /// <summary>
    /// The program entry point.
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        var root = CaptureRootDirectory(args);
        var exclude = CaptureExcludeDirectory(args);
        var projects = root.FindProjects(exclude);
        var packables = projects.SelectPackables().OrderByDependencies();

        WriteLine();
        foreach (var packable in packables)
        {
            var action = GetAction(packable);
            switch (action)
            {
                case RunMode.PushLocal: PushLocal(packable); break;
                case RunMode.PushLocalIncreaseVersion: PushLocalIncreaseVersion(packable); break;
            }
        }

        WriteLine();
        WriteLine(Color.Green, FatSeparator);
        WriteLine(Color.Green, "Press [Enter] to finalize...");
        ReadLine();

        /// <summary>
        /// Pushes to local the files on disk of the given package.
        /// </summary>
        void PushLocal(Packable packable)
        {
            packable.Push(PushMode.Local, deleteOld: true);
        }

        /// <summary>
        /// Pushes to local the files on disk of the given package.
        /// Increases its version.
        /// Updates the references to it in all projects.
        /// </summary>
        void PushLocalIncreaseVersion(Packable packable)
        {
            var done = packable.Push(PushMode.Local, deleteOld: true);
            if (!done)
            {
                WriteLine(Color.Red, $"Could not push package files.");
                return;
            }

            var version = packable.Version.Increase(SemanticOptions.BetaExpand, out _);

            WriteLine();
            Write(Color.Green, "Increasing version of: ");
            Write($"{packable.Project.File.Name}, {packable.Version}");
            Write(Color.Green, " To: ");
            WriteLine(version);

            done = packable.Project.ResetVersion(version);
            if (!done)
            {
                WriteLine(Color.Red, $"Could not found version specification.");
                return;
            }

            var reference = new PackageReference(packable.Project.File.Name, packable.Version);
            foreach (var project in projects)
                project.UpdateReference(reference, version);
        }
    }

    // ====================================================

    enum RunMode
    {
        None,
        PushLocal,
        PushLocalIncreaseVersion,
    }

    /// <summary>
    /// Gets the action to execute with the given packable.
    /// </summary>
    /// <param name="packable"></param>
    static RunMode GetAction(Packable packable)
    {
        var names = Enum.GetNames<RunMode>();
        var values = Enum.GetValues<RunMode>();

        WriteLine();
        WriteLine(Color.Green, FatSeparator);
        DisplayActions();

        var header = true;
        while (true)
        {
            if (header)
            {
                WriteLine();
                Write(Color.Green, "Select action for: "); Write($"{packable} : ");
                header = false;
            }

            var keyinfo = ReadKey(TimeSpan.FromSeconds(10));
            if (keyinfo == null)
            {
                WriteLine(Color.Cyan, $"{RunMode.None}");
                return RunMode.None;
            }

            var chr = keyinfo.Value.KeyChar;
            if (chr == '?')
            {
                WriteLine(); DisplayActions();
                header = true;
                continue;
            }

            var num = chr - '0';
            if (num >= 0 && num < names.Length)
            {
                var value = values[num]; WriteLine(Color.Cyan, value.ToString());
                return value;
            }
        }        

        /// <summary> Invoked to display the available actions...
        /// </summary>
        void DisplayActions()
        {
            for (int i = 0; i < names!.Length; i++)
            {
                Write($"- [{i}]: ");
                WriteLine(Color.Green, names[i]);
            }
            WriteLine();
            Write("- [?]: "); WriteLine(Color.Green, "Display this options again.");
        }
    }

    // ====================================================

    /// <summary>
    /// Captures the root directory.
    /// </summary>
    static Directory CaptureRootDirectory(string[] args)
    {
        WriteLine();
        WriteLine(Color.Green, FatSeparator);

        var path = args.Length > 0 ? args[0] : string.Empty;
        var asked = false;

        while (true)
        {
            if (asked && _Directory.Exists(path))
            {
                WriteLine(Color.Green, FatSeparator);
                return path;
            }

            asked = true;
            Write(Color.Green, "Root directory: "); Write(path);
            Write(Color.Green, " ==> ");

            var str = ReadLine();
            if (str != null) path = str.Trim();
        }
    }

    /// <summary>
    /// Captures the root directory.
    /// </summary>
    static Directory CaptureExcludeDirectory(string[] args)
    {
        WriteLine();
        WriteLine(Color.Green, FatSeparator);

        var path = args.Length > 1 ? args[1] : string.Empty;
        var empty = path.Length == 0;
        var asked = false;

        if (empty) path = GetExclude();

        while (true)
        {
            if (!empty && _Directory.Exists(path))
            {
                if (!asked) { Write(Color.Green, "Exclude directory: "); WriteLine(path); }
                WriteLine(Color.Green, FatSeparator);
                return path;
            }

            if (empty) { path = GetExclude(); empty = false; }

            asked = true;
            Write(Color.Green, "Exclude directory: "); Write(path);
            Write(Color.Green, " ==> ");

            var str = ReadLine();
            if (str != null) path = str.Trim();
        }

        /// <summary> Gets the default exclusion directory...
        /// </summary>
        static Directory GetExclude()
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
}