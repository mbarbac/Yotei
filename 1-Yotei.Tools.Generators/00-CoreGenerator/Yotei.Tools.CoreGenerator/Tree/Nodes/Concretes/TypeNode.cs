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
    public TypeNode(INamedTypeSymbol symbol) => Symbol = symbol;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Type: {Symbol.Name}";

    // ----------------------------------------------------

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    public INamedTypeSymbol Symbol { get; }

    /// <summary>
    /// By default, the collection of syntax nodes where the associated symbol was found, or an
    /// empty one. Its members are by default instances of the following types:
    /// <see cref="EnumDeclarationSyntax"/> and <see cref="TypeDeclarationSyntax"/>.
    /// </summary>
    public List<BaseTypeDeclarationSyntax> SyntaxNodes { get; } = [];

    /// <summary>
    /// By default, the collection of attributes by which the associated symbol was found, or an
    /// empty one.
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance was created only to hold its child elements.
    /// </summary>
    public bool IsChildsOnly { get; init; }

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
    /// Augments the contents of this instance with the ones obtained from the given one. The
    /// default implementation just adds the syntax nodes and attributes to their respective
    /// collections.
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

        foreach (var item in node.ChildEvents)
            if (ChildEvents.Find(x => comparer.Equals(x.Symbol, item.Symbol)) == null)
                ChildEvents.Add(item);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual bool Validate(SourceProductionContext context)
    {
        var r = true;

        if (!Symbol.IsPartial) { Symbol.ReportError(TreeError.TypeNotPartial, context); r = false; }
        if (!IsSupportedKind()) { Symbol.ReportError(TreeError.KindNotSupported, context); r = false; }

        foreach (var node in ChildProperties) if (!node.Validate(context)) r = false;
        foreach (var node in ChildFields) if (!node.Validate(context)) r = false;
        foreach (var node in ChildMethods) if (!node.Validate(context)) r = false;
        foreach (var node in ChildEvents) if (!node.Validate(context)) r = false;

        return r;
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
    /// <inheritdoc/> The '<see cref="EmitCore(SourceProductionContext, CodeBuilder)"/>' and the
    /// '<see cref="EmitChilds(SourceProductionContext, CodeBuilder)"/>' methods can be overriden
    /// as needed.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        var num = EmitPreType(context, cb);

        var head = GetTypeHeader(Symbol);
        var blist = GetTypeBaseList();
        if (blist is not null) head += $" : {blist}";

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
    /// Invoked to emit the pre-type code, as namespaces and containing types. Returns the number
    /// of indentation levels emitted so that they can be decreased afterwards.
    /// </summary>
    int EmitPreType(SourceProductionContext context, CodeBuilder cb)
    {
        cb.AppendLine("// <auto-generated>");
        cb.AppendLine("#nullable enable");
        cb.AppendLine();

        return SyntaxNodes.Count == 0 ? EmitFromSymbol() : EmitFromSymtaxNodes();

        /// <summary>
        /// Invoked to emit the pre-type code when there were no syntax nodes available. In this
        /// case, we can just emit the compound namespace.
        /// </summary>
        int EmitFromSymbol()
        {
            var nschain = Symbol.GetElementChain().Where(static x => x is INamespaceSymbol);
            var name = string.Join(".", nschain.Select(x => x.Name));

            cb.AppendLine($"namespace {name}");
            cb.AppendLine("{");
            cb.IndentLevel++;

            return (1 + EmitTypeParents(context, cb));
        }

        /// <summary>
        /// Invoked to emit the pre-type code when there are syntax nodes available. In this case,
        /// we can emit pragmas and usings.
        /// </summary>
        int EmitFromSymtaxNodes()
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
                    cb.AppendLine($"namespace {name}");
                    cb.AppendLine("{");
                    cb.IndentLevel++;

                    EmitNamespaceUsings(nspace);
                    num++;
                }
            }

            return (num + EmitTypeParents(context, cb));
        }

        /// <summary>
        /// Invoked to populate the given collection of file-level pragmas with the ones obtained
        /// from the elements in the given chain.
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
        /// Invoked to populate the given collection of file-level usings with the ones obtained
        /// from the elements in the given chain.
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
        /// Invoked to emit the usings found at the given namespace level.
        /// </summary>
        void EmitNamespaceUsings(BaseNamespaceDeclarationSyntax ns)
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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the parent types of this one, if any. Returns the number of indentation
    /// levels emitted so that they can be decreased afterwards.
    /// </summary>
    int EmitTypeParents(SourceProductionContext context, CodeBuilder cb)
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
    /// Returns an appropriate header for the given type symbol.
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

    /// <summary>
    /// Invoked to obtain the base list that shall follow the type's name, as in "TBase, IFace..."
    /// or <see langword="null"/> if any is needed. If not null, a semicolon is automatically added
    /// before the returned string.
    /// </summary>
    /// <returns></returns>
    protected string? GetTypeBaseList() => null;

    /// <summary>
    /// Invoked to emit the source code associated with this type per-se, without emitting code
    /// for any child elements. By default this method does nothing.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected virtual void EmitCore(SourceProductionContext context, CodeBuilder cb) { }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code associated with the child elements of this instance, if
    /// any. By default this method invokes the appropriate methods of that child elements.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected virtual void EmitChilds(SourceProductionContext context, CodeBuilder cb)
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
        foreach (var node in ChildEvents)
        {
            if (nl) cb.AppendLine(); nl = true;
            node.Emit(context, cb);
        }
    }
}