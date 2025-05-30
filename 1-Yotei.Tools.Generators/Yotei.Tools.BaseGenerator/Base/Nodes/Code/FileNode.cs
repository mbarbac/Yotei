﻿namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Represents a file-alike top-most node in the source code generation hierarchy.
/// </summary>
internal sealed class FileNode : INode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="compilation"></param>
    public FileNode(string fileName, Compilation compilation)
    {
        FileName = fileName.NotNullNotEmpty();
        Compilation = compilation.ThrowWhenNull();
    }

    /// <inheritdoc/>
    public override string ToString() => $"File: {FileName}";

    /// <summary>
    /// The name of this file, without extension parts.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// The compilation for which this node is obtained.
    /// </summary>
    public Compilation Compilation { get; }

    /// <inheritdoc/>
    public Compilation GetBranchCompilation() => Compilation;

    // ----------------------------------------------------

    /// <summary>
    /// The collection of child namespaces registered in this file node.
    /// </summary>
    public ChildNamespaces ChildNamespaces { get; } = [];

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Validate(SourceProductionContext context)
    {
        var r = true;

        foreach (var node in ChildNamespaces) if (!node.Validate(context)) r = false;
        return r;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        cb.AppendLine("// <auto-generated/>");
        cb.AppendLine("#nullable enable");
        cb.AppendLine();
        cb.AppendLine("#pragma warning disable CS0108");
        cb.AppendLine();

        EmitPragmas(cb);
        EmitUsings(cb);

        var done = false;
        foreach (var node in ChildNamespaces)
        {
            if (done) cb.AppendLine(); done = true;
            node.Emit(context, cb);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Generates file-level pragmas, if any.
    /// </summary>
    /// <param name="cb"></param>
    void EmitPragmas(CodeBuilder cb)
    {
        var items = new List<string>();

        foreach (var node in ChildNamespaces)
        {
            var tree = node.Syntax.SyntaxTree;
            if (tree.TryGetText(out var text))
            {
                foreach (var line in text.Lines)
                {
                    var str = line.ToString().Trim();
                    if (str.StartsWith("#pragma") && !items.Contains(str)) items.Add(str);
                    else if (str.StartsWith("namespace")) break;
                }
            }
        }

        if (items.Count > 0)
        {
            foreach (var item in items) cb.AppendLine(item);
            cb.AppendLine();
        }
    }

    /// <summary>
    /// Generates file-level usings, if any.
    /// </summary>
    /// <param name="cb"></param>
    void EmitUsings(CodeBuilder cb)
    {
        var items = new List<string>();

        foreach (var node in ChildNamespaces)
        {
            var comp = node.Syntax.GetCompilationUnitSyntax();
            foreach (var item in comp.Usings)
            {
                var str = item.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(str) && !items.Contains(str)) items.Add(str);
            }
        }

        if (items.Count > 0)
        {
            foreach (var item in items) cb.AppendLine(item);
            cb.AppendLine();
        }
    }
}