using static System.ConsoleColor;
using static Yotei.Tools.Diagnostics.ConsoleWrapper;
using Debug = Yotei.Tools.Diagnostics.DebugWrapper;

namespace Runner.Tester;

// ========================================================
public class MenuTester(bool breakOnError) : MenuEntry
{
    /// <summary>
    /// To break when a test fails, or to continue executing otherwise.
    /// </summary>
    public bool BreakOnError { get; } = breakOnError;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string Header() => "Execute Tests";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Execute()
    {
        WriteLine(true);
        WriteLine(true, Green, Program.FatSeparator);
        WriteLine(true, Green, Header());

        var assemblyHolders = new AssemblyHolderList();
        Populate(assemblyHolders);
        PurgeExcludes(assemblyHolders);
        PurgeNotEnforcedMethods(assemblyHolders);
        PurgeNotEnforcedTypes(assemblyHolders);
        PurgeEmpty(assemblyHolders);

        Execute(assemblyHolders);
    }

    // ====================================================

    /// <summary>
    /// Populates the list of known assemblies.
    /// </summary>
    /// <param name="assemblyHolders"></param>
    static void Populate(AssemblyHolderList assemblyHolders)
    {
        if (Program.Includes.Count == 0) PopulateFromRoot(assemblyHolders);
        else PopulateFromIncludes(assemblyHolders);
    }

