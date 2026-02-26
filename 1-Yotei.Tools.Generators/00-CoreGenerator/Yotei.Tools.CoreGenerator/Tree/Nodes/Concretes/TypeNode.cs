namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a type-alike source code generation element.
/// </summary>
internal class TypeNode : ITreeNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public TypeNode(INamedTypeSymbol symbol) => Symbol = symbol.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Type: {Symbol.Name}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    INode ITreeNode.ParentNode => null!;

    /// <summary>
    /// <inheritdoc cref="ITreeNode.Symbol"/>
    /// </summary>
    public INamedTypeSymbol Symbol { get; }
    ISymbol ITreeNode.Symbol => Symbol;

    /// <summary>
    /// <inheritdoc cref="ITreeNode.SyntaxNodes"/>
    /// </summary>
    public List<BaseTypeDeclarationSyntax> SyntaxNodes { get; } = [];
    List<SyntaxNode> ITreeNode.SyntaxNodes
        => (List<SyntaxNode>)SyntaxNodes.Cast<BaseTypeDeclarationSyntax>();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance was created for the sole purpose of holding child elements,
    /// and not because it was identified as a source code generation element by itself.
    /// </summary>
    public bool ChildsOnly { get; init; }

    /// <summary>
    /// The collection of child properties known to this instance.
    /// </summary>
    public List<PropertyNode> ChildProperties { get; } = [];

    /// <summary>
    /// The collection of child fields known to this instance.
    /// </summary>
    public List<FieldNode> ChildFields { get; } = [];

    /// <summary>
    /// The collection of child methods known to this instance.
    /// </summary>
    public List<MethodNode> ChildMethods { get; } = [];

    /// <summary>
    /// The collection of child events known to this instance.
    /// </summary>
    public List<EventNode> ChildEvents { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual void Augment(TypeNode node)
    {
        foreach (var syntax in node.SyntaxNodes)
            if (SyntaxNodes.Find(x => x.IsEquivalentTo(syntax)) == null)
                SyntaxNodes.Add(syntax);

        foreach (var at in node.Attributes)
            if (Attributes.Find(x => x.EqualsTo(at)) == null)
                Attributes.Add(at);

        var comparer = SymbolEqualityComparer.Default;

        foreach (var item in node.ChildProperties)
            if (ChildProperties.Find(x => comparer.Equals(x.Symbol, item.Symbol)) == null)
                ChildProperties.Add(item);

        foreach (var item in node.ChildFields)
            if (ChildFields.Find(x => comparer.Equals(x.Symbol, item.Symbol)) == null)
                ChildFields.Add(item);

        foreach (var item in node.ChildMethods)
            if (ChildMethods.Find(x => comparer.Equals(x.Symbol, item.Symbol)) == null)
                ChildMethods.Add(item);

        foreach (var item in node.ChildEvents)
            if (ChildEvents.Find(x => comparer.Equals(x.Symbol, item.Symbol)) == null)
                ChildEvents.Add(item);
    }
    void ITreeNode.Augment(ITreeNode node) => Augment((TypeNode)node);

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this type is of a supported kind, or not.
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsSupportedKind() => Symbol.TypeKind is
        TypeKind.Class or
        TypeKind.Struct or
        TypeKind.Interface;

    /// <summary>
    /// Invoked to validate this node at source code generation time. If this method returns
    /// <see langword="false"/>, source code generation is aborted. Derived types must invoke
    /// their base ones first.
    /// <br/> Validation of the child nodes this one may carry will happen at their respective
    /// source code generation times.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual bool OnValidate(SourceProductionContext context)
    {
        var r = true;
        if (!Symbol.IsPartial) { Symbol.ReportError(TreeError.TypeNotPartial, context); r = false; }
        if (!IsSupportedKind()) { Symbol.ReportError(TreeError.KindNotSupported, context); r = false; }

        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to obtain a string with the  base list that shall follow the type's name (as in
    /// 'Name : TBase, IFace1, ...'), or <see langword="null"/> if not needed (which is what is
    /// returned by default). Derived types can override this method as needed.
    /// </summary>
    /// <returns></returns>
    protected virtual string? GetBaseList() => null;

    /// <summary>
    /// Invoked to emit the source code of this type per-se, so not dealing with its headers, base
    /// list, or with any child element this node may carry. By default this method does nothing.
    /// Derived types can override this method as needed.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    protected virtual bool OnEmitCore(SourceProductionContext context, CodeBuilder cb) => true;

    /// <summary>
    /// Invoked to emit the source code of the child elements carried by this node, if any. If
    /// derived types override this method, then they typically shall take complete control of
    /// this process.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    protected virtual bool OnEmitChilds(SourceProductionContext context, CodeBuilder cb)
    {
        var r = true;
        var n = false;

        foreach (var node in ChildProperties)
        {
            if (n) cb.AppendLine(); n = true;
            if (!node.Emit(context, cb)) r = false;
        }

        foreach (var node in ChildFields)
        {
            if (n) cb.AppendLine(); n = true;
            if (!node.Emit(context, cb)) r = false;
        }

        foreach (var node in ChildMethods)
        {
            if (n) cb.AppendLine(); n = true;
            if (!node.Emit(context, cb)) r = false;
        }

        foreach (var node in ChildEvents)
        {
            if (n) cb.AppendLine(); n = true;
            if (!node.Emit(context, cb)) r = false;
        }

        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool Emit(SourceProductionContext context, CodeBuilder cb)
    {
        var num = EmitAllParents(cb);

        var head = GetTypeHeader(Symbol);
        var str = GetBaseList();
        if (str != null) head += $" : {str}";

        cb.AppendLine(head);
        cb.AppendLine("{");
        cb.IndentLevel++;

        var old = cb.Length;
        var r = OnValidate(context) && OnEmitCore(context, cb);
        if (r)
        {
            var len = cb.Length; if (old != len) cb.AppendLine();
            if (!OnEmitChilds(context, cb)) r = false;
        }

        cb.IndentLevel--;
        cb.AppendLine("}");

        for (int i = 0; i < num; i++)
        {
            cb.IndentLevel--;
            cb.AppendLine("}");
        }

        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns an appropriate header for the given type.
    /// </summary>
    static string GetTypeHeader(INamedTypeSymbol symbol)
    {
        var rec = symbol.IsRecord ? "record " : string.Empty;
        string kind = symbol.TypeKind switch
        {
            TypeKind.Class => "class",
            TypeKind.Struct => "struct",
            TypeKind.Interface => "interface",
            _ => throw new ArgumentException("Type kind not supported.").WithData(symbol.Name)
        };

        var name = symbol.EasyName(new()
        {
            UseSpecialNames = true,
            NullableStyle = IsNullableStyle.UseAnnotations,
            GenericOptions = new()
            {
                NamespaceOptions = new() { UseHostNamespace = true },
                UseSpecialNames = true,
                NullableStyle = IsNullableStyle.UseAnnotations,
            }
        });
        return $"partial {rec}{kind} {name}";
    }

    // ====================================================

    /// <summary>
    /// Invoked to emit the namespaces and containing types of this one. Returns the number of
    /// indentation levels emitted in the out argument, so they can be decreased afterwards.
    /// </summary>
    int EmitAllParents(CodeBuilder cb)
    {
        cb.AppendLine("// <auto-generated>");
        cb.AppendLine("#nullable enable");
        cb.AppendLine();

        return SyntaxNodes.Count == 0 ? EmitFromSymbol() : EmitFromSyntaxes();

        /// <summary>
        /// Invoked to emit the namespaces and containing types when there are no syntax nodes
        /// available.
        /// </summary>
        int EmitFromSymbol()
        {
            // In this we can only emit a compound namespace...
            var nschain = Symbol.GetElementChain().Where(static x => x is INamespaceSymbol);
            var name = string.Join(".", nschain.Select(x => x.Name));

            cb.AppendLine($"namespace {name}");
            cb.AppendLine("{");
            cb.IndentLevel++;

            // Finishing...
            var num = 1 + EmitTypeParents(cb);
            return num;
        }

        /// <summary>
        /// Invoked to emit the namespaces and containing types when there are known syntax nodes.
        /// </summary>
        int EmitFromSyntaxes()
        {
            // Emitting file-level pragmas and usings...
            List<string> pragmas = [];
            List<string> usings = [];

            foreach (var syntax in SyntaxNodes)
            {
                var nschain = syntax
                    .GetElementChain()
                    .Where(static x => x is BaseNamespaceDeclarationSyntax);
                
                PopulateFileLevelPragmas(pragmas, nschain);
                PopulateFileLevelUsings(usings, nschain);
            }

            if (pragmas.Count > 0) { foreach (var str in pragmas) cb.AppendLine(str); cb.AppendLine(); }
            if (usings.Count > 0) { foreach (var str in usings) cb.AppendLine(str); cb.AppendLine(); }

            // Emitting namespaces...
            var num = 0;
            foreach (var syntax in SyntaxNodes)
            {
                var nschain = syntax
                    .GetElementChain()
                    .Where(static x => x is BaseNamespaceDeclarationSyntax);
                
                foreach (var ns in nschain)
                {
                    var nspace = (BaseNamespaceDeclarationSyntax)ns;
                    var name = nspace.Name.ToString();
                    cb.AppendLine($"namespace {name}");
                    cb.AppendLine("{");
                    cb.IndentLevel++;

                    EmitNamespaceUsings(nspace, cb);
                    num++;
                }
            }

            // Finishing...
            num += EmitTypeParents(cb);
            return num;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the containing types of this one. Returns the number of indentation
    /// levels emitted in the out argument, so they can be decreased afterwards.
    /// </summary>
    int EmitTypeParents(CodeBuilder cb)
    {
        var tpchain = Symbol.GetElementChain().Where(static x => x is INamedTypeSymbol).ToList();
        tpchain = tpchain.GetRange(0, tpchain.Count - 1);

        for (int i = 0; i < tpchain.Count; i++)
        {
            var type = (INamedTypeSymbol)tpchain[i];
            var head = GetTypeHeader(type);

            cb.AppendLine(head);
            cb.AppendLine("{");
            cb.IndentLevel++;
        }

        return tpchain.Count;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Involed to populate the given collection of file-level pragmas, using the elements in
    /// the given chain.
    /// </summary>
    static void PopulateFileLevelPragmas(List<string> items, IEnumerable<SyntaxNode> nschain)
    {
        foreach (var ns in nschain)
        {
            if (ns.SyntaxTree.TryGetText(out var text))
            {
                foreach (var line in text.Lines)
                {
                    var str = line.ToString().Trim();
                    if (str.StartsWith("#pragma") && !items.Contains(str)) items.Add(str);
                    else if (str.StartsWith("namespace")) break;
                }
            }
        }
    }

    /// <summary>
    /// Involed to populate the given collection of file-level usings, using the elements in
    /// the given chain.
    /// </summary>
    static void PopulateFileLevelUsings(List<string> items, IEnumerable<SyntaxNode> nschain)
    {
        foreach (var ns in nschain)
        {
            var comp = GetCompilation(ns);
            if (comp is null) continue;

            foreach (var item in comp.Usings)
            {
                var str = item.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(str) && !items.Contains(str)) items.Add(str);
            }
        }

        // Gets the compilation unit syntax the given syntax node belongs to...
        static CompilationUnitSyntax? GetCompilation(SyntaxNode node)
        {
            while (node is not null)
            {
                if (node is CompilationUnitSyntax comp) return comp;
                node = node.Parent!;
            }
            return null;
        }
    }

    /// <summary>
    /// Invoked to **emit** the usings found at the given namespace's level.
    /// </summary>
    /// <param name="ns"></param>
    /// <param name="cb"></param>
    static void EmitNamespaceUsings(BaseNamespaceDeclarationSyntax ns, CodeBuilder cb)
    {
        List<string> items = []; foreach (var item in ns.Usings)
        {
            var str = item.ToString().Trim();
            if (!string.IsNullOrWhiteSpace(str) && !items.Contains(str)) items.Add(str);
        }

        if (items.Count > 0)
        {
            foreach (var str in items) cb.AppendLine(str);
            cb.AppendLine();
        }
    }
}