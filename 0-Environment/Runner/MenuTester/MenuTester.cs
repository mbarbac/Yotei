using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class MenuTester(bool breakOnError) : ConsoleMenuEntry
{
    bool BreakOnError { get; } = breakOnError;

    static readonly StringComparison Ordinal = StringComparison.Ordinal;
    static readonly StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;
    static readonly BindingFlags MethodFlags =
        BindingFlags.Instance | BindingFlags.Static |
        BindingFlags.Public | BindingFlags.NonPublic;

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
        Console.Clear();
        Console.WriteLineEx(true);
        Console.WriteLineEx(true, Green, Program.FatSeparator);
        Console.WriteLineEx(true, Green, Header());

        Populate(out var assemblyHolders);
        ValidateEnforced(assemblyHolders);
        Execute(assemblyHolders, BreakOnError);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create and populate the test hierarchy.
    /// </summary>
    static void Populate(out AssemblyHolderList assemblyHolders)
    {
        assemblyHolders = [];

        var root = new DirectoryInfo(AppContext.BaseDirectory);
        Console.WriteLineEx(true);
        Console.WriteEx(true, DarkYellow, "Populating from: ");
        Console.WriteEx(true, root.FullName);
        Console.WriteEx(true, " ");

        var top = Console.CursorTop;
        var pos = Console.CursorLeft;
        var sb = new StringBuilder();

        var files = root.GetFiles();
        foreach (var file in files)
        {
            var name = file.Name;
            Print(name);

            // Assembly must have a valid termination...
            if (!name.EndsWith(".DLL", IgnoreCase) &&
                !name.EndsWith(".EXE", IgnoreCase))
                continue;

            // Try to load and populate the assembly...
            try
            {
                var assembly = Assembly.LoadFrom(file.FullName);
                if (assemblyHolders.Find(assembly) is not null) continue;

                var assemblyHolder = new AssemblyHolder(assembly);

                Populate(assemblyHolder);
                if (assemblyHolder.TypeHolders.Count > 0)
                    assemblyHolders.Add(assemblyHolder);
            }
            catch { }
        }

        Print("");
        Console.WriteLineEx(true);

        /// <summary>
        /// Clears the printed buffer and captures and prints the new name.
        /// No need to replicate on DEBUG.
        /// </summary>
        void Print(string name)
        {
            Console.CursorTop = top;
            Console.CursorLeft = pos;
            var len = sb.Length; for (int i = 0; i < len; i++) sb[i] = ' ';
            Console.Write(sb.ToString());

            Console.CursorTop = top;
            Console.CursorLeft = pos;
            sb.Clear(); sb.Append(name);
            Console.Write(name);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to populate the given assembly holder.
    /// </summary>
    static void Populate(AssemblyHolder assemblyHolder)
    {
        foreach (var type in assemblyHolder.Assembly.DefinedTypes)
        {
            if (!TypeHolder.IsValidTest(type)) continue;
            if (assemblyHolder.TypeHolders.Find(type) is not null) continue;

            var typeHolder = new TypeHolder(type);

            Populate(assemblyHolder, typeHolder);
            if (typeHolder.MethodHolders.Count > 0) assemblyHolder.TypeHolders.Add(typeHolder);
        }
    }

    /// <summary>
    /// Invoked to populate the given type holder in the given assembly holder.
    /// </summary>
    static void Populate(AssemblyHolder assemblyHolder, TypeHolder typeHolder)
    {
        foreach (var method in typeHolder.Type.GetMethods(MethodFlags))
        {
            if (!MethodHolder.IsValidTest(method)) continue;
            if (typeHolder.MethodHolders.Find(method) is not null) continue;

            var methodHolder = new MethodHolder(method);
            var valid = true;

            // Explicit includes: must match...
            foreach (var item in Program.Includes)
            {
                if (item.AssemblyName != null && !Match(assemblyHolder.Name, item.AssemblyName, IgnoreCase)) { valid = false; break; }
                if (item.TypeName != null && !Match(typeHolder.Name, item.TypeName, Ordinal)) { valid = false; break; }
                if (item.MethodName != null && !Match(methodHolder.Name, item.MethodName, Ordinal)) { valid = false; break; }
            }
            if (!valid) continue;

            // Explicit includes: must not match...
            foreach (var item in Program.Excludes)
            {
                if (item.AssemblyName != null && Match(assemblyHolder.Name, item.AssemblyName, IgnoreCase)) { valid = false; break; }
                if (item.TypeName != null && Match(typeHolder.Name, item.TypeName, IgnoreCase)) { valid = false; break; }
                if (item.MethodName != null && Match(methodHolder.Name, item.MethodName, IgnoreCase)) { valid = false; break; }
            }
            if (!valid) continue;

            // Populating...
            typeHolder.MethodHolders.Add(methodHolder);
        }
    }

    /// <summary>
    /// Determines if the given source matches the given specification, or not.
    /// <br/>- If the specification ends with '*', then if the source starts with the spec a
    /// match is reported. Otherwise, it only matches if they are equal.
    /// </summary>
    static bool Match(string source, string? spec, StringComparison comparison)
    {
        source = source.NotNullNotEmpty(true);
        spec = spec?.NotNullNotEmpty(true);

        if (source.Length == 0) return false;
        if (spec is null) return false;

        bool strict = true;
        if (spec.EndsWith('*'))
        {
            if (spec.Length == 1) return true; // Spec is '*'...
            strict = false;
            spec = spec[..^1].NotNullNotEmpty(true);
        }

        return strict
            ? string.Compare(source, spec, comparison) == 0
            : source.StartsWith(spec, comparison);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Validates if there are enforced elements and, if so, remove the not enforced ones.
    /// </summary>
    static void ValidateEnforced(AssemblyHolderList assemblyHolders)
    {
        // See if there are types enforced...
        var types = HasEnforcedTypes(assemblyHolders);
        if (types)
        {
            // Assemblies...
            foreach (var assemblyHolder in assemblyHolders.ToList())
            {
                // Types...
                foreach (var typeHolder in assemblyHolder.TypeHolders.ToList())
                {
                    if (!typeHolder.IsEnforced)
                        assemblyHolder.TypeHolders.Remove(typeHolder);
                }

                // Empty assemblies...
                if (assemblyHolder.TypeHolders.Count == 0) assemblyHolders.Remove(assemblyHolder);
            }
        }

        // See if there are method enforced in the remaining elements...
        var methods = HasEnforcedMethods(assemblyHolders);
        if (methods)
        {
            // Assemblies...
            foreach (var assemblyHolder in assemblyHolders.ToList())
            {
                // Types...
                foreach (var typeHolder in assemblyHolder.TypeHolders.ToList())
                {
                    // Methods...
                    foreach (var methodHolder in typeHolder.MethodHolders.ToList())
                    {
                        if (!methodHolder.IsEnforced)
                            typeHolder.MethodHolders.Remove(methodHolder);
                    }

                    // Empty types...
                    if (typeHolder.MethodHolders.Count == 0)
                        assemblyHolder.TypeHolders.Remove(typeHolder);
                }

                // Empty assemblies...
                if (assemblyHolder.TypeHolders.Count == 0) assemblyHolders.Remove(assemblyHolder);
            }
        }
    }

    /// <summary>
    /// Determines if the given hierarchy has enforced types.
    /// </summary>
    static bool HasEnforcedTypes(AssemblyHolderList assemblyHolders)
    {
        foreach (var assemblyHolder in assemblyHolders)
            foreach (var typeHolder in assemblyHolder.TypeHolders)
                if (typeHolder.IsEnforced) return true;

        return false;
    }

    /// <summary>
    /// Determines if the given hierarchy has types with enforced methods.
    /// </summary>
    static bool HasEnforcedMethods(AssemblyHolderList assemblyHolders)
    {
        foreach (var assemblyHolder in assemblyHolders)
            foreach (var typeHolder in assemblyHolder.TypeHolders)
                foreach (var methodHolder in typeHolder.MethodHolders)
                    if (methodHolder.IsEnforced) return true;

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to execute the tests in the given hierarchy.
    /// </summary>
    static void Execute(AssemblyHolderList assemblyHolders, bool breakOnError)
    {
        var done = false; ;
        var num = 0;
        var ts = TimeSpan.Zero;
        var old = Console.ForegroundColor;

        // Iterating through the assemblies...
        foreach (var assemblyHolder in assemblyHolders)
        {
            // [Escape] might have been pressed...
            if (done) break;

            // Iterating through the tyes...
            foreach (var typeHolder in assemblyHolder.TypeHolders)
            {
                // [Escape] might have been pressed...
                if (done) break;

                // We may need a type instance...
                var type = typeHolder.Type;
                var instance = type.IsStatic ? null : Activator.CreateInstance(type);

                // Executing the test methods...
                foreach (var methodHolder in typeHolder.MethodHolders)
                {
                    // [Escape] might have been pressed...
                    var key = Console.ReadKey(TimeSpan.Zero, intercept: true);
                    if (key != null && key.Value.Key == ConsoleKey.Escape) done = true;
                    if (done) break;

                    // Method header...
                    var name = $"{assemblyHolder.Name}.{typeHolder.Name}.{methodHolder.Name}";
                    Console.WriteLineEx(true);
                    Console.WriteLineEx(true, Green, Program.FatSeparator);
                    Console.WriteLineEx(true, Green, $"Executing: {name}");
                    Console.WriteLineEx(true, Green, Program.FatSeparator);

                    Console.ForegroundColor = old;
                    var span = Execute(instance, methodHolder.Method, breakOnError);
                    Console.ForegroundColor = old;
                    Console.WriteLineEx(true);
                    PrintResults(span);

                    ts = ts.Add(span);
                    num++;
                }

                // Releasing the instance if needed...
                if (instance is not null and IDisposable disposable) disposable.Dispose();
            }
        }

        // Finishing...
        Console.WriteLineEx(true);
        Console.WriteLineEx(true, Green, Program.FatSeparator);
        Console.WriteLineEx(true, Green, "Execution summary...");
        Console.WriteLineEx(true, Green, Program.FatSeparator);

        Console.WriteLineEx(true);
        Console.WriteEx(true, "Number of tests executed: ");
        Console.WriteLineEx(true, Cyan, num.ToString());
        PrintResults(ts);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to execute the given test method of the given instance, or the static one if
    /// that instance is null.
    /// </summary>
    static TimeSpan Execute(object? instance, MethodInfo method, bool breakOnError)
    {
        var ini = Stopwatch.GetTimestamp();

        // xUnit only support 'Task' and 'void' as return types...
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
            else if (method.ReturnType.IsAssignableTo(typeof(void)))
            {
                method.Invoke(instance, null);
            }
            else throw new InvalidOperationException("Invalid return type.").WithData(method);
        }
        catch (Exception ex) // We may need to break further execution if requested...
        {
            Console.WriteLineEx(true);
            Console.WriteLineEx(true, Red, Program.FatSeparator);

            var str = ex.ToDisplayString();
            Console.WriteLineEx(true, Red, str);
            Console.WriteLineEx(true, Red, Program.FatSeparator);
            Console.WriteLineEx(true);

            if (breakOnError)
            {
                Console.WriteLineEx(true);
                Console.WriteLineEx(true, "Executor cannot proceed further and will finish.");
                Console.ReadLine();
                Environment.FailFast(null);
            }
        }

        // Finishing
        var end = Stopwatch.GetElapsedTime(ini);
        return end;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to print execution results.
    /// </summary>
    static void PrintResults(TimeSpan ts)
    {
        var ms = ts.TotalMilliseconds;

        Console.WriteEx(true, "Execution time: ");
        Console.WriteLineEx(true, Cyan, ms < 1000 ? $"{ms:#.00} ms" : $"{ms / 1000:#.000}");
    }
}