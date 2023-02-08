namespace Dev.Builder;

// ========================================================
/// <summary>
/// Represents a project file.
/// </summary>
public record Project
{
    /// <summary>
    /// An empty instance.
    /// </summary>
    public static Project Empty { get; } = new();
    protected Project() { }

    /// <summary>
    /// Initializes a new instance using the given file.
    /// </summary>
    /// <param name="file"></param>
    public Project(File file)
    {
        File = file;

        if (string.Compare(file.Extension, "csproj", Program.Comparison) != 0)
            throw new ArgumentException($"File extension is not 'csproj': {File}");
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString() => File.Name;

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    public static implicit operator string(Project project) => project.File.Path;

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    public static implicit operator Project(string path) => new(new File(path));

    /// <summary>
    /// The file of this project.
    /// </summary>
    public File File
    {
        get => _Value;
        init => _Value = value.ThrowIfNull();
    }
    File _Value = File.Empty;
}

// ========================================================
public static class ProjectExtensions
{
    /// <summary>
    /// Selects from the given collection of projects the first one whose path matches the given
    /// one.
    /// </summary>
    /// <param name="projects"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Project? SelectProject(this IEnumerable<Project> projects, string path)
    {
        projects = projects.ThrowIfNull();
        path = path.NotNullNotEmpty();

        return projects.FirstOrDefault(x => x.File.EquivalentTo(path));
    }

    /// <summary>
    /// Removes from the given collection of projects the ones that are equivalent to the given
    /// one.
    /// </summary>
    /// <param name="projects"></param>
    /// <param name="removable"></param>
    /// <returns></returns>
    public static ImmutableArray<Project> RemoveProject(
        this IEnumerable<Project> projects, Project removable)
    {
        projects = projects.ThrowIfNull();
        removable = removable.ThrowIfNull();

        var list = new List<Project>();
        foreach (var project in projects)
        {
            if (!project.File.EquivalentTo(removable.File)) list.Add(project);
        }
        return list.ToImmutableArray();
    }

    /// <summary>
    /// Removes from the given collection of projects the ones that are equivalent to the given
    /// ones.
    /// </summary>
    /// <param name="projects"></param>
    /// <param name="removables"></param>
    /// <returns></returns>
    public static ImmutableArray<Project> RemoveProjects(
        this IEnumerable<Project> projects, IEnumerable<Project> removables)
    {
        projects = projects.ThrowIfNull();
        removables = removables.ThrowIfNull();

        var list = new List<Project>();
        foreach (var project in projects)
        {
            var found = false;
            foreach (var removable in removables)
            {
                if (project.File.EquivalentTo(removable.File))
                {
                    found = true;
                    break;
                }
            }
            if (!found) list.Add(project);
        }
        return list.ToImmutableArray();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Compiles the given project.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static bool Compile(this Project project, CompileMode mode)
    {
        project = project.ThrowIfNull();

        WriteLine();
        Write(Color.Green, "Compiling: "); WriteLine(project.File.NameAndExtension);
        WriteLine();

        var p = new Process();
        p.StartInfo.FileName = Program.CompileExe;
        p.StartInfo.WorkingDirectory = project.File.Directory.Path;
        p.StartInfo.Arguments = $"build -c {mode} {project.File.NameAndExtension}";
        p.Start();
        p.WaitForExit();

        return p.ExitCode == 0;
    }
}