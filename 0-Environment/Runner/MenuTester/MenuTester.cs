using static Yotei.Tools.ConsoleExtensions;
using static System.ConsoleColor;
namespace Runner;

// ========================================================
public class MenuTester(bool all) : ConsoleMenu.Entry
{
    /// <summary>
    /// Determines if all tests are to be executed, instead of trying to filter them.
    /// </summary>
    public bool All { get; } = all;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string Headline() => All
        ? "Execute All Tests"
        : "Execute Filtered Tests";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Execute()
    {
        Console.WriteLineEx(true);
        Console.WriteLineEx(true, Green, Program.FatSeparator);
        Console.WriteLineEx(true, Green, Headline());
        Console.WriteLineEx(true);

        var items = Populate();
        if (items.Count == 0) { Console.WriteLineEx(true, "No tests found."); return; }

        // Trying to filter the available tests...
        if (!All)
        {
            string? name = null;
            List<string> temps = [];
            Console.WriteLineEx(true, Green, "Blank to ignore. [Esc] to abort. [Tab] to choose.");

            Console.WriteLine();
            Console.WriteEx(true, Green, "Assembly filter:    ");
            temps = [.. items.Select(x => x.AssemblyName).Distinct(StringComparer.OrdinalIgnoreCase)];
            name = Console.EditLineEx(true, null, temps, White, Blue);
            if (name is null) { Console.WriteLineEx(true, "Aborted."); return; }
            name = name.NullWhenEmpty(trim: true);
            if (name != null) items = [.. items.Where(x => x.AssemblyName.Contains(name, ignoreCase: true))];
            if (items.Count == 0) { Console.WriteLineEx(true, "No tests found."); return; }

            Console.WriteEx(true, Green, "Test Class filter:  ");
            temps = [.. items.Select(x => x.TypeFullName).Distinct(StringComparer.Ordinal)];
            name = Console.EditLineEx(true, null, temps, White, Blue);
            if (name is null) { Console.WriteLineEx(true, "Aborted."); return; }
            name = name.NullWhenEmpty(trim: true);
            if (name != null) items = [.. items.Where(x => x.TypeFullName.Contains(name, ignoreCase: true))];
            if (items.Count == 0) { Console.WriteLineEx(true, "No tests found."); return; }

            Console.WriteEx(true, Green, "Test Method filter: ");
            temps = [.. items.Select(x => $"{x.TypeFullName}.{x.MethodName}")];
            name = Console.EditLineEx(true, null, temps, White, Blue);
            if (name is null) { Console.WriteLineEx(true, "Aborted."); return; }
            name = name.NullWhenEmpty(trim: true);
            if (name != null) items = [.. items.Where(x => x.MethodFullName.Contains(name, ignoreCase: true))];
            if (items.Count == 0) { Console.WriteLineEx(true, "No tests found."); return; }
        }

        // Executing...
        Execute(items);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to obtain a base list of test methods.
    /// </summary>
    static List<TestElement> Populate()
    {
        var items = new List<TestElement>();

        var root = new DirectoryInfo(AppContext.BaseDirectory);
        Console.WriteEx(true, Green, "Populating from: ");
        Console.WriteLineEx(true, root.FullName);

        Console.PreventBufferOverflow(5);
        var top = Console.CursorTop;
        var len = 0;

        var files = root.GetFiles();
        foreach (var file in files)
        {
            Console.CursorTop = top;
            Console.CursorLeft = 0;
            Console.WriteEx(true, FromSpaces(len));
            var name = file.Name;

            Console.CursorTop = top;
            Console.CursorLeft = 0;
            Console.WriteEx(true, name);
            len = name.Length;

            if (!name.EndsWith(".DLL", ignoreCase: true) &&
                !name.EndsWith(".EXE", ignoreCase: true))
                continue;

            try
            {
                var assembly = Assembly.LoadFrom(file.FullName);
                var temp = Populate(assembly);
                items.AddRange(temp);
            }
            catch { }
        }
        Console.CursorTop = top;
        Console.CursorLeft = 0;
        Console.WriteLineEx(true, FromSpaces(len));

        return items;
    }

    /// <summary>
    /// Invoked to obtain a list of test methods from the given assembly.
    /// </summary>
    static List<TestElement> Populate(Assembly assembly)
    {
        var items = new List<TestElement>();
        foreach (var type in assembly.DefinedTypes)
        {
            var temp = Populate(type);
            items.AddRange(temp);
        }
        return items;
    }

    /// <summary>
    /// Invoked to obtain a list of test methods from the given type.
    /// </summary>
    static List<TestElement> Populate(Type type)
    {
        var items = new List<TestElement>();
        var flags =
            BindingFlags.Instance | BindingFlags.Static |
            BindingFlags.Public | BindingFlags.NonPublic;

        foreach (var method in type.GetMethods(flags))
            if (TestElement.IsValid(method)) items.Add(new(method));

        return items;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Executes the filtered collection of tests.
    /// </summary>
    /// <param name="items"></param>
    static void Execute(List<TestElement> items)
    {
        var ini = Stopwatch.GetTimestamp();
        var num = 0;
        var dict = items
            .GroupBy(x => x.Type)
            .ToDictionary(x => x.Key, x => x.Select(x => x.Method).ToList());

        // Iterating through the types...
        var cancel = false;
        foreach (var (type, methods) in dict)
        {
            // Creating an instance if needed...
            var instance = type.IsStatic ? null : Activator.CreateInstance(type);
            foreach (var method in methods)
            {
                // Cancellation requested...
                var key = Console.ReadKey(TimeSpan.Zero, hide: true);
                if (key != null && key.Value.Key == ConsoleKey.Escape) cancel = true;
                if (cancel) break;

                // Header...
                Console.WriteLineEx(true);
                Console.WriteLineEx(true, Green, Program.SlimSeparator);
                Console.WriteEx(true, Green, "Executing: ");
                Console.WriteEx(true, $"{method.DeclaringType!.FullName}.");
                Console.WriteEx(true, $"{method.Name}");
                Console.WriteLineEx(true);

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
                        num++;
                    }
                    if (method.ReturnType.IsAssignableTo(typeof(void)))
                    {
                        method.Invoke(instance, null);
                        num++;
                    }
                    else throw new UnExpectedException("Invalid return type.");
                }
                catch (Exception e)
                {
                    Console.WriteLineEx(true);
                    Console.WriteLineEx(true, Red, Program.SlimSeparator);
                    Console.WriteLineEx(true, Red, e.ToDisplayString());
                    Console.WriteLineEx(true);
                    cancel = true;
                }
            }

            // Releasing instance if needed...
            if (instance is not null and IDisposable disposable) disposable.Dispose();
            if (cancel) break;
        }

        // Finishing...
        Console.WriteLineEx(true);
        Console.WriteLineEx(true, Green, Program.SlimSeparator);
        Console.WriteLineEx(true, Green, "Execution summary.");
        Console.WriteLineEx(true);

        Console.WriteEx(true, Green, "Tests executed: ");
        Console.WriteLineEx(true, num.ToString());

        var end = Stopwatch.GetElapsedTime(ini);
        var ms = end.TotalMilliseconds;
        Console.WriteEx(true, Green, "Execution time: ");
        WriteLineEx(true, ms < 1000 ? $"{ms:#.00} ms" : $"{ms / 1000:#.000} ms");
    }
}