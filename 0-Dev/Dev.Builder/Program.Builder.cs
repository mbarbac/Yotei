namespace Dev.Builder;

// ========================================================
internal enum RunMode
{
    None,
    Debug_PushLocal,
    Debug_Compile_PushLocal,
    Debug_UpdateVersion_Compile_PushLocal_UpdateProjects,
}

// ========================================================
/// <summary>
/// Represents this program.
/// </summary>
internal class Program
{
    static StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

    public const string FatSeparator = "******************************";
    public const string SlimSeparator = "--------------------";
    public const string NuGetExe = @"C:\Dev\Nuget.exe";
    public const string CompileExe = "dotnet";
    public const string BetaDefault = "v0001";

    /// <summary>
    /// The program entry point.
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        var root = CaptureRootDirectory(args);
        var exclude = CaptureExcludeDirectory(args);

        var projects = root.FindProjects(exclude);
        var packables = projects.SelectPackables();

        packables = packables.OrderByDependencies();
        projects = projects.RemovePackables(packables);

        var actions = CaptureActions(packables);
        for (int i = 0; i < actions.Count; i++)
        {
            var action = actions[i]; if (action == RunMode.None) continue;
            var packable = packables[i];

            WriteLine();
            WriteLine(Color.Green, FatSeparator);
            Write(Color.Green, "Executing: "); WriteLine(Color.Yellow, action.ToString());
            Write(Color.Green, "On Packable: "); WriteLine(packable.Name);
            WriteLine(Color.Green, FatSeparator);

            switch (action)
            {
                case RunMode.Debug_PushLocal:
                    Debug_PushLocal(packable);
                    break;

                case RunMode.Debug_Compile_PushLocal:
                    Debug_Compile_PushLocal(packable);
                    break;

                case RunMode.Debug_UpdateVersion_Compile_PushLocal_UpdateProjects:
                    Debug_UpdateVersion_Compile_PushLocal_UpdateProjects(packable);
                    break;
            }
        }

        WriteLine();
        WriteLine(Color.Green, FatSeparator);
        WriteLine(Color.Green, "Press [Enter] to finalize...");
        ReadLine();

        /// <summary> Executes the requested action on the given packable...
        /// </summary>
        void Debug_PushLocal(Packable packable)
        {
            var files = packable.Push(PushMode.Local);
            files.Delete(true);
        }

        /// <summary> Executes the requested action on the given packable...
        /// </summary>
        void Debug_Compile_PushLocal(Packable packable)
        {
            var done = packable.Compile(CompileMode.Debug);
            if (done) Debug_PushLocal(packable);
        }

        /// <summary> Executes the requested action on the given packable...
        /// </summary>
        void Debug_UpdateVersion_Compile_PushLocal_UpdateProjects(Packable packable)
        {
            /*var updated = packable.UpdateVersion(SemanticOptions.Beta, BetaDefault);
            if (updated != null)
            {
                var done = packable.File.CompileProject(CompileMode.Debug);
                if (done)
                {
                    Debug_PushLocal(packable);

                    var temps = projects.UpdateReferences(updated);
                    foreach (var temp in temps) temp.CompileProject(CompileMode.Debug);

                    var items = temps.SelectPackables();
                    foreach (var item in items) Debug_PushLocal(item);
                }
            }*/
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Captures the root directory, potentially the first command argument.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    static Directory CaptureRootDirectory(string[] args)
    {
        var root = args.Length > 0 ? args[0] : string.Empty;
        while (true)
        {
            WriteLine();
            WriteLine(Color.Green, FatSeparator);
            Write(Color.Green, "Root Directory: "); WriteLine(root);
            Write(Color.Green, "[Enter] to accept, or override with: ");

            var str = ReadLine();
            if (str != null) root = str.Trim();

            if (root.Length > 0 && _Directory.Exists(root))
            {
                Write(Color.Green, "Captured root directory: "); WriteLine(root);
                WriteLine(Color.Green, FatSeparator);
                return new Directory(root);
            }
            else WriteLine(Color.Green, FatSeparator);
        }
    }

    /// <summary>
    /// Captures the root directory, potentially the second command argument.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    static Directory CaptureExcludeDirectory(string[] args)
    {
        var root = args.Length > 1 ? args[1] : GetExclude().Value;
        while (true)
        {
            WriteLine();
            WriteLine(Color.Green, FatSeparator);
            Write(Color.Green, "Exclude Directory: "); WriteLine(root);
            Write(Color.Green, "[Enter] to accept, or override with: ");

            var str = ReadLine();
            if (str != null) root = str.Trim();

            if (root.Length > 0 && _Directory.Exists(root))
            {
                Write(Color.Green, "Captured root directory: "); WriteLine(root);
                WriteLine(Color.Green, FatSeparator);
                return new Directory(root);
            }
            else WriteLine(Color.Green, FatSeparator);
        }

        /// <summary> Invoked to obtain the default exclusion directory...
        /// </summary>
        static Directory GetExclude()
        {
            var path = AppContext.BaseDirectory;
            while (true)
            {
                var files = _Directory.GetFiles(path, "*.csproj");

                if (files.Length == 1) return new Directory(path);
                if (files.Length == 0)
                {
                    path = _Directory.GetParent(path)?.FullName!;
                    if (path == null) return Directory.Empty;
                }
                else Environment.FailFast($"Too many projects at: {path}");
            }
        }
    }

    /// <summary>
    /// Gets the collection of actions to execute with each of the given packable projects.
    /// </summary>
    /// <param name="packables"></param>
    /// <returns></returns>
    static ImmutableList<RunMode> CaptureActions(ImmutableList<Packable> packables)
    {
        packables = packables.ThrowIfNull();

        var names = Enum.GetNames<RunMode>();
        var values = Enum.GetValues<RunMode>();

        WriteLine();
        WriteLine(Color.Green, FatSeparator);
        WriteLine(Color.Green, "Please select the action for the following packable projects:");
        DisplayActions();
        WriteLine(Color.Green, FatSeparator);

        var array = new RunMode[packables.Count];
        for (int i = 0; i < packables.Count; i++) array[i] = RunMode.None;

        for (int i = 0; i < packables.Count; i++)
        {
            var packable = packables[i];
            var mode = CaptureAction(packable, true);
            array[i] = mode;
        }

        return array.ToImmutableList();

        /// <summary> Invoked to capture the requested action for the given packable...
        /// </summary>
        RunMode CaptureAction(Packable packable, bool header)
        {
            WriteLine();
            while (true)
            {
                if (header)
                {
                    Write(Color.Green, "Action for: ");
                    Write(packable.Name);
                    Write(Color.Green, " : ");
                    header = false;
                }

                var keyinfo = ReadKey(TimeSpan.FromSeconds(10));
                if (keyinfo == null)
                {
                    WriteLine();
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
                if (num >= 0 && num < names!.Length)
                {
                    var value = values[num]; WriteLine(Color.Yellow, value.ToString());
                    return value;
                }
            }
        }

        /// <summary> Invoked to display the available actions...
        /// </summary>
        void DisplayActions()
        {
            for (int i = 0; i < names!.Length; i++)
            {
                Write($"- [{i}]: "); WriteLine(Color.Green, names[i]);
            }
            Write("- [?]: "); WriteLine(Color.Green, "Display this options again.");
        }
    }
}