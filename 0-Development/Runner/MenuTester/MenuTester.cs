using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class MenuTester : MenuEntry
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="breakOnError"></param>
    public MenuTester(bool breakOnError) => BreakOnError = breakOnError;

    /// <summary>
    /// Determines if the execution fails as soon as an error is detected, or rather it is
    /// reported and the remaining tests executed.
    /// </summary>
    public bool BreakOnError { get; }

    /// <inheritdoc/>
    public override string Header() => "Execute Tests";

    /// <inheritdoc/>
    public override void Execute()
    {
        WriteLine(true);
        WriteLine(true, Green, Program.FatSeparator);
        WriteLine(true, Green, Header());

        var assemblyHolders = new AssemblyHolderList();
        Populate(assemblyHolders);
        RemoveExcludes(assemblyHolders);
        RemoveNotEnforced(assemblyHolders);

        PurgeEmpty(assemblyHolders);
        Execute(assemblyHolders, BreakOnError);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Populates the list of assemblies.
    /// </summary>
    /// <param name="assemblyHolders"></param>
    static void Populate(AssemblyHolderList assemblyHolders)
    {
        if (Program.Includes.Count == 0) PopulateFromRoot(assemblyHolders);
        else PopulateFromIncludes(assemblyHolders);
    }

    /// <summary>
    /// Populates the list of assemblies from the solution's root directory.
    /// </summary>
    /// <param name="assemblyHolders"></param>
    static void PopulateFromRoot(AssemblyHolderList assemblyHolders)
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);

        WriteLine(true);
        Write(true, Green, "Populating from root directory: ");
        WriteLine(true, root.FullName);

        var files = root.GetFiles();
        var comparison = StringComparison.OrdinalIgnoreCase;

        foreach (var file in files)
        {
            if (!file.Name.EndsWith(".DLL", comparison) &&
                !file.Name.EndsWith(".EXE", comparison)) continue;

            try
            {
                var assembly = Assembly.LoadFrom(file.FullName);

                if (assemblyHolders.Find(assembly) == null)
                {
                    var holder = new AssemblyHolder(assembly);
                    Populate(holder, null, null);

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
    /// Populates the list of assemblies from the explicit includes.
    /// </summary>
    /// <param name="assemblyHolders"></param>
    static void PopulateFromIncludes(AssemblyHolderList assemblyHolders)
    {
        WriteLine(true);
        WriteLine(true, Green, "Populating from explicit includes...");

        foreach (var item in Program.Includes)
        {
            WriteLine(true, item.ToString());

            var name = item.AssemblyName ?? throw new ArgumentException(
                "Assembly name for a explicit include cannot be null.")
                .WithData(item);

            if (!name.EndsWith(".DLL", StringComparison.OrdinalIgnoreCase))
                name += ".DLL";

            var assembly = Assembly.LoadFrom(name);

            if (assemblyHolders.Find(assembly) == null)
            {
                var holder = new AssemblyHolder(assembly);
                Populate(holder, item.TypeName, item.MethodName);

                if (holder.TypeHolders.Count > 0)
                {
                    assemblyHolders.Add(holder);
                }
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Populates the test classes and methods.
    /// </summary>
    /// <param name="assemblyHolder"></param>
    /// <param name="typeName"></param>
    /// <param name="methodName"></param>
    static void Populate(AssemblyHolder assemblyHolder, string? typeName, string? methodName)
    {
        foreach (var type in assemblyHolder.Assembly.DefinedTypes)
        {
            if (!TypeHolder.IsValidTestClass(type)) continue;

            if (typeName is not null &&
                string.Compare(typeName, type.Name) != 0) continue;

            if (assemblyHolder.TypeHolders.Find(type) == null)
            {
                var typeHolder = new TypeHolder(type);
                Populate(typeHolder, methodName);

                if (typeHolder.MethodHolders.Count > 0)
                    assemblyHolder.TypeHolders.Add(typeHolder);
            }
        }
    }

    /// <summary>
    /// Populates the test methods.
    /// </summary>
    /// <param name="typeHolder"></param>
    /// <param name="methodName"></param>
    static void Populate(TypeHolder typeHolder, string? methodName)
    {
        var flags =
            BindingFlags.Instance | BindingFlags.Static |
            BindingFlags.Public | BindingFlags.NonPublic;

        foreach (var method in typeHolder.Type.GetMethods(flags))
        {
            if (!MethodHolder.IsValidTestMethod(method)) continue;

            if (methodName is not null &&
                string.Compare(methodName, method.Name) != 0) continue;

            if (typeHolder.MethodHolders.Find(method) == null)
            {
                var methodHolder = new MethodHolder(method);
                typeHolder.MethodHolders.Add(methodHolder);
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes the explicit excludes.
    /// </summary>
    /// <param name="assemblyHolders"></param>
    static void RemoveExcludes(AssemblyHolderList assemblyHolders)
    {
        if (Program.Excludes.Count == 0) return;

        WriteLine(true);
        WriteLine(true, Green, "Removing explicit excludes...");

        foreach (var exclude in Program.Excludes)
        {
            if (exclude.AssemblyName is not null) // Assembly is given...
            {
                var assemblyHolder = assemblyHolders.Find(exclude.AssemblyName);
                if (assemblyHolder != null)
                {
                    if (exclude.TypeName is null && exclude.MethodName is null)
                    {
                        Write(true, Green, "Removing assembly: ");
                        WriteLine(exclude.AssemblyName);

                        assemblyHolders.Remove(assemblyHolder);
                    }
                    else
                    {
                        RemoveExcludes(assemblyHolder, exclude.TypeName, exclude.MethodName);
                    }
                }
            }
            else // Assembly not given...
            {
                foreach (var assemblyHolder in assemblyHolders)
                    RemoveExcludes(assemblyHolder, exclude.TypeName, exclude.MethodName);
            }
        }
    }

    /// <summary>
    /// Removes the explicit excludes.
    /// </summary>
    /// <param name="assemblyHolder"></param>
    /// <param name="typeName"></param>
    /// <param name="methodName"></param>
    static void RemoveExcludes(AssemblyHolder assemblyHolder, string? typeName, string? methodName)
    {
        if (typeName is not null) // Type is given...
        {
            var typeHolder = assemblyHolder.TypeHolders.Find(typeName);
            if (typeHolder is not null)
            {
                if (methodName is null)
                {
                    Write(true, Green, "Removing type: ");
                    WriteLine(typeName);

                    assemblyHolder.TypeHolders.Remove(typeHolder);
                }
                else
                {
                    RemoveExcludes(typeHolder, methodName);
                }
            }
        }
        else // Type is not given...
        {
            foreach (var typeHolder in assemblyHolder.TypeHolders)
                RemoveExcludes(typeHolder, methodName);
        }
    }

    /// <summary>
    /// Removes the explicit excludes.
    /// </summary>
    /// <param name="typeHolder"></param>
    /// <param name="methodName"></param>
    static void RemoveExcludes(TypeHolder typeHolder, string? methodName)
    {
        if (methodName is null) return;

        var methodHolder = typeHolder.MethodHolders.Find(methodName);
        if (methodHolder is not null)
        {
            Write(true, Green, "Removing method: ");
            Write(true, $"{typeHolder.FullName}.");
            WriteLine(methodName);

            typeHolder.MethodHolders.Remove(methodHolder);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes the not enforced test classes and methods, if needed.
    /// </summary>
    /// <param name="assemblyHolders"></param>
    static void RemoveNotEnforced(AssemblyHolderList assemblyHolders)
    {
        // Maintains if there are enforced methods and types...
        bool methods = false;
        bool types = false;

        foreach (var assemblyHolder in assemblyHolders)
        {
            foreach (var typeHolder in assemblyHolder.TypeHolders)
            {
                foreach (var methodHolder in typeHolder.MethodHolders)
                {
                    if (methodHolder.IsEnforced) methods = true;
                }

                if (typeHolder.IsEnforced) types = true;
            }
        }

        // Enforced methods found...
        if (methods)
        {
            foreach (var assemblyHolder in assemblyHolders)
            {
                foreach (var typeHolder in assemblyHolder.TypeHolders)
                {
                    if (!typeHolder.IsEnforced)
                    {
                        foreach (var methodHolder in typeHolder.MethodHolders.ToList())
                        {
                            if (!methodHolder.IsEnforced)
                                typeHolder.MethodHolders.Remove(methodHolder);
                        }
                    }
                }
            }
        }

        // Enforced types found...
        if (types)
        {
            foreach (var assemblyHolder in assemblyHolders)
            {
                foreach (var typeHolder in assemblyHolder.TypeHolders.ToList())
                {
                    if (!typeHolder.IsEnforced &&
                        !typeHolder.MethodHolders.Any(x => x.IsEnforced))
                        assemblyHolder.TypeHolders.Remove(typeHolder);
                }
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Purges empty entries.
    /// </summary>
    /// <param name="assemblyHolders"></param>
    static void PurgeEmpty(AssemblyHolderList assemblyHolders)
    {
        foreach (var assemblyHolder in assemblyHolders.ToList())
        {
            PurgeEmpty(assemblyHolder.TypeHolders);

            if (assemblyHolder.TypeHolders.Count == 0)
                assemblyHolders.Remove(assemblyHolder);
        }
    }

    /// <summary>
    /// Purges empty entries.
    /// </summary>
    /// <param name="typeHolders"></param>
    static void PurgeEmpty(TypeHolderList typeHolders)
    {
        foreach (var typeHolder in typeHolders.ToList())
        {
            if (typeHolder.MethodHolders.Count == 0)
                typeHolders.Remove(typeHolder);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Executes the tests found in the given assemblies.
    /// </summary>
    /// <param name="assemblyHolders"></param>
    /// <param name="breakOnError"></param>
    static void Execute(AssemblyHolderList assemblyHolders, bool breakOnError)
    {
        var done = false;
        var num = 0;
        var ts = TimeSpan.Zero;
        var old = Console.ForegroundColor;

        // Iterating through the assemblies...
        foreach (var assemblyHolder in assemblyHolders)
        {
            if (done) break;

            // Iterating through the types...
            foreach (var typeHolder in assemblyHolder.TypeHolders)
            {
                if (done) break;

                // Obtaining a type instance, if needed...
                var type = typeHolder.Type;
                var instance = type.IsStatic() ? null : Activator.CreateInstance(type);

                // Iterating through the methods...
                foreach (var methodHolder in typeHolder.MethodHolders)
                {
                    // Finding if [Escape] is pressed...
                    var key = ReadKey(TimeSpan.Zero, display: false);
                    if (key != null && key.Value.Key == ConsoleKey.Escape)
                    {
                        done = true;
                        break;
                    }

                    var name = $"{assemblyHolder.Name}.{type.Name}.{methodHolder.Name}";
                    WriteLine(true);
                    WriteLine(true, Green, Program.FatSeparator);
                    WriteLine(true, Green, $"Executing: {name}");
                    WriteLine(true, Green, Program.FatSeparator);

                    Console.ForegroundColor = old;
                    var span = Execute(instance, methodHolder.Method, breakOnError);
                    Console.ForegroundColor = old;

                    WriteLine(true);
                    PrintResults(span);

                    ts = ts.Add(span);
                    num++;
                }

                // Releasing the type's instance...
                if (instance is not null and IDisposable disposable) disposable.Dispose();
            }
        }

        // Printing summary...
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
    /// Executes the given test method.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    static TimeSpan Execute(object? instance, MethodInfo method, bool breakOnError)
    {
        var ini = Stopwatch.GetTimestamp();

        // xUnit only supports Task and void as return types...
        try
        {
            if (method.ReturnType.IsAssignableTo(typeof(Task)))
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
        catch (Exception ex)
        {
            WriteLine(true);
            WriteLine(true, Red, Program.FatSeparator);
            WriteLine(true, Red, ex.ToDisplayString());
            WriteLine(true, Red, Program.FatSeparator);
            WriteLine(true);

            if (breakOnError)
            {
                WriteLine(true);
                WriteLine(true, "Executor cannot proceed further and will finish.");
                ReadLine();
                Environment.FailFast(null);
            }
        }

        var end = Stopwatch.GetElapsedTime(ini);
        return end;
    }

    /// <summary>
    /// Invoked to print the execution results.
    /// </summary>
    /// <param name="ts"></param>
    static void PrintResults(TimeSpan ts)
    {
        var ms = ts.TotalMilliseconds;

        Write(true, "Execution time: ");
        WriteLine(true, Cyan, ms < 1000 ? $"{ms:#.00} ms" : $"{ms / 1000:#.000}");
    }
}