namespace Dev.Builder;

// ========================================================
public class CacheManager : MenuItem
{
    /// <inheritdoc>
    /// </inheritdoc>
    public override void Print() => WriteLine("Manage package caches.");

    /// <inheritdoc>
    /// </inheritdoc>
    public override void Execute()
    {
        new Menu().Run(() =>
        {
            WriteLine();
            WriteLine(Program.Color, Program.Separator);
            WriteLine(Program.Color, "Manage package caches.");
            WriteLine();
        },
        new ClearLocalCache(),
        new ClearLocalSource());
    }
}

// ========================================================
public class ClearLocalCache : MenuItem
{
    /// <inheritdoc>
    /// </inheritdoc>
    public override void Print() => WriteLine("Clear local caches.");

    /// <inheritdoc>
    /// </inheritdoc>
    public override void Execute()
    {
        WriteLine();
        WriteLine(Program.Color, Program.Separator);
        WriteLine(Program.Color, "Clear local caches.");
        WriteLine();

        var p = new Process();
        p.StartInfo.FileName = Builder.DotNetExe;
        p.StartInfo.WorkingDirectory = Program.ProjectRoot();
        p.StartInfo.Arguments = "nuget locals all --clear";
        p.Start();
        p.WaitForExit();
    }
}

// ========================================================
public class ClearLocalSource : MenuItem
{
    /// <inheritdoc>
    /// </inheritdoc>
    public override void Print() => WriteLine("Clear local source.");

    /// <inheritdoc>
    /// </inheritdoc>
    public override void Execute()
    {
        WriteLine();
        WriteLine(Program.Color, Program.Separator);
        WriteLine(Program.Color, "Clear local source.");
        WriteLine();

        Directory dir = Builder.LocalRepoPath;
        var files = dir.GetFiles();
        foreach (var file in files)
        {
            Write(Color.Green, "Deleting: "); WriteLine(file.NameAndExtension);
            file.Delete();
        }
    }
}