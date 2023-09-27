namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a tree-oriented incremental source code generator that generates a hierarchy of
/// nodes for which source code shall be generated. Concrete classes that inherit from this one
/// must be decorated with a '<see cref="GeneratorAttribute"/><c>(LanguageNames.CSharp)</c>'
/// attribute to be picked-up and used by the compiler.
/// </summary>
internal abstract class TreeGenerator : IIncrementalGenerator
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    public virtual void Initialize(IncrementalGeneratorInitializationContext context)
    {
        if (LaunchDebugger && !Debugger.IsAttached) Debugger.Launch();

        context.RegisterPostInitializationOutput(PostInitialize);

        var items = context.SyntaxProvider
            .CreateSyntaxProvider(Validate, Transform)
            .Collect();

        context.RegisterSourceOutput(items, Print);
    }

    /// <summary>
    /// Determines if this instance will try to launch a debugging session when invoked by the
    /// compiler.
    /// </summary>
    protected virtual bool LaunchDebugger => false;

    /// <summary>
    /// Invoked to register post-initialization actions, such as reading external files, emitting
    /// code for custom attributes, and so forth.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void PostInitialize(IncrementalGeneratorPostInitializationContext context) { }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to determine if the given node shall be considered for source code generation, or
    /// not. The default behavior of this method is to validate if the node is decorated with any
    /// attribute whose name is among the ones defined in this instance for its type.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual bool Validate(SyntaxNode node, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        // Types...
        if (node is TypeDeclarationSyntax typeSyntax)
        {
            if (TypeAttributes.Length > 0)
            {
                var atts = typeSyntax.AttributeLists.GetAttributes();
                return atts.Any(x => Compare(x.Name, TypeAttributes));
            }
            return false;
        }

        // Properties...
        if (node is PropertyDeclarationSyntax propertySyntax)
        {
            if (PropertyAttributes.Length > 0)
            {
                var atts = propertySyntax.AttributeLists.GetAttributes();
                return atts.Any(x => Compare(x.Name, PropertyAttributes));
            }
            return false;
        }

        // Fields...
        if (node is FieldDeclarationSyntax fieldSyntax)
        {
            if (FieldAttributes.Length > 0)
            {
                var atts = fieldSyntax.AttributeLists.GetAttributes();
                return atts.Any(x => Compare(x.Name, FieldAttributes));
            }
            return false;
        }

        // Methods...
        if (node is MethodDeclarationSyntax methodSyntax)
        {
            if (MethodAttributes.Length > 0)
            {
                var atts = methodSyntax.AttributeLists.GetAttributes();
                return atts.Any(x => Compare(x.Name, MethodAttributes));
            }
            return false;
        }

        // Not supported...
        return false;

        /// <summary>
        /// Determines if the given syntax name matches any of the given attribute names, or not.
        /// </summary>
        static bool Compare(NameSyntax syntax, string[] names)
        {
            var span = "Attribute".AsSpan();
            var orig = syntax.ShortName().AsSpan().RemoveLast(span);

            for (int i = 0; i < names.Length; i++)
            {
                var name = names[i].AsSpan().RemoveLast(span);
                if (orig.CompareTo(name, StringComparison.OrdinalIgnoreCase) == 0) return true;
            }

            return false;
        }
    }

    /// <summary>
    /// The collection of attribute names that, if decorates a given type, marks it as a suitable
    /// candidate for source code generation.
    /// </summary>
    protected virtual string[] TypeAttributes => Array.Empty<string>();

    /// <summary>
    /// The collection of attribute names that, if decorates a given property, marks it as a suitable
    /// candidate for source code generation.
    /// </summary>
    protected virtual string[] PropertyAttributes => Array.Empty<string>();

    /// <summary>
    /// The collection of attribute names that, if decorates a given field, marks it as a suitable
    /// candidate for source code generation.
    /// </summary>
    protected virtual string[] FieldAttributes => Array.Empty<string>();

    /// <summary>
    /// The collection of attribute names that, if decorates a given method, marks it as a suitable
    /// candidate for source code generation.
    /// </summary>
    protected virtual string[] MethodAttributes => Array.Empty<string>();

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to transform a validated syntax node into an equivalent candidate.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual Candidate Transform(GeneratorSyntaxContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var node = context.Node;
        var model = context.SemanticModel;

        // Types...
        if (node is TypeDeclarationSyntax typeSyntax)
        {
            var symbol = model.GetDeclaredSymbol(typeSyntax);
            if (symbol != null) return new TypeCandidate(model, typeSyntax, symbol);

            throw new NotFoundException(
                "No symbold found for the given type candidate.")
                .WithData(typeSyntax.Identifier.Text, nameof(typeSyntax));
        }

        // Properties...
        if (node is PropertyDeclarationSyntax propertySyntax)
        {
            var symbol = model.GetDeclaredSymbol(propertySyntax);
            if (symbol != null) return new PropertyCandidate(model, propertySyntax, symbol);

            throw new NotFoundException(
                "No symbold found for the given property candidate.")
                .WithData(propertySyntax.Identifier.Text, nameof(propertySyntax));
        }

        // Fields...
        if (node is FieldDeclarationSyntax fieldSyntax)
        {
            var items = fieldSyntax.Declaration.Variables;
            foreach (var item in items)
            {
                var symbol = model.GetDeclaredSymbol(item) as IFieldSymbol;
                if (symbol != null) return new FieldCandidate(model, fieldSyntax, symbol);
            }

            throw new NotFoundException(
                "No symbold found for the given field candidate.")
                .WithData(fieldSyntax.Declaration.Variables[0].Identifier.Text, nameof(fieldSyntax));
        }

        // Methods...
        if (node is MethodDeclarationSyntax methodSyntax)
        {
            var symbol = model.GetDeclaredSymbol(methodSyntax);
            if (symbol != null) return new MethodCandidate(model, methodSyntax, symbol);

            throw new NotFoundException(
                "No symbold found for the given method candidate.")
                .WithData(methodSyntax.Identifier.Text, nameof(methodSyntax));
        }

        // Not supported...
        throw new UnExpectedException(
            "Unsupported syntax node.").WithData(node, nameof(node));
    }

    // ----------------------------------------------------

    /// <summary>
    /// Creates a new property-alike node for this generator, based on the given symbol.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public virtual TypeNode CreateType(
        Node parent, ITypeSymbol symbol) => new(parent, symbol);

    /// <summary>
    /// Creates a new property-alike node for this generator, based on the given candidate.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public virtual TypeNode CreateType(
        Node parent, TypeCandidate candidate) => new(parent, candidate);

    /// <summary>
    /// Creates a new property-alike node for this generator, based on the given candidate.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public virtual PropertyNode CreateProperty(
        TypeNode parent, PropertyCandidate candidate) => new(parent, candidate);

    /// <summary>
    /// Creates a new field-alike node for this generator, based on the given candidate.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public virtual FieldNode CreateField(
        TypeNode parent, FieldCandidate candidate) => new(parent, candidate);

    /// <summary>
    /// Creates a new method-alike node for this generator, based on the given candidate.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public virtual MethodNode CreateMethod(
        TypeNode parent, MethodCandidate candidate) => new(parent, candidate);

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create the hierarchy of the given collection of candidates, and then to emit the
    /// corresponding source code.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="candidates"></param>
    protected virtual void Print(SourceProductionContext context, ImmutableArray<Candidate> candidates)
    {
        var files = new ChildFiles();

        // Hierarchy...
        foreach (var candidate in candidates)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var name = GetFileName(candidate);
            var node = files.Find(name);
            if (node == null)
            {
                node = new FileNode(this, name);
                files.Add(node);
            }
            node.Register(candidate);
        }

        // Source code...
        foreach (var node in files)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (!node.Validate(context)) continue;

            var cb = new CodeBuilder();
            node.Print(context, cb);

            var name = node.FileName + ".g.cs";
            var code = cb.ToString();
            context.AddSource(name, code);
        }
    }

    /// <summary>
    /// Invoked to obtain the file where the source code of the given candidate shall be emitted.
    /// </summary>
    /// <param name="candidate"></param>
    protected virtual string GetFileName(Candidate candidate)
    {
        return candidate.GetTailTypeFile();
    }
}