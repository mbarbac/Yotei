﻿using static Yotei.Tools.Diagnostics.ConsoleEx;
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

        var backups = new BuildBackups();
        var done = true;

        foreach (var project in Projects)
        {
            done = project.IsPackable(out var oldversion);
            if (!done) break;

            SemanticVersion newversion = default!;
            switch (mode)
            {
                case BuildMode.Debug: newversion = oldversion!.IncreasePreRelease("v001"); break;
                case BuildMode.Local: newversion = oldversion!.PreRelease.IsEmpty ? oldversion with { PreRelease = "v001" } : oldversion; break;
                case BuildMode.Release: newversion = oldversion!.IncreasePatch(); break;
                default: goto FINISHING;
            }

            done = EntryPackage.Execute(backups, project, mode, newversion);
            if (!done) break;
        }

        FINISHING:
        if (!done)
        {
            WriteLine(true);
            WriteLine(true, Red, Program.FatSeparator);
            WriteLine(true, Red, "Errors have been detected...");

            backups.Restore();
        }
    }
}