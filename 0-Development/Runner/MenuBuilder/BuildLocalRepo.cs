using static Yotei.Tools.Diagnostics.ConsoleWrapper;
using static System.ConsoleColor;

namespace Runner.Builder;

// ========================================================
public class BuildLocalRepo : MenuEntry
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string Header() => "ReBuild Local Repo";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Execute()
    {
        WriteLine(true);
        WriteLine(true, Green, Program.FatSeparator);
        WriteLine(true, Green, Header());
        WriteLine(true);

        WriteLine(true, Green, Program.SlimSeparator);
        WriteLine(true, Green, "Clearing local Repo...");
        try
        {
            var dir = new DirectoryInfo(Program.LocalRepoPath);
            var files = dir.GetFiles();
            foreach (var file in files) file.Delete();
        }
        catch { }

        var root = Program.GetSolutionDirectory();
        var projects = root.FindProjects(null);
        var packables = projects.SelectPackableProjects();

        WriteLine(true);
        WriteLine(true, Green, "Ordering packable projects...");
        packables = ReOrder(packables);
        foreach (var packable in packables) WriteLine(true, $"- {packable}");

        foreach (var packable in packables)
        {
            var done = PackageBuilder.UpdateLocalRepo(packable);
            if (!done) return;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reorders the list of projects according to their cross-references.
    /// </summary>
    /// <param name="projects"></param>
    /// <returns></returns>
    List<Project> ReOrder(IEnumerable<Project> packables)
    {
        var list = new List<Project>(); foreach (var packable in packables)
        {
            var pname = packable.Name;
            var found = false;

            for (int i = 0; i < list.Count; i++)
            {
                if (found) break;

                var item = list[i];
                var nrefs = item.GetNuReferences(); foreach (var nref in nrefs)
                {
                    if (string.Compare(pname, nref.Name, ignoreCase: true) == 0)
                    {
                        list.Insert(i, packable);
                        found = true;
                        break;
                    }
                }
            }
            if (!found) list.Add(packable);
        }
        return list;
    }
}