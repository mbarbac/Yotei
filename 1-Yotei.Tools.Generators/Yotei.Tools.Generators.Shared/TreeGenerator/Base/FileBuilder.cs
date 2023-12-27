namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a top-most file node in the source code generation hierarchy.
/// </summary>
internal class FileBuilder : INode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="fileName"></param>
    public FileBuilder(BaseGenerator generator, string fileName)
    {
        Generator = generator.ThrowWhenNull();
        FileName = fileName.NotNullNotEmpty();

        NamespaceChildren = new(this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"File: {FileName}";

    /// <summary>
    /// The generator that created this instance.
    /// </summary>
    public BaseGenerator Generator { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    INode? INode.ParentNode => null;

    /// <summary>
    /// The file name of this instance, without extensions.
    /// </summary>
    public string FileName { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of namespace nodes registered into this instance.
    /// </summary>
    public NamespaceList NamespaceChildren { get; }
    public class NamespaceList : ChildrenList<NamespaceNode>
    {
        public NamespaceList(FileBuilder master)
            : base(master)
            => OnCompare = (item, other) => item.Name == other.Name;
    }

    /// <summary>
    /// Registers the given candidate into this branch of the hierarchy at its appropriate level.
    /// </summary>
    /// <param name="candidate"></param>
    public void Register(Candidate candidate)
    {
        candidate.ThrowWhenNull();
        var comparer = SymbolEqualityComparer.Default;

        // Namespaces...
        INode parent = this;
        NamespaceNode nsNode = null!;
        ChildrenList<NamespaceNode> nsList = NamespaceChildren;
        for (int nsIndex = 0; nsIndex < candidate.NamespaceSyntaxChain.Length; nsIndex++)
        {
            var syntax = candidate.NamespaceSyntaxChain[nsIndex];
            var name = syntax.Name.ToString();
            var index = nsList.IndexOf(x => x.Name == name);

            if (index >= 0) nsNode = nsList[index];
            else
            {
                nsNode = Generator.CreateNode(parent, syntax);
                nsList.Add(nsNode);
            }
            nsList = nsNode.NamespaceChildren;
            parent = nsNode;
        }

        // Types...
        TypeNode tpNode = null!;
        ChildrenList<TypeNode> tpList = nsNode.TypeChildren;
        for (int tpIndex = 0; tpIndex < candidate.TypeSymbolChain.Length; tpIndex++)
        {
            var symbol = candidate.TypeSymbolChain[tpIndex];
            var index = tpList.IndexOf(x => comparer.Equals(symbol, x.Symbol));

            if (index >= 0) tpNode = tpList[index];
            else
            {
                if (candidate is TypeCandidate typeCandidate &&
                    tpIndex == candidate.TypeSyntaxChain.Length - 1)
                {
                    tpNode = Generator.CreateNode(parent, typeCandidate);
                    tpList.Add(tpNode);
                }
                else
                {
                    tpNode = new TypeNode(parent, symbol);
                    tpList.Add(tpNode);
                }
            }
            tpList = tpNode.TypeChildren;
            parent = tpNode;
        }

        // Properties...
        if (candidate is PropertyCandidate propertyCandidate)
        {
            var node = Generator.CreateNode(tpNode, propertyCandidate);
            tpNode.PropertyChildren.Add(node);
            return;
        }

        // Fields...
        if (candidate is FieldCandidate fieldCandidate)
        {
            var node = Generator.CreateNode(tpNode, fieldCandidate);
            tpNode.FieldChildren.Add(node);
            return;
        }

        // Methods...
        if (candidate is MethodCandidate methodCandidate)
        {
            var node = Generator.CreateNode(tpNode, methodCandidate);
            tpNode.MethodChildren.Add(node);
            return;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public bool Validate(SourceProductionContext context)
    {
        foreach (var node in NamespaceChildren) if (!node.Validate(context)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="builder"></param>
    public void Print(SourceProductionContext context, CodeBuilder builder)
    {
        PrintHeaders(builder);
        PrintPragmas(builder);
        PrintUsings(builder);

        foreach (var node in NamespaceChildren) node.Print(context, builder);
    }

    /// <summary>
    /// Emits the file-level headers.
    /// </summary>
    /// <param name="builder"></param>
    static void PrintHeaders(CodeBuilder builder)
    {
        builder.AppendLine("// <auto-generated/>");
        builder.AppendLine("#nullable enable");
        builder.AppendLine();
    }

    /// <summary>
    /// Emits the file-level pragmas.
    /// </summary>
    /// <param name="builder"></param>
    void PrintPragmas(CodeBuilder builder)
    {
        var list = new CustomList<string>()
        {
            OnAcceptDuplicate = (x, y) => false,
            OnCompare = (x, y) => x == y,
        };

        foreach (var node in NamespaceChildren)
        {
            if (node.Syntax == null) continue;

            var tree = node.Syntax.SyntaxTree;
            if (tree.TryGetText(out var text))
            {
                foreach (var line in text.Lines)
                {
                    var str = line.ToString().Trim();
                    if (str.StartsWith("#pragma")) list.Add(str);
                    if (str.StartsWith("namespace")) break;
                }
            }
        }

        if (list.Count > 0)
        {
            foreach (var item in list) builder.AppendLine(item);
            builder.AppendLine();
        }
    }

    /// <summary>
    /// Emits the file-level usings.
    /// </summary>
    /// <param name="builder"></param>
    void PrintUsings(CodeBuilder builder)
    {
        var list = new CustomList<string>()
        {
            OnAcceptDuplicate = (x, y) => false,
            OnCompare = (x, y) => x == y,
        };

        foreach (var node in NamespaceChildren)
        {
            if (node.Syntax == null) continue;

            var comp = node.Syntax.GetCompilationUnitSyntax();
            foreach (var item in comp.Usings)
            {
                var str = item.ToString();
                if (str != null && str.Length > 0) list.Add(str);
            }
        }

        if (list.Count > 0)
        {
            foreach (var item in list) builder.AppendLine(item);
            builder.AppendLine();
        }
    }
}