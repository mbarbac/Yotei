namespace Dev.Builder;

// ========================================================
/// <summary>
/// Pushes a local package.
/// </summary>
public class LocalPush : Runner
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="packable"></param>
    public LocalPush(Packable packable) => Packable = packable.ThrowIfNull();
    public Packable Packable = Packable.Empty;

    /// <summary>
    /// Prints the header title of this action, ending in a new line.
    /// </summary>
    public override void PrintHead()
    {
        Write("Local push: "); WriteLine(Color.Cyan, $"{Packable}.");
    }

    /// <summary>
    /// Executes this action.
    /// </summary>
    public override void Execute()
    {
        WriteLine();
        WriteLine(Color.Green, Program.Separator);
        Write(Color.Green, "Pushing local package: "); WriteLine(Packable.ToString());
        var files = Packable.Push(PushMode.Local);

        WriteLine();
        foreach (var file in files)
        {
            Write(Color.Green, "Deleting file: "); WriteLine(file.NameAndExtension);
            file.Delete();
        }
        if (files.Length == 0) WriteLine(Color.Red, "No files were deleted...");

        WriteLine();
        WriteLine(Color.Green, "Updating references to this package in found projects...");

        var projects = Program.Projects.Remove(Packable.Project);
        foreach (var project in projects)
        {
            var references = project.GetReferences();
            foreach (var reference in references)
            {
                if (string.Compare(reference.Name, Packable.Project.File.Name, Program.Comparison) == 0)
                {
                    WriteLine();
                    WriteLine(Color.Green, Program.Separator);
                    Write(Color.Green, "Updating reference at project: ");
                    WriteLine(Color.Cyan, project.File.NameAndExtension);
                    WriteLine();

                    var done = RemovePackage(project, reference.Name);
                    if (done) WriteLine(Color.Green, "Reference removed.");
                    else WriteLine(Color.Green, $"Cannot remove referece to: {Packable}");

                    done = AddPackage(project, reference.Name, Packable.Version);
                    if (done) WriteLine(Color.Green, "Reference added.");
                    else WriteLine(Color.Green, $"Cannot add referece to: {Packable}");
                }
            }
        }

        /// <summary> Invoked to remove from the project the given package.
        /// </summary>
        bool RemovePackage(Project project, string name)
        {
            var p = new Process();
            p.StartInfo.FileName = Program.DotNetExe;
            p.StartInfo.WorkingDirectory = project.File.Directory;
            p.StartInfo.Arguments = $"remove package {Packable.Project.File.Name}";

            p.Start();
            p.WaitForExit();
            return p.ExitCode == 0;
        }

        /// <summary> Invoked to add to the project the given package.
        /// </summary>
        bool AddPackage(Project project, string name, string version)
        {
            var source = Program.LocalRepoPath;

            var p = new Process();
            p.StartInfo.FileName = Program.DotNetExe;
            p.StartInfo.WorkingDirectory = project.File.Directory;
            p.StartInfo.Arguments = $"add package {Packable.Project.File.Name} -v {version} -s {source}";

            p.Start();
            p.WaitForExit();
            return p.ExitCode == 0;
        }
    }
}

// ========================================================
/// <summary>
/// Pushes all local packages.
/// </summary>
public class LocalPushAll : Runner
{
    /// <summary>
    /// Prints the header title of this action, ending in a new line.
    /// </summary>
    public override void PrintHead() => WriteLine("Pushes all local packages.");

    /// <summary>
    /// Executes this action.
    /// </summary>
    public override void Execute()
    {
        WriteLine();
        WriteLine(Color.Green, "Pushing all local packages.");

        foreach (var packable in Program.Packables) new LocalPush(packable).Execute();
    }
}

// ========================================================
/// <summary>
/// Pushes all local packages.
/// </summary>
public class LocalPushSelect : Runner
{
    /// <summary>
    /// Prints the header title of this action, ending in a new line.
    /// </summary>
    public override void PrintHead() => WriteLine("Pushes selected local packages.");

    /// <summary>
    /// Executes this action.
    /// </summary>
    public override void Execute()
    {
        var list = new List<LocalPush>();
        foreach (var packable in  Program.Packables) list.Add(new LocalPush(packable));
        Program.MenuRun(list.ToArray());
    }
}