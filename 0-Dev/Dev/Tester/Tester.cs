namespace Dev.Tester;

// ========================================================
public class Tester : MenuEntry
{
    public const string FactAttribute = nameof(FactAttribute);
    public const string EnforcedAttribute = nameof(EnforcedAttribute);
    public const BindingFlags Flags =
        BindingFlags.Instance | BindingFlags.Static |
        BindingFlags.Public | BindingFlags.NonPublic;

    public string Header => "Execute Solution Tests.";

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="includes"></param>
    /// <param name="excludes"></param>
    public Tester(ElementList includes, ElementList excludes)
    {
        Includes = includes.ThrowIfNull();
        Excludes = excludes.ThrowIfNull();
    }
    public ElementList Includes { get; }
    public ElementList Excludes { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void OnPrint() => WriteLine(Header);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void OnExecute()
    {
        WriteLine();
        WriteLine(Program.Color, Program.FatSeparator);
        WriteLine(Program.Color, Header);

        var assemblyHolders = Populate();
        PurgeExcludes(assemblyHolders);
        EnsureEnforced(assemblyHolders);
        RemoveEmpty(assemblyHolders);

        var breakOnError = false;
        Executor(breakOnError, assemblyHolders);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Populates the assemblies.
    /// </summary>
    AssemblyHolderList Populate() => Includes.Count == 0
        ? PopulateFromRoot()
        : PopulateFromIncludes();

    /// <summary>
    /// Populates the assemblies from the root directory.
    /// </summary>
    /// <returns></returns>
    AssemblyHolderList PopulateFromRoot()
    {
        var assemblyHolders = new AssemblyHolderList();
        var root = AppContext.BaseDirectory;
        WriteLine();
        Write(Program.Color, $"Populating from: {root} ");

        var files = _Directory.EnumerateFiles(root);
        foreach (var file in files)
        {
            Write(Color.Magenta, ".");
            var upper = file.ToUpper();
            if (!upper.EndsWith(".DLL") && !upper.EndsWith(".EXE")) continue;

            string? name = null;
            try
            {
                name = _Path.GetFileNameWithoutExtension(file);
                var assembly = Assembly.Load(new AssemblyName(name));

                if (assemblyHolders.Find(assembly) == null)
                {
                    var holder = new AssemblyHolder(assembly);
                    assemblyHolders.Add(holder);
                    holder.Populate();
                }
            }
            catch { Debug.WriteLine(Color.Red, $"*** Error loading assembly: {name}"); }
        }

        WriteLine();
        return assemblyHolders;
    }

    /// <summary>
    /// Populates the assemblies from the collection of explicit includes.
    /// </summary>
    /// <returns></returns>
    AssemblyHolderList PopulateFromIncludes()
    {
        var assemblyHolders = new AssemblyHolderList();
        WriteLine();
        Write(Program.Color, $"Populating from explicit includes ");

        foreach (var item in Includes)
        {
            Write(Color.Magenta, ".");

            var name = item.AssemblyName ?? throw new ArgumentException(
                $"Assembly name of explicit include cannot be null: {item}");

            var holder = assemblyHolders.Find(name);
            if (holder == null)
            {
                var assembly = Assembly.Load(new AssemblyName(name));
                holder = new AssemblyHolder(assembly);
                assemblyHolders.Add(holder);
                holder.Populate();
            }
        }

        WriteLine();
        return assemblyHolders;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Purges the explicit exclude specifications.
    /// </summary>
    void PurgeExcludes(AssemblyHolderList assemblyHolders)
    {
        if (Excludes.Count == 0) return;

        Element specs;
        WriteLine();
        Write(Program.Color, $"Purging explicit excludes ");

        foreach (var assemblyHolder in assemblyHolders.ToList())
        {
            Write(Color.Magenta, ".");

            specs = new Element(assemblyHolder.Name, null, null);
            foreach (var item in Excludes)
                if (specs.Match(item)) assemblyHolders.Remove(assemblyHolder);
        }

        foreach (var assemblyHolder in assemblyHolders)
        {
            foreach (var typeHolder in assemblyHolder.TypeHolders.ToList())
            {
                specs = new Element(assemblyHolder.Name, typeHolder.FullName, null);
                foreach (var item in Excludes)
                    if (specs.Match(item)) assemblyHolder.TypeHolders.Remove(typeHolder);
            }

            foreach (var typeHolder in assemblyHolder.TypeHolders)
            {
                foreach (var methodHolder in typeHolder.MethodHolders.ToList())
                {
                    specs = new Element(assemblyHolder.Name, typeHolder.FullName, methodHolder.Name);
                    foreach (var item in Excludes)
                        if (specs.Match(item)) typeHolder.MethodHolders.Remove(methodHolder);
                }
            }
        }

        WriteLine();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Ensures that only the decorated elements are taken into consideration, if any is
    /// decorated.
    /// </summary>
    /// <param name="assemblyHolders"></param>
    void EnsureEnforced(AssemblyHolderList assemblyHolders)
    {
        WriteLine();
        Write(Program.Color, $"Ensuring enforced elements ");

        foreach (var assemblyHolder in assemblyHolders)
        {
            Write(Color.Magenta, ".");
            assemblyHolder.EnsureEnforced();
        }
        WriteLine();

        bool areMethods = false;
        foreach (var assemblyHolder in assemblyHolders)
            foreach (var typeHolder in assemblyHolder.TypeHolders)
                foreach (var methodHolder in typeHolder.MethodHolders)
                    if (methodHolder.Enforced) areMethods = true;

        if (areMethods) PurgeNotEnforced(assemblyHolders);
    }

    /// <summary>
    /// Purges not enforced elements.
    /// </summary>
    /// <param name="assemblyHolders"></param>
    void PurgeNotEnforced(AssemblyHolderList assemblyHolders)
    {
        WriteLine();
        Write(Program.Color, $"Purging not enforced elements ");

        foreach (var assemblyHolder in assemblyHolders)
        {
            Write(Color.Magenta, ".");
            assemblyHolder.PurgeNotEnforced();
        }
        var items = assemblyHolders.Where(x => x.TypeHolders.Count == 0).ToList();
        foreach (var item in items) assemblyHolders.Remove(item);

        WriteLine();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes empty elements.
    /// </summary>
    /// <param name="assemblyHolders"></param>
    void RemoveEmpty(AssemblyHolderList assemblyHolders)
    {
        foreach (var assemblyHolder in assemblyHolders)
            RemoveEmpty(assemblyHolder.TypeHolders);

        var items = assemblyHolders.Where(x => x.TypeHolders.Count == 0).ToList();
        foreach (var item in items) assemblyHolders.Remove(item);
    }

    void RemoveEmpty(TypeHolderList typeHolders)
    {
        var items = typeHolders.Where(x => x.MethodHolders.Count == 0).ToList();
        foreach (var item in items) typeHolders.Remove(item);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Executes the tests.
    /// </summary>
    /// <param name="assemblyHolders"></param>
    void Executor(bool breakOnError, AssemblyHolderList assemblyHolders)
    {
        var num = 0;
        var ts = TimeSpan.Zero;

        foreach (var assemblyHolder in assemblyHolders)
        {
            foreach (var typeHolder in assemblyHolder.TypeHolders)
            {
                var type = typeHolder.Type;
                var instance = type.IsStatic() ? null : Activator.CreateInstance(type);

                foreach (var methodHolder in typeHolder.MethodHolders)
                {
                    WriteLine();
                    WriteLine(Program.Color, Program.FatSeparator);
                    WriteLine(Program.Color, $"{typeHolder.FullName}.{methodHolder}");
                    WriteLine(Program.Color, Program.FatSeparator);

                    var span = Executor(breakOnError, instance, methodHolder.Method);
                    WriteLine();
                    PrintResults(span);

                    ts = ts.Add(span);
                    num++;
                }

                if (instance != null &&
                    instance is IDisposable disposable) disposable.Dispose();
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
                Task.Run(async () => {
                    var item = method.Invoke(host, null);
                    var task = (ValueTask)item!;
                    await task;
                })
                .Wait();
            }
            else if (method.ReturnType.IsAssignableTo(typeof(Task)))
            {
                Task.Run(async () => {
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

        WriteLine();
        Write("Number of tests executed: ");
        WriteLine(Color.Cyan, num.ToString());
        PrintResults(ts);
        WriteLine();

        WriteLine(Program.Color, Program.FatSeparator);
    }
}