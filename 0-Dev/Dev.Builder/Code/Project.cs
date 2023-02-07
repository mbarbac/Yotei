using System.Linq;

namespace Dev.Builder;

// ========================================================
/// <summary>
/// Represents a project file.
/// </summary>
public record Project : File
{
    /// <summary>
    /// An empty instance.
    /// </summary>
    public static new Project Empty { get; } = new();
    protected Project() { }

    /// <summary>
    /// Initializes a new instance using the given path, which can either be null, or empty, or
    /// an existing project path.
    /// </summary>
    /// <param name="path"></param>
    public Project(string path) : base(path) { }

    /// <summary>
    /// Initializes a new instance using its directory, name and extension.
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="name"></param>
    /// <param name="extension"></param>
    public Project(Directory directory, string name, string extension)
        : base(directory, name, extension) { }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString() => Name;

    /// <summary>
    /// The extension of this file, or an empty string.
    /// </summary>
    public override string Extension
    {
        get => base.Extension;
        init
        {
            if (value != null &&
                value.Length > 0 &&
                !ValidExtension(value))
                throw new ArgumentException($"Extension '{value}' is not 'csproj'.");

            base.Extension = value!;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given extension is a valid project extension one, or not.
    /// </summary>
    /// <param name="extension"></param>
    /// <returns></returns>
    public static bool ValidExtension(string extension)
    {
        extension = extension.NotNullNotEmpty();
        return string.Compare(extension, "csproj", Comparison) == 0;
    }
}

// ========================================================
public static class ProjectExtensions
{
    static StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given project is a packable one, or not.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static bool IsPackable(this Project project, out SemanticVersion? version)
    {
        project = project.ThrowIfNull();

        version = project.FindVersion();
        if (version != null)
        {
            var lines = _File.ReadAllLines(project.FullName);
            foreach (var line in lines)
            {
                var ini = line.IndexOf(PackableHead, Comparison);
                if (ini < 0) continue;
                ini += PackableHead.Length;

                var end = line.IndexOf(PackableTail, ini, Comparison);
                if (end < 0) continue;

                var value = line[ini..end].Trim();
                return bool.TryParse(value, out var temp) ? temp : false;
            }
        }

        return false;
    }
    const string PackableHead = "<IsPackable>";
    const string PackableTail = "</IsPackable>";

    // ----------------------------------------------------

    /// <summary>
    /// Returns the version found in the given project, or null if any.
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    public static SemanticVersion? FindVersion(this Project project)
    {
        project = project.ThrowIfNull();

        var lines = _File.ReadAllLines(project.FullName);
        foreach (var line in lines)
        {
            var ini = line.IndexOf(VersionHead, Comparison);
            if (ini < 0) continue;
            ini += VersionHead.Length;

            var end = line.IndexOf(VersionTail, ini, Comparison);
            if (end < 0) continue;

            var value = line[ini..end].Trim();
            if (value.Length > 0) return new SemanticVersion(value);
        }

        return null;
    }
    const string VersionHead = "<Version>";
    const string VersionTail = "</Version>";

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given collection of projects the equivalent ones in the given range.
    /// </summary>
    /// <param name="projects"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static ImmutableList<Project> RemoveRange(
        this IEnumerable<Project> projects,
        IEnumerable<Project> range)
    {
        projects = projects.ThrowIfNull();
        range = range.ThrowIfNull();

        var list = projects.ToList();
        foreach (var item in range)
        {
            var temp = list.Find(x => x.EquivalentTo(item));
            if (temp != null) list.Remove(temp);
        }

        return list.ToImmutableList();
    }

    /// <summary>
    /// Finds among the given collection of projects the first one whose name matches the given
    /// one.
    /// </summary>
    /// <param name="projects"></param>
    /// <param name="fullname"></param>
    /// <returns></returns>
    public static Project? FindProject(this IEnumerable<Project> projects, string fullname)
    {
        projects = projects.ThrowIfNull();
        fullname = fullname.NotNullNotEmpty();

        return projects.FirstOrDefault(x => x.EquivalentTo(fullname));
    }

    // ----------------------------------------------------

    /// <summary>
    /// Compiles this project. Returns true if the compilation exitted with code value of cero,
    /// or false otherwise.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static bool Compile(this Project project, CompileMode mode)
    {
        project = project.ThrowIfNull();

        WriteLine();
        Write(Color.Green, "Compiling: "); WriteLine(project.NameAndExtension);
        WriteLine();

        var p = new Process();
        p.StartInfo.FileName = Program.CompileExe;
        p.StartInfo.WorkingDirectory = project.Directory.Value;
        p.StartInfo.Arguments = $"build -c {mode} {project.NameAndExtension}";
        p.Start();
        p.WaitForExit();

        return p.ExitCode == 0;
    }
}