namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a type-alike node in the source code generation hierarchy.
/// </summary>
internal class TypeNode : INode
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

    /// <summary>
    /// Obtains the file name where the code of this type, and the code of its child elements,
    /// shall be emitted.
    /// </summary>
    /// <returns></returns>
    public virtual string GetFileName()
    {
        var name = GetTypeFullDisplayName(Symbol);

        List<int> dots = [];
        int depth = 0;
        for (int i = 0; i < name.Length; i++)
        {
            if (name[i] == '<') { depth++; continue; }
            if (name[i] == '>') { depth--; continue; }
            if (name[i] == '.' && depth == 0) dots.Add(i);
        }

        List<string> parts = [];
        int last = 0;

        foreach (var dot in dots)
        {
            parts.Add(name[last..dot]);
            last = dot + 1;
        }
        parts.Add(name[last..]);

        parts.Reverse();
        return string.Join(".", parts);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance was created just to hold its child elements. If not, it means
    /// the associated type was identified by its own (even if this instance contains other child
    /// elements).
    /// </summary>
    public bool IsHolderOnly { get; init; }

    /// <summary>
    /// The symbol captured for this instance.
    /// </summary>
    public INamedTypeSymbol Symbol { get; }

    /// <summary>
    /// The collection of syntax nodes captured for this instance, or an empty one if any.
    /// </summary>
    public List<BaseTypeDeclarationSyntax> SyntaxNodes { get; } = [];

    /// <summary>
    /// The collection of attributes by which this candidate was identified
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];

    // ----------------------------------------------------

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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to add to the contents of this node with the information obtained from the given
    /// candidate. This method is invoked by the hierarchy-creation process when a node for the
    /// element already exist in that hierarchy.
    /// </summary>
    /// <param name="candidate"></param>
    public virtual void Augment(TypeCandidate candidate)
    {
        if (candidate.Syntax is not null)
            if (SyntaxNodes.Find(x => x.IsEquivalentTo(candidate.Syntax)) == null)
                SyntaxNodes.Add(candidate.Syntax);

        foreach (var at in candidate.Attributes)
            if (Attributes.Find(x => x.EqualsTo(at)) == null)
                Attributes.Add(at);
    }

    /// <summary>
    /// Invoked to add to the contents of this node with the information obtained from the given
    /// node. This method is invoked by the hierarchy-creation process when a node for the element
    /// already exist in that hierarchy.
    /// </summary>
    /// <param name="node"></param>
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
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual bool Validate(SourceProductionContext context)
    {
        if (!Symbol.IsPartial) { Symbol.ReportError(CoreError.TypeNotPartial, context); return false; }
        if (!IsSupportedKind()) { Symbol.ReportError(CoreError.KindNotSupported, context); return false; }

        foreach (var node in ChildProperties) if (!node.Validate(context)) return false;
        foreach (var node in ChildFields) if (!node.Validate(context)) return false;
        foreach (var node in ChildMethods) if (!node.Validate(context)) return false;

        return true;
    }

    /// <summary>
    /// Determines if this type is of a supported kind, or not.
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsSupportedKind() => Symbol.TypeKind is
        TypeKind.Class or
        TypeKind.Struct or
        TypeKind.Interface;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/> This method is not intended to be overriden.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        var num = EmitPreType(cb);
        var head = GetHeader();
        cb.AppendLine(head);

        cb.AppendLine("{");
        cb.IndentLevel++;

        var old = cb.Length; EmitCore(context, cb);
        var len = cb.Length; if (old != len) cb.AppendLine();
        EmitChilds(context, cb);

        cb.IndentLevel--;
        cb.AppendLine("}");

        for (int i = 0; i < num; i++)
        {
            cb.IndentLevel--;
            cb.AppendLine("}");
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to obtain the header string of this type, which by default is 'partial {typename}'.
    /// If overriden, inheritors must call this base method and then add a '... : TBase, IFace, ...'
    /// base list as needed.
    /// </summary>
    /// <returns></returns>
    protected virtual string GetHeader() => GetTypeHeader(Symbol);

    /// <summary>
    /// Invoked to obtain the 'partial {typename}' header for the given type symbol.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string GetTypeHeader(INamedTypeSymbol symbol)
    {
        var rec = symbol.IsRecord ? "record " : string.Empty;
        string kind = symbol.TypeKind switch
        {
            TypeKind.Class => "class",
            TypeKind.Struct => "struct",
            TypeKind.Interface => "interface",
            _ => throw new ArgumentException("Type kind not supported.").WithData(symbol.Name)
        };
        var name = GetTypeFullDisplayName(symbol);
        return $"partial {rec}{kind} {name}";
    }

    /// <summary>
    /// Returns the full display name of the given type symbol.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string GetTypeFullDisplayName(INamedTypeSymbol symbol)
    {
        var options = new SymbolDisplayFormat(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        return symbol.ThrowWhenNull().ToDisplayString(options);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code associated with this type, but not the one of its child
    /// elements, if any. This method is invoked *BEFORE* emitting the child elements.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected virtual void EmitCore(SourceProductionContext context, CodeBuilder cb) { }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code of the known childs of this type, if any.
    /// </summary>
    void EmitChilds(SourceProductionContext context, CodeBuilder cb)
    {
        var nl = false;

        foreach (var node in ChildFields)
        {
            if (nl) cb.AppendLine(); nl = true;
            node.Emit(context, cb);
        }
        foreach (var node in ChildProperties)
        {
            if (nl) cb.AppendLine(); nl = true;
            node.Emit(context, cb);
        }
        foreach (var node in ChildMethods)
        {
            if (nl) cb.AppendLine(); nl = true;
            node.Emit(context, cb);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the pre-type code. Returns the number of indentation levels it has opened
    /// so that they can be closed afterwards.
    /// </summary>
    int EmitPreType(CodeBuilder cb)
    {
        cb.AppendLine("// <auto-generated>");
        cb.AppendLine("#nullable enable");
        cb.AppendLine();

        return SyntaxNodes.Count > 0 ? EmitPreTypeFromSyntaxes(cb) : EmitPreTypeFromSymbol(cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the pre-type code when no syntaxes are available. In this case, we just
    /// emit the compound namespace, with no pragmas or usings. Returns the number of indentation
    /// levels it has opened so that they can be closed afterwards.
    /// </summary>
    int EmitPreTypeFromSymbol(CodeBuilder cb)
    {
        var nschain = Symbol.GetElementChain().Where(static x => x is INamespaceSymbol);
        var first = true;
        var sb = new StringBuilder(); foreach (var ns in nschain)
        {
            if (!first) sb.Append('.'); first = false;
            sb.Append(ns.Name);
        }

        cb.AppendLine("namespace " + sb.ToString());
        cb.AppendLine("{");
        cb.IndentLevel++;

        return (1 + EmitTypeParents(cb));
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the pre-type code when there are syntaxes are available so that we can
    /// capture and emit the appropriate pragmas and usings. Returns the number of indentation
    /// levels it has opened so that they can be closed afterwards.
    /// </summary>
    int EmitPreTypeFromSyntaxes(CodeBuilder cb)
    {
        // Emitting file-level pragmas and usings...
        List<string> pragmas = [];
        List<string> usings = [];
        foreach (var syntax in SyntaxNodes)
        {
            var nschain = syntax.GetElementChain().Where(static x => x is BaseNamespaceDeclarationSyntax);
            PopulateFileLevelPragmas(pragmas, nschain);
            PopulateFileLevelUsings(usings, nschain);
        }
        if (pragmas.Count > 0) { foreach (var str in pragmas) cb.AppendLine(str); cb.AppendLine(); }
        if (usings.Count > 0) { foreach (var str in usings) cb.AppendLine(str); cb.AppendLine(); }

        // Emitting namespaces...
        var num = 0;
        foreach (var syntax in SyntaxNodes)
        {
            var nschain = syntax.GetElementChain().Where(static x => x is BaseNamespaceDeclarationSyntax);
            foreach (var ns in nschain)
            {
                var nspace = (BaseNamespaceDeclarationSyntax)ns;
                var name = nspace.Name.ToString();
                cb.AppendLine("namespace " + name);
                cb.AppendLine("{");
                cb.IndentLevel++;

                EmitNamespaceUsings(nspace, cb);
                num++;
            }
        }

        return (num + EmitTypeParents(cb));
    }

    /// <summary>
    /// Invoked to add the the collection the file-level pragmas from the given chain.
    /// </summary>
    void PopulateFileLevelPragmas(List<string> items, IEnumerable<SyntaxNode> nschain)
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
    /// Invoked to add the the collection the file-level usings from the given chain.
    /// </summary>
    void PopulateFileLevelUsings(List<string> items, IEnumerable<SyntaxNode> nschain)
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
    /// Invoked to emit the collection of namespace-level usings.
    /// </summary>
    void EmitNamespaceUsings(
        BaseNamespaceDeclarationSyntax ns, CodeBuilder cb)
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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the pre-type code for the parent types of this type. Returns the number of
    /// indentation levels it has opened so that they can be closed afterwards.
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
}