using static Yotei.Tools.Diagnostics.DebugEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class MenuTester : ConsoleMenuEntry
{
    static readonly StringComparison Ordinal = StringComparison.Ordinal;
    static readonly StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;
    
    static readonly BindingFlags MethodFlags =
        BindingFlags.Instance | BindingFlags.Static |
        BindingFlags.Public | BindingFlags.NonPublic;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="breakOnError"></param>
    public MenuTester(bool breakOnError) => BreakOnError = breakOnError;
    bool BreakOnError { get; }

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
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create and populate the test hierarchy.
    /// </summary>
    static void Populate(AssemblyHolderList assemblyHolders)
    {
        assemblyHolders = [];

        var root = new DirectoryInfo(AppContext.BaseDirectory);
        WriteLine(true);
        Write(true, Green, "Populating from: "); WriteLine(true, root.FullName);

        var first = true;
        var files = root.GetFiles();
        foreach (var file in files)
        {
            // Assembly must have a valid termination...
            var name = file.Name;
            if (!name.EndsWith(".DLL", IgnoreCase) &&
                !name.EndsWith(".EXE", IgnoreCase))
                continue;

            if (first) WriteLine(true); first = false;
            WriteLine(true, file.Name);

            // Try to load and populate the assembly...
            try
            {
                var assembly = Assembly.LoadFrom(file.FullName);
                if (assemblyHolders.Find(assembly) is not null) continue;

                var assemblyHolder = new AssemblyHolder(assembly);

                Populate(assemblyHolder);
                if (assemblyHolder.TypeHolders.Count > 0) assemblyHolders.Add(assemblyHolder);
            }
            catch { }
        }
    }

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
    /// <param name="assemblyHolder"></param>
    /// <param name="typeHolder"></param>
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
                if (!Match(assemblyHolder.Name, item.AssemblyName, IgnoreCase)) { valid = false; break; }
                if (!Match(typeHolder.Name, item.TypeName, Ordinal)) { valid = false; break; }
                if (!Match(methodHolder.Name, item.MethodName, Ordinal)) { valid = false; break; }
            }
            if (!valid) continue;

            // Explicit includes: must match...
            foreach (var item in Program.Excludes)
            {
                if (!Match(assemblyHolder.Name, item.AssemblyName, IgnoreCase)) continue;
                if (!Match(typeHolder.Name, item.TypeName, Ordinal)) continue;
                if (Match(methodHolder.Name, item.MethodName, Ordinal)) { valid = false; break; }
            }
            if (!valid) continue;

            // Populating...
            typeHolder.MethodHolders.Add(methodHolder);
        }
    }

    /// <summary>
    /// Determines if the given source matches the given specification, or not.
    /// <br/>- If the specification is null, then it is ignored and a match is reported.
    /// <br/>- If the specification ends with '*', then if the source starts with the spec a
    /// match is reported. Otherwise, it only matches if they are equal.
    /// </summary>
    static bool Match(string source, string? spec, StringComparison comparison)
    {
        source = source.NotNullNotEmpty(true);
        spec = spec?.NotNullNotEmpty(true);

        if (spec is null) return true;

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
}