    /// <summary>
    /// Populates the list of known assemblies from the solution's root directory.
    /// </summary>
    /// <param name="assemblyHolders"></param>
    static void PopulateFromRoot(AssemblyHolderList assemblyHolders)
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);

        WriteLine(true);
        Write(true, Green, "Populating from root directory: "); WriteLine($"{root.FullName}");

        var files = root.GetFiles();
        foreach (var file in files)
        {
            var upper = file.Name.ToUpper();
            if (!upper.EndsWith(".DLL") && !upper.EndsWith(".EXE")) continue;

            try
            {
                var assembly = Assembly.LoadFrom(file.FullName);

                if (assemblyHolders.Find(assembly) == null)
                {
                    var holder = new AssemblyHolder(assembly);
                    holder.Populate();

                    if (holder.TypeHolders.Count > 0)
                    {
                        assemblyHolders.Add(holder);
                        WriteLine(file.Name);
                    }
                }
            }
            catch { }
        }
    }

    /// <summary>
    /// Populates the list of known assemblies from the list of explicit includes.
    /// </summary>
    /// <param name="assemblyHolders"></param>
    static void PopulateFromIncludes(AssemblyHolderList assemblyHolders)
    {
        WriteLine(true);
        WriteLine(true, Green, "Populating from includes: ");

        foreach (var item in Program.Includes)
        {
            WriteLine(true, item.ToString());

            var name = item.AssemblyName ?? throw new ArgumentException(
                "Assembly name of an explicit include cannot be null.")
                .WithData(item);

            if (!name.EndsWith(".DLL", StringComparison.OrdinalIgnoreCase))
                name += ".DLL";

            var assembly = Assembly.LoadFrom(name);

            if (assemblyHolders.Find(assembly) == null)
            {
                var holder = new AssemblyHolder(assembly);
                holder.Populate();

                if (holder.TypeHolders.Count > 0)
                {
                    assemblyHolders.Add(holder);
                }
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the list of known assemblies, and their childs, the explicit excludes,
    /// if any.
    /// </summary>
    /// <param name="assemblyHolders"></param>
    static void PurgeExcludes(AssemblyHolderList assemblyHolders)
    {
        if (Program.Excludes.Count == 0) return;

        WriteLine(true);
        WriteLine(true, Green, "Purging explicit excludes: ");

        foreach (var item in Program.Excludes)
        {
            WriteLine(true, item.ToString());

            foreach (var holder in assemblyHolders.ToList())
            {
                if (item.AssemblyName != null)
                {
                    if (string.Compare(item.AssemblyName, holder.Name, ignoreCase: true) == 0)
                    {
                        if (item.TypeName == null && item.MethodName == null)
                        {
                            assemblyHolders.Remove(holder);
                            continue;
                        }
                        else
                        {
                            holder.PurgeExcludes(item);
                        }
                    }
                }
                else
                {
                    holder.PurgeExcludes(item);
                }
            }
        }
    }

    /// <summary>
    /// Invoked to purge not-enforced methods, if any was enforced.
    /// </summary>
    /// <param name="assemblyHolders"></param>
    static void PurgeNotEnforcedMethods(AssemblyHolderList assemblyHolders)
    {
        WriteLine(true);
        WriteLine(true, Green, "Purging not enforced methods...");

        if (assemblyHolders.Any(x => x.HasEnforcedMethods()))
        {
            foreach (var holder in assemblyHolders.ToList())
            {
                holder.PurgeNotEnforcedMethods();
                if (holder.TypeHolders.Count == 0) assemblyHolders.Remove(holder);
            }
        }
    }

    /// <summary>
    /// Invoked to purge not-enforced types, if any was enforced.
    /// </summary>
    /// <param name="assemblyHolders"></param>
    static void PurgeNotEnforcedTypes(AssemblyHolderList assemblyHolders)
    {
        WriteLine(true);
        WriteLine(true, Green, "Purging not enforced types... ");

        if (assemblyHolders.Any(x => x.HasEnforcedTypes()))
        {
            foreach (var holder in assemblyHolders.ToList())
            {
                holder.PurgeNotEnforcedTypes();
                if (holder.TypeHolders.Count == 0) assemblyHolders.Remove(holder);
            }
        }
    }

    /// <summary>
    /// Invoked to purge the elements with no tests classes or methods.
    /// </summary>
    /// <param name="assemblyHolders"></param>
    static void PurgeEmpty(AssemblyHolderList assemblyHolders)
    {
        WriteLine(true);
        WriteLine(true, Green, "Purging empty elements... ");

        foreach (var holder in assemblyHolders.ToList())
        {
            holder.PurgeEmpty();
            if (holder.TypeHolders.Count == 0) assemblyHolders.Remove(holder);
        }
    }

    // ====================================================

    /// <summary>
    /// Invoked to execute the test methods.
    /// </summary>
    /// <param name="assemblyHolders"></param>
    void Execute(AssemblyHolderList assemblyHolders)
    {
        var num = 0;
        var ts = TimeSpan.Zero;
        var done = false;
        ConsoleKeyInfo? key;
        ConsoleColor old = Console.ForegroundColor;

        foreach (var assemblyHolder in assemblyHolders)
        {
            key = ReadKey(TimeSpan.Zero, display: false);
            if (key != null && key.Value.Key == ConsoleKey.Escape) done = true;
            if (done) break;

            foreach (var typeHolder in assemblyHolder.TypeHolders)
            {
                key = ReadKey(TimeSpan.Zero, display: false);
                if (key != null && key.Value.Key == ConsoleKey.Escape) done = true;
                if (done) break;

                var type = typeHolder.Type;
                var instance = type.IsStatic() ? null : Activator.CreateInstance(type);

                foreach (var methodHolder in typeHolder.MethodHolders)
                {
                    key = ReadKey(TimeSpan.Zero, display: false);
                    if (key != null && key.Value.Key == ConsoleKey.Escape) done = true;
                    if (done) break;

                    WriteLine(true);
                    WriteLine(true, Green, Program.FatSeparator);
                    WriteLine(true, Green, $"{typeHolder.FullName}.{methodHolder}");
                    WriteLine(true, Green, Program.FatSeparator);

                    Console.ForegroundColor = old;
                    var span = Execute(instance, methodHolder.Method);
                    Console.ForegroundColor = old;

                    WriteLine(true);
                    PrintResults(span);

                    ts = ts.Add(span);
                    num++;
                }

                if (instance is not null and
                    IDisposable disposable) disposable.Dispose();
            }
        }

        WriteLine(true);
        WriteLine(true, Green, Program.FatSeparator);
        WriteLine(true, Green, "Execution summary...");
        WriteLine(true, Green, Program.FatSeparator);

        WriteLine(true);
        Write(true, "Number of tests executed: ");
        WriteLine(true, Cyan, num.ToString());
        PrintResults(ts);
        WriteLine(true);
    }

    /// <summary>
    /// Invoked to execute the given method on the given instance, which can be null in case of
    /// static methods.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="method"></param>
    TimeSpan Execute(object? instance, MethodInfo method)
    {
        var watch = new Stopwatch();
        watch.Start();

        try
        {
            if (method.ReturnType.IsAssignableTo(typeof(ValueTask)))
            {
                Task.Run(async () =>
                {
                    var item = method.Invoke(instance, null);
                    var task = (ValueTask)item!;
                    await task;
                })
                .Wait();
            }
            else if (method.ReturnType.IsAssignableTo(typeof(Task)))
            {
                Task.Run(async () =>
                {
                    var item = method.Invoke(instance, null);
                    var task = (Task)item!;
                    await task;
                })
                .Wait();
            }
            else
            {
                method.Invoke(instance, null);
            }
        }
        catch (Exception e)
        {
            WriteLine(true);
            WriteLine(true, Red, Program.FatSeparator);
            WriteLine(true, Red, e.ToDisplayString());
            WriteLine(true, Red, Program.FatSeparator);
            WriteLine(true);

            if (BreakOnError)
            {
                WriteLine(true);
                WriteLine(true, "Executor cannot proceed further and will finish.");
                ReadLine();
                Environment.FailFast(null);
            }
        }
        finally
        {
            watch.Stop();
        }

        return watch.Elapsed;
    }

    /// <summary>
    /// Invoked to print the given results.
    /// </summary>
    /// <param name="ts"></param>
    static void PrintResults(TimeSpan ts)
    {
        var ms = ts.TotalMilliseconds;

        Write(true, "Execution time: ");
        WriteLine(true, Cyan, ms < 1000 ? $"{ms:#.00} ms" : $"{ms / 1000:#.000}");
    }
}