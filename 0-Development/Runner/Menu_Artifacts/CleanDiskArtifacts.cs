﻿using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Runner.Artifacts;

// ========================================================
public class CleanDiskArtifacts : MenuEntry
{
    /// <inheritdoc/>
    public override string Header() => "Clean Disk Artifacts";

    /// <inheritdoc/>
    public override void Execute()
    {
        WriteLine(true);
        WriteLine(true, Green, Program.FatSeparator);
        WriteLine(true, Green, Header());
        WriteLine(true);

        var root = Program.EditDirectory(Program.GetSolutionDirectory(), "Root Directory");
        if (root != null) Execute(root, root, false);
    }

    /// <summary>
    /// Executes on the given path.
    /// </summary>
    static void Execute(string root, string path, bool delete)
    {
        var directory = new DirectoryInfo(path);
        if (!directory.Exists) return;

        var reduced = path.Remove(root);
        if (reduced.Length > 0 && reduced[0] == '\\') reduced = reduced[1..];

        var comparison = StringComparison.OrdinalIgnoreCase;
        if (path.EndsWith(".git", comparison)) return;
        if (path.EndsWith(".vs", comparison)) return;

        // Delete files...
        if (delete)
        {
            var files = directory.GetFiles();
            foreach (var file in files)
            {
                Write(true, Blue, "- File: ");
                Write(true, $"...\\{reduced}\\{file.Name}");
                try
                {
                    file.Delete();
                    WriteLine(true, Green, " - deleted");
                }
                catch (Exception e)
                {
                    WriteLine(true, Red, $" - {e.Message}");
                }
            }
        }

        // Directories...
        var dirs = directory.GetDirectories();
        foreach (var dir in dirs)
        {
            var temp = dir.FullName.ToUpper();
            var remove =
                delete ||
                temp.EndsWith("\\BIN", comparison) ||
                temp.EndsWith("\\OBJ", comparison) ||
                temp.EndsWith("\\GENERATED", comparison);

            Execute(root, dir.FullName, remove);
        }

        // Delete this directory...
        if (delete)
        {
            Write(true, Cyan, "- Path: ");
            Write(true, $"...\\{reduced}");
            try
            {
                directory.Delete();
                WriteLine(true, Green, " - deleted");
            }
            catch (Exception e)
            {
                WriteLine(true, Red, $" -- {e.Message}");
            }
        }
    }
}