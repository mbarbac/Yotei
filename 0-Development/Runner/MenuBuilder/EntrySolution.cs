using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class EntrySolution : MenuEntry
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="projects"></param>
    public EntrySolution(List<Project> projects) => Projects = projects.ThrowWhenNull();

    /// <summary>
    /// The collection of projects to build.
    /// </summary>
    public List<Project> Projects { get; }

    /// <inheritdoc/>
    public override string Header() => "All Packages";

    /// <inheritdoc/>
    public override void Execute()
    {
        WriteLine(true);
        WriteLine(true, Green, Program.FatSeparator);
        WriteLine(true, Green, "Compiling all projects.");

        if (!MenuBuilder.CaptureMode(out var mode)) return;

        var backups = new ProjectBackups();
        bool done = false;

        try
        {
            foreach (var project in Projects)
            {
                done = project.IsPackable(out var oldversion);
                if (!done) break;

                var newversion = MenuBuilder.IncreaseVersion(oldversion!, mode);
                var entry = new EntryPackage(project);

                done = entry.Execute(backups, true, mode, newversion);
                if (!done) break;
            }
        }
        catch (Exception e)
        {
            WriteLine(true);
            WriteLine(true, Red, "Exception intercepted: ");
            WriteLine(true, e.ToDisplayString());
        }
        finally
        {
            if (!done)
            {
                WriteLine(true);
                WriteLine(true, Red, Program.FatSeparator);
                WriteLine(true, Red, "Errors have been detected...");

                backups.Restore();
            }
        }
    }
}