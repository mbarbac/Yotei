namespace Dev.Tester;

// ========================================================
public class Tester : MenuEntry
{
    public const string FactAttribute = nameof(FactAttribute);
    public const string EnforcedAttribute = nameof(EnforcedAttribute);
    public const BindingFlags Flags =
        BindingFlags.Instance | BindingFlags.Static |
        BindingFlags.Public | BindingFlags.NonPublic;

    readonly ElementList Includes = new();
    readonly ElementList Excludes = new();
    readonly AssemblyHolderList AssemblyHolders = new();

    public string Header => "Execute Solution Tests.";

    /// <inheritdoc>
    /// </inheritdoc>
    public override void OnPrint() => WriteLine(Header);

    /// <inheritdoc>
    /// </inheritdoc>
    public override void OnExecute()
    {
        WriteLine();
        WriteLine(Program.Color, Program.FatSeparator);
        WriteLine(Program.Color, Header);

        Executor(breakOnError: true);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Prints the given results.
    /// </summary>
    /// <param name="ts"></param>
    void PrintResults(TimeSpan ts)
    {
        var ms = ts.TotalMilliseconds;
        Write("Total execution time: ");
        WriteLine(Color.Cyan, ms < 1000 ? $"{ms:#.00} ms" : $"{ms / 1000:#.000} sec");
    }

    /// <summary>
    /// Prints the given results.
    /// </summary>
    /// <param name="num"></param>
    /// <param name="ts"></param>
    void PrintResults(int num, TimeSpan ts)
    {
        WriteLine(Program.Color, Program.FatSeparator);
        WriteLine(Program.Color, "Execution summary...");
        WriteLine(Program.Color, Program.FatSeparator);
        Write("Number of tests executed: ");
        WriteLine(Color.Cyan, num.ToString());

        PrintResults(ts);
        WriteLine(Program.Color, Program.FatSeparator);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Executors the tests.
    /// </summary>
    /// <param name="breakOnError"></param>
    void Executor(bool breakOnError)
    {
        var listener = new ConsoleTraceListener();
        Trace.Listeners.Add(listener);
        Debug.IndentSize = 3;
        Debug.AutoFlush = true;

        if (Includes.Count == 0) PopulateFromRoot();
        else PopulateFromIncludes();

        PurgeExcludes();
        PurgeEmpties();
        EnsureEnforced();

        var num = 0;
        var ts = TimeSpan.Zero;

        // Assemblies...
        foreach (var assemblyHolder in AssemblyHolders)
        {
            // Types...
            foreach (var typeHolder in assemblyHolder.TypeHolders)
            {
                // Obtaining an instance, if needed...
                var type = typeHolder.Type;
                var instance = type.IsStatic()
                    ? null
                    : Activator.CreateInstance(type);

                // Methods...
                foreach (var methodHolder in typeHolder.MethodHolders)
                {
                    var method = methodHolder.MethodInfo;
                    WriteLine();
                    WriteLine(Program.Color, Program.FatSeparator);
                    WriteLine(Program.Color, $"{assemblyHolder.Name}.{typeHolder.Name}.{methodHolder.Name}");
                    WriteLine(Program.Color, Program.FatSeparator);

                    var span = Executor(breakOnError, instance, method);
                    ts = ts.Add(span);
                    num++;

                    PrintResults(span);
                }

                // Disposing the instance, if needed...
                if (instance is not null and
                    IDisposable disposable) disposable.Dispose();
            }
        }

        WriteLine();
        PrintResults(num, ts);
    }

    /// <summary>
    /// Executors the given method of the given host, or if it is null, a static one.
    /// </summary>
    /// <param name="breakOnError"></param>
    /// <param name="host"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    TimeSpan Executor(bool breakOnError, object? host, MethodInfo method)
    {
        var watch = new Stopwatch();
        watch.Start();
        try
        {
            if (method.ReturnType.IsAssignableTo(typeof(ValueTask)))
            {
                Task.Run(async () =>
                {
                    var item = method.Invoke(host, null);
                    var task = (ValueTask)item!;
                    await task;
                })
                .Wait();
            }
            else if (method.ReturnType.IsAssignableTo(typeof(Task)))
            {
                Task.Run(async () =>
                {
                    var item = method.Invoke(host, null);
                    var task = (Task)item!;
                    await task;
                })
                .Wait();
            }
            else
            {
                method.Invoke(host, null);
            }
        }
        catch (Exception e)
        {
            WriteLine();
            WriteLine(Color.Red, Program.FatSeparator);
            WriteLine(Color.Red, e.ToDisplayString());
            WriteLine(Color.Red, Program.FatSeparator);
            WriteLine();

            if (breakOnError)
            {
                WriteLine();
                WriteLine("Executor cannot proceed further and will finish.");
                Environment.FailFast(null);
            }
        }
        finally { watch.Stop(); }

        return watch.Elapsed;
    }

    // ----------------------------------------------------

    // <summary>
    /// Populates the tests to run from the root directory.
    /// </summary>
    void PopulateFromRoot()
    {
        var root = AppContext.BaseDirectory;
        WriteLine();
        Write(Program.Color, $"Populating from: {root} ");

        var files = System.IO.Directory.EnumerateFiles(root);
        foreach (var file in files)
        {
            var upper = file.ToUpper();
            if (!upper.EndsWith(".DLL") && !upper.EndsWith(".EXE")) continue;

            Write(Color.Magenta, ".");
            var name = Path.GetFileNameWithoutExtension(file);

            try
            {
                var holder = AssemblyHolders.Find(name);
                if (holder == null)
                {
                    var assembly = Assembly.Load(new AssemblyName(name));
                    holder = AssemblyHolders.Add(assembly);
                }

                holder.Populate();
            }
            catch { }
        }

        WriteLine();
    }

    /// <summary>
    /// Populates the tests to run from the explicit includes.
    /// </summary>
    void PopulateFromIncludes()
    {
        WriteLine();
        Write(Program.Color, "Populating explicit includes: ");

        foreach (var item in Includes)
        {
            Write(Color.Magenta, ".");

            var name = item.AssemblyName;
            var holder = AssemblyHolders.Find(name);

            if (holder == null)
            {
                var assembly = Assembly.Load(new AssemblyName(name));
                holder = AssemblyHolders.Add(assembly);
            }

            holder.Populate(item.TypeName, item.MethodName);
        }

        WriteLine();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes the exclusions from the collection of tests to run.
    /// </summary>
    void PurgeExcludes()
    {
        WriteLine();
        Write(Program.Color, "Purging explicit includes: ");

        foreach (var item in Excludes)
        {
            Write(Color.Magenta, ".");

            var holder = AssemblyHolders.Find(item.AssemblyName);
            holder?.Purge(item.TypeName, item.MethodName);
        }

        WriteLine();
    }

    /// <summary>
    /// Removes the empty elements.
    /// </summary>
    void PurgeEmpties()
    {
        var temps = new List<AssemblyHolder>();
        foreach (var assemblyHolder in AssemblyHolders)
        {
            PurgeEmpties(assemblyHolder);
            if (assemblyHolder.TypeHolders.Count == 0) temps.Add(assemblyHolder);
        }

        foreach (var assemblyHolder in temps) AssemblyHolders.Remove(assemblyHolder);
    }

    /// <summary>
    /// Removes the empty elements from the given holder hierarchy.
    /// </summary>
    /// <param name="assemblyHolder"></param>
    void PurgeEmpties(AssemblyHolder assemblyHolder)
    {
        var temps = new List<TypeHolder>();
        foreach (var typeHolder in assemblyHolder.TypeHolders)
            if (typeHolder.MethodHolders.Count == 0) temps.Add(typeHolder);

        foreach (var typeHolder in temps) assemblyHolder.TypeHolders.Remove(typeHolder);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Ensures that, if an 'Enforced' attribute is used, then only the test classes and methods
    /// decorates with it are Executord.
    /// </summary>
    void EnsureEnforced()
    {
        WriteLine();
        Write(Program.Color, "Ensuring enforced elements: ");

        EnsureEnforcedTypes();
        EnsureEnforcedMethods();
        WriteLine();
    }
    /// <summary>
    /// Ensures enforced types only, if needed.
    /// </summary>
    void EnsureEnforcedTypes()
    {
        if (!AreEnforcedTypes()) return;

        foreach (var assemblyHolder in AssemblyHolders)
        {
            var items = assemblyHolder.TypeHolders.Where(x => !x.IsEnforced).ToList();
            foreach (var item in items) assemblyHolder.TypeHolders.Remove(item);
        }
        PurgeEmpties();
    }

    /// <summary>
    /// Determines if there are enforced types, or not.
    /// </summary>
    /// <returns></returns>
    bool AreEnforcedTypes()
    {
        foreach (var assemblyHolder in AssemblyHolders)
            foreach (var typeHolder in assemblyHolder.TypeHolders)
                if (typeHolder.IsEnforced) return true;

        return false;
    }

    /// <summary>
    /// Ensures enforced methods only, if needed.
    /// </summary>
    void EnsureEnforcedMethods()
    {
        if (!AreEnforcedMethods()) return;

        foreach (var assemblyHolder in AssemblyHolders)
            foreach (var typeHolder in assemblyHolder.TypeHolders)
                if (typeHolder.MethodHolders.Any(x => x.IsEnforced))
                    typeHolder.IsEnforced = true;
        EnsureEnforcedTypes();

        foreach (var assemblyHolder in AssemblyHolders)
        {
            foreach (var typeHolder in assemblyHolder.TypeHolders)
            {
                var items = typeHolder.MethodHolders.Where(x => !x.IsEnforced).ToList();
                foreach (var item in items) typeHolder.MethodHolders.Remove(item);
            }
        }
        PurgeEmpties();
    }

    /// <summary>
    /// Determines if there are enforced methods, or not.
    /// </summary>
    /// <returns></returns>
    bool AreEnforcedMethods()
    {
        foreach (var assemblyHolder in AssemblyHolders)
            if (AreEnforcedMethods(assemblyHolder)) return true;

        return false;
    }

    /// <summary>
    ///  Determines if there are enforced methods in the given assembly hierarchy, or not.
    /// </summary>
    /// <param name="assemblyHolder"></param>
    /// <returns></returns>
    bool AreEnforcedMethods(AssemblyHolder assemblyHolder)
    {
        foreach (var typeHolder in assemblyHolder.TypeHolders)
            if (AreEnforcedMethods(typeHolder)) return true;

        return false;
    }

    /// <summary>
    /// Determines if there are enforced methods in the given type hierarchy, or not.
    /// </summary>
    /// <param name="typeHolder"></param>
    /// <returns></returns>
    bool AreEnforcedMethods(TypeHolder typeHolder)
    {
        return typeHolder.MethodHolders.Any(x => x.IsEnforced);
    }
}