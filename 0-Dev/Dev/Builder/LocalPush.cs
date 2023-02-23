namespace Dev.Builder;

// ========================================================
public class LocalPush : MenuEntry
{
    public LocalPush(LocalBuilder builder, Packable packable)
    {
        Builder = builder.ThrowIfNull();
        Packable = packable.ThrowIfNull();
    }
    LocalBuilder Builder;
    Packable Packable;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void OnPrint()
    {
        Write(Program.Color, "Push Local Package: ");
        WriteLine(Packable);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void OnExecute()
    {
        WriteLine();
        WriteLine(Program.Color, Program.FatSeparator);
        Print();

        if (!OnCompile()) return;
        if (!OnPush()) return;
        if (!OnReload()) return;
    }

    /// <summary>
    /// Compiles this package.
    /// </summary>
    bool OnCompile()
    {
        WriteLine();
        WriteLine(Program.Color, Program.FatSeparator);
        Write(Program.Color, "Compile: "); WriteLine(Packable);
        WriteLine();

        var done = Packable.Project.Compile(CompileMode.Debug);
        return done;
    }

    /// <summary>
    /// Pushes this package.
    /// </summary>
    bool OnPush()
    {
        WriteLine();
        WriteLine(Program.Color, Program.FatSeparator);
        Write(Program.Color, "Pushing: "); WriteLine(Packable);

        var files = new PackageFiles(Packable, PushMode.Local);
        if (files.RegularFiles.Length == 0)
        {
            WriteLine(Color.Red, "No regular file found...");
            return false;
        }
        if (files.RegularFiles.Length > 1)
        {
            WriteLine(Color.Red, "Too many regular files found...");
            return false;
        }

        var rfile = files.RegularFiles[0];
        WriteLine();
        Write(Program.Color, "Pushing Regular File: "); WriteLine(rfile.NameAndExtension);

        var done = Command.Execute(
            Program.DotNetExe,
            $"nuget push {rfile.NameAndExtension} -s {Program.LocalRepoSource}",
            rfile.Directory);

        if (done != 0)
        {
            WriteLine(Color.Red, "Cannot push regular file...");
            return false;
        }

        var sfile = files.SymbolFiles.Length == 1 ? files.SymbolFiles[0] : null;
        if (sfile != null)
        {
            WriteLine();
            Write(Program.Color, "Pushing Symbols File: "); WriteLine(sfile.NameAndExtension);

            Command.Execute(
                Program.DotNetExe,
                $"nuget push {sfile.NameAndExtension} -s {Program.LocalRepoSource}",
                sfile.Directory);
        }

        WriteLine();
        Write(Program.Color, "Deleting file: "); WriteLine(rfile.NameAndExtension);
        rfile.Delete();

        if (sfile != null)
        {
            Write(Program.Color, "Deleting file: "); WriteLine(sfile.NameAndExtension);
            sfile.Delete();
        }

        return true;
    }

    /// <summary>
    /// Reload this package in the appropriate projects.
    /// </summary>
    bool OnReload()
    {
        WriteLine();
        WriteLine(Program.Color, Program.FatSeparator);
        Write(Program.Color, "Reload: "); WriteLine(Packable);

        var projects = Builder.Projects.Remove(Packable.Project);
        foreach (var project in projects)
        {
            var references = project.GetReferences();
            if (references.Any(x =>
                string.Compare(x.Name, Packable.Name, Program.Comparison) == 0 &&
                string.Compare(x.Version, Packable.Version, Program.Comparison) == 0))
            {
                WriteLine();
                WriteLine(Program.Color, Program.SlimSeparator);
                Write(Program.Color, "Reload on project: "); WriteLine(project.Name);
                WriteLine(Color.Blue, "Please DO NOT interrupt this proces...");

                var lines = _File.ReadAllLines(project.Path);

                WriteLine();
                WriteLine(Program.Color, "Removing...");
                Command.Execute(
                    Program.DotNetExe,
                    $"remove package {Packable.Name}",
                    project.File.Directory);

                WriteLine();
                WriteLine(Program.Color, "Adding...");
                var done = Command.Execute(
                    Program.DotNetExe,
                    $"add package {Packable.Name} -v {Packable.Version} -s {Program.LocalRepoPath}",
                    project.File.Directory)
                    == 0;

                // Keeping the relevant XML nodes from the original project file...
                if (done)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        var line = lines[i];

                        if (!line.Contains("<PackageReference", Program.Comparison)) continue;

                        var head = $"Include=\"";
                        var ini = line.IndexOf(head, Program.Comparison); if (ini < 0) continue;
                        ini += head.Length;
                        var end = line.IndexOf("\"", ini, Program.Comparison); if (end < 0) continue;

                        var oldname = line[ini..end].Trim();
                        if (oldname.Length == 0) continue;
                        if (string.Compare(Packable.Name, oldname, Program.Comparison) != 0) continue;

                        head = $"Version=\"";
                        ini = line.IndexOf(head, Program.Comparison); if (ini < 0) continue;
                        ini += head.Length;
                        end = line.IndexOf("\"", ini, Program.Comparison); if (end < 0) continue;

                        var oldversion = line[ini..end].Trim();
                        if (oldversion.Length == 0) continue;

                        lines[i] = line.Replace(oldversion, Packable.Version);
                        break;
                    }
                }

                _File.WriteAllLines(project.Path, lines);
            }
        }

        return true;
    }
}