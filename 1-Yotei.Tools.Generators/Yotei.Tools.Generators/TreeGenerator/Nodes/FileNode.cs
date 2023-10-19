namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a file-alike node in the source code generation hierarchy.
/// </summary>
internal partial class FileNode : Node
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="fileName"></param>
    public FileNode(TreeGenerator generator, string fileName) : base(generator)
    {
        FileName = fileName.NotNullNotEmpty(nameof(fileName));
        ChildNamespaces = new();
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"File: {FileName}";

    /// <summary>
    /// The file name without path or extensions.
    /// </summary>
    public string FileName { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of child namespaces registered into this instance.
    /// </summary>
    public ChildNamespaces ChildNamespaces { get; }

    /// <summary>
    /// Invoked to register the given candidate into this hierarchy branch.
    /// </summary>
    /// <param name="candidate"></param>
    public void Register(Candidate candidate)
    {
        candidate = candidate.ThrowWhenNull(nameof(candidate));

        // Namespaces...
        ChildNamespaces nsChilds = ChildNamespaces;
        Node nsParent = this;

        BaseNamespaceDeclarationSyntax nsSyntax;
        string nsLongName;
        NamespaceNode nsNode = default!;

        var nsLen = candidate.NamespaceSyntaxChain.Length;
        for (int i = 0; i < nsLen; i++)
        {
            nsSyntax = candidate.NamespaceSyntaxChain[i];
            nsLongName = nsSyntax.Name.LongName();

            nsNode = nsChilds.Find(nsLongName)!;
            if (nsNode == null)
            {
                nsNode = new NamespaceNode(nsParent, nsSyntax);
                nsChilds.Add(nsNode);
            }

            nsChilds = nsNode.ChildNamespaces;
            nsParent = nsNode;
        }

        // Types...
        ChildTypes tpChilds = nsNode.ChildTypes;
        Node tpParent = nsNode;

        ITypeSymbol tpSymbol;
        TypeNode tpNode = default!;

        var tpLen = candidate.TypeSyntaxChain.Length;
        for (int i = 0; i < tpLen; i++)
        {
            tpSymbol = candidate.TypeSymbolChain[i];

            tpNode = tpChilds.Find(tpSymbol)!;
            if (tpNode == null)
            {
                if (i == (tpLen - 1) && candidate is TypeCandidate tpCandidate)
                {
                    tpNode = Generator.CreateType(tpParent, tpCandidate);
                    tpChilds.Add(tpNode);
                }
                else
                {
                    tpNode = new TypeNode(tpParent, tpSymbol);
                    tpChilds.Add(tpNode);
                }
            }

            tpChilds = tpNode.ChildTypes;
            tpParent = tpNode;
        }

        // For type candidates we're done...
        if (candidate is TypeCandidate) return;

        // Properties...
        if (candidate is PropertyCandidate propertyCandidate)
        {
            var node = tpNode.ChildProperties.Find(propertyCandidate.Symbol);
            if (node == null)
            {
                node = Generator.CreateProperty(tpNode, propertyCandidate);
                tpNode.ChildProperties.Add(node);
            }
            return;
        }

        // Fields...
        if (candidate is FieldCandidate fieldCandidate)
        {
            var node = tpNode.ChildFields.Find(fieldCandidate.Symbol);
            if (node == null)
            {
                node = Generator.CreateField(tpNode, fieldCandidate);
                tpNode.ChildFields.Add(node);
            }
            return;
        }

        // Methods...
        if (candidate is MethodCandidate methodCandidate)
        {
            var node = tpNode.ChildMethods.Find(methodCandidate.Symbol);
            if (node == null)
            {
                node = Generator.CreateMethod(tpNode, methodCandidate);
                tpNode.ChildMethods.Add(node);
            }
            return;
        }

        // Unknown...
        throw new ArgumentException(
            "Unknown candidate.")
            .WithData(candidate, nameof(candidate));
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate the contents of this instance, and its child elements, if any.
    /// Returns true if it is valid for source generation purposes, of false otherwise.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        foreach (var node in ChildNamespaces) if (!node.Validate(context)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to append to the given code builder the contents of this instance, and its
    /// child elements, if any.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public override void Print(SourceProductionContext context, CodeBuilder cb)
    {
        cb.AppendLine("// <auto-generated/>");
        cb.AppendLine("#nullable enable");
        cb.AppendLine();
        PrintPragmas(cb);
        PrintUsings(cb);

        var num = 0; foreach (var node in ChildNamespaces)
        {
            if (num > 0) cb.AppendLine();
            num++;
            node.Print(context, cb);
        }
    }

    /// <summary>
    /// Invoked to print the file pragmas, if any.
    /// </summary>
    /// <param name="cb"></param>
    protected virtual void PrintPragmas(CodeBuilder cb)
    {
        var list = new CoreList<string>()
        {
            AcceptDuplicate = (iterator) => false,
            Compare = (x, y) => x == y,
        };

        foreach (var node in ChildNamespaces)
        {
            var syntax = node.Syntax;
            if (syntax == null) continue;

            var tree = syntax.SyntaxTree;
            if (tree.TryGetText(out var text))
            {
                foreach (var line in text.Lines)
                {
                    var str = line.ToString();
                    if (str.StartsWith("pragma")) list.Add(str);
                    if (str.StartsWith("namespace")) break;
                }
            }
        }

        if (list.Count > 0)
        {
            foreach (var item in list) cb.AppendLine(item);
            cb.AppendLine();
        }
    }

    /// <summary>
    /// Invoked to print the file usings, if any.
    /// </summary>
    /// <param name="cb"></param>
    protected virtual void PrintUsings(CodeBuilder cb)
    {
        var list = new CoreList<string>()
        {
            AcceptDuplicate = (item) => false,
            Compare = (x, y) => x == y,
        };
        list.Add("using System;");

        foreach (var node in ChildNamespaces)
        {
            var syntax = node.Syntax;
            if (syntax == null) continue;

            var comp = syntax.GetCompilationUnitSyntax();
            foreach (var item in comp.Usings)
            {
                var str = item.ToString();
                if (str != null && str.Length > 0) list.Add(str);
            }
        }

        if (list.Count > 0)
        {
            foreach (var item in list) cb.AppendLine(item);
            cb.AppendLine();
        }
    }
}