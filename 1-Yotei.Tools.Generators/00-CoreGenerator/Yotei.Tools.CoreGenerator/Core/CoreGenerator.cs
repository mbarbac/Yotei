namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents an incremental source code generator that, by default, uses the types, properties,
/// fields and methods decorated with any of the specified attributes (or attribute names). Once
/// captured, these candidates are arranged in a file-based collection where each file represents
/// a given type (identified by its attributes, and/or needed because its child elements). All
/// these defaults can be modified and overriden as needed.
/// <para>
/// Inheritor classes shall be decorated with the <see cref="GeneratorAttribute"/> attribute to be
/// recognized by the compiler. It is expected that the <see cref="LanguageNames.CSharp"/> value is
/// used as its argument.
/// </para>
/// </summary>
internal class CoreGenerator : IIncrementalGenerator
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Launching a compile-time debug session if needed...
        if (LaunchDebugger && !Debugger.IsAttached) Debugger.Launch();

        // Registering post-initialization actions....
        context.RegisterPostInitializationOutput(OnInitialize);

        // Registering pipeline steps...
        var items = context.SyntaxProvider
            .CreateSyntaxProvider(ConsiderCandidate, CaptureCandidate)
            .Where(static x => x is not null)
            .Collect();

        var combined = context.CompilationProvider.Combine(items);

        // Registering source code emit actions...
        context.RegisterSourceOutput(combined, EmitCode);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance tries to launch a compile-time debug session when compiling.
    /// <br/> The default value of this property is <see langword="false"/>.
    /// </summary>
    protected virtual bool LaunchDebugger => false;

    /// <summary>
    /// If true, emits the <see cref="IsNullable{T}"/> and <see cref="IsNullableAttribute"/> types
    /// on the namespace of the generator.
    /// <br/> The default value of this property is <see langword="false"/>.
    /// </summary>
    protected virtual bool EmitTypeNullabilityHelpers => false;

    /// <summary>
    /// Invoked to register post-initialization actions, such as reading external files, or
    /// generating code for marker attributes.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        if (EmitTypeNullabilityHelpers)
        {
            var ns = GetType().Namespace!;
            var str = $$"""
                using System;

                namespace {{ns}}
                {
                    /// <summary>
                    /// <inheritdoc cref="Yotei.Tools.CoreGenerator.IsNullable{T}"/>
                    /// </summary>
                    public class IsNullable<T> { }
                    
                    /// <summary>
                    /// <inheritdoc cref="IsNullable{T}"/>
                    /// </summary>
                    [AttributeUsage(AttributeTargets.All)]
                    public class IsNullableAttribute : Attribute { }
                }
                """;

            context.AddEmbeddedAttributeDefinition();
            context.AddSource("IsNullable[T].g.cs", str);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if type-kind syntax elements shall be considered as potential candidates.
    /// <br/> The default value of this property is <see langword="false"/>.
    /// </summary>
    protected virtual bool ConsiderTypeCandidates => false;

    /// <summary>
    /// Determines if type-kind syntax elements shall be considered as potential candidates.
    /// <br/> The default value of this property is <see langword="false"/>.
    /// </summary>
    protected virtual bool ConsiderPropertyCandidates => false;

    /// <summary>
    /// Determines if type-kind syntax elements shall be considered as potential candidates.
    /// <br/> The default value of this property is <see langword="false"/>.
    /// </summary>
    protected virtual bool ConsiderFieldCandidates => false;

    /// <summary>
    /// Determines if type-kind syntax elements shall be considered as potential candidates.
    /// <br/> The default value of this property is <see langword="false"/>.
    /// </summary>
    protected virtual bool ConsiderMethodCandidates => false;

    /// <summary>
    /// Invoked to quickly determine if the given syntax node shall be considered as a potential
    /// candidate for source code generation, or not. Typically, this method just filters out all
    /// syntax kinds except the ones the generator is interested in.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual bool ConsiderCandidate(SyntaxNode node, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        return
            (node is TypeDeclarationSyntax && ConsiderTypeCandidates) ||
            (node is PropertyDeclarationSyntax && ConsiderPropertyCandidates) ||
            (node is FieldDeclarationSyntax && ConsiderFieldCandidates) ||
            (node is MethodDeclarationSyntax && ConsiderMethodCandidates);
    }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of attribute types this generator uses, by default, to identify decorated
    /// types as source code generation candidates.
    /// </summary>
    protected virtual List<Type> TypeAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types this generator uses, by default, to identify decorated
    /// properties as source code generation candidates.
    /// </summary>
    protected virtual List<Type> PropertyAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types this generator uses, by default, to identify decorated
    /// fields as source code generation candidates.
    /// </summary>
    protected virtual List<Type> FieldAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types this generator uses, by default, to identify decorated
    /// methods as source code generation candidates.
    /// </summary>
    protected virtual List<Type> MethodAttributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// The collection of fully qualified attribute type names used by this generator, by default,
    /// to identify decorated types as source code generation candidates. This default generator
    /// uses these names only if there is no match for the specified types, if any.
    /// </summary>
    protected virtual List<string> TypeAttributeNames { get; } = [];

    /// <summary>
    /// The collection of fully qualified attribute type names used by this generator, by default,
    /// to identify decorated properties as source code generation candidates. This default generator
    /// uses these names only if there is no match for the specified types, if any.
    /// </summary>
    protected virtual List<string> PropertyAttributeNames { get; } = [];

    /// <summary>
    /// The collection of fully qualified attribute type names used by this generator, by default,
    /// to identify decorated fields as source code generation candidates. This default generator
    /// uses these names only if there is no match for the specified types, if any.
    /// </summary>
    protected virtual List<string> FieldAttributeNames { get; } = [];

    /// <summary>
    /// The collection of fully qualified attribute type names used by this generator, by default,
    /// to identify decorated methods as source code generation candidates. This default generator
    /// uses these names only if there is no match for the specified types, if any.
    /// </summary>
    protected virtual List<string> MethodAttributeNames { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a source code generation candidate of the appropriate kind.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual TypeCandidate CreateCandidate(
        INamedTypeSymbol symbol,
        TypeDeclarationSyntax? syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new TypeCandidate(symbol) { Syntax = syntax };
        item.Attributes.AddRange(attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a source code generation candidate of the appropriate kind.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual PropertyCandidate CreateCandidate(
        IPropertySymbol symbol,
        BasePropertyDeclarationSyntax? syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new PropertyCandidate(symbol) { Syntax = syntax };
        item.Attributes.AddRange(attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a source code generation candidate of the appropriate kind.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual FieldCandidate CreateCandidate(
        IFieldSymbol symbol,
        FieldDeclarationSyntax? syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new FieldCandidate(symbol) { Syntax = syntax };
        item.Attributes.AddRange(attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a source code generation candidate of the appropriate kind.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual MethodCandidate CreateCandidate(
        IMethodSymbol symbol,
        BaseMethodDeclarationSyntax? syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new MethodCandidate(symbol) { Syntax = syntax };
        item.Attributes.AddRange(attributes);
        return item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture a candidate for source code generation purposes using the given context.
    /// This method may also return <see langword="null"/> if the syntax node carried by the context
    /// shall be ignored, or an error candidate if there is the need to report an error condition.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0019")]
    protected virtual ICandidate CaptureCandidate(GeneratorSyntaxContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var syntax = context.Node;
        var model = context.SemanticModel;

        // Types...
        while (syntax is TypeDeclarationSyntax membersyntax)
        {
            var symbol = model.GetDeclaredSymbol(membersyntax, token);
            if (symbol is null) break;

            var ats = FindAttributes(symbol, membersyntax, TypeAttributes, TypeAttributeNames);
            if (ats.Count == 0) break;

            var candidate = CreateCandidate(symbol, membersyntax, ats, model);
            return candidate;
        }

        // Properties...
        while (syntax is BasePropertyDeclarationSyntax membersyntax)
        {
            var symbol = model.GetDeclaredSymbol(membersyntax, token) as IPropertySymbol;
            if (symbol is null) break;

            var ats = FindAttributes(symbol, membersyntax, PropertyAttributes, PropertyAttributeNames);
            if (ats.Count == 0) break;

            var candidate = CreateCandidate(symbol, membersyntax, ats, model);
            return candidate;
        }

        // Fields...
        while (syntax is FieldDeclarationSyntax membersyntax)
        {
            var items = membersyntax.Declaration.Variables;
            foreach (var item in items)
            {
                var symbol = model.GetDeclaredSymbol(item, token) as IFieldSymbol;
                if (symbol is null) break;

                var ats = FindAttributes(symbol, membersyntax, FieldAttributes, FieldAttributeNames);
                if (ats.Count == 0) break;

                var candidate = CreateCandidate(symbol, membersyntax, ats, model);
                return candidate;
            }
            break;
        }

        // Methods...
        while (syntax is BaseMethodDeclarationSyntax membersyntax)
        {
            var symbol = model.GetDeclaredSymbol(membersyntax, token);
            if (symbol is null) break;

            var ats = FindAttributes(symbol, membersyntax, MethodAttributes, MethodAttributeNames);
            if (ats.Count == 0) break;

            var candidate = CreateCandidate(symbol, membersyntax, ats, model);
            return candidate;
        }

        // Finishing by ignoring the node...
        return null!;
    }

    /// <summary>
    /// Returns the collection of attributes that decorate the given element, either from the given
    /// collection of types, or from the given collection of names. Returns an empyt collectio if no
    /// specified argument was found.
    /// </summary>
    static List<AttributeData> FindAttributes(
        ISymbol symbol,
        MemberDeclarationSyntax syntax,
        List<Type> types,
        List<string> names)
    {
        var ats = FindAttributes(symbol, syntax);

        // By matching the found attributes against the given types...
        var list = ats.Where(x =>
            x.AttributeClass != null &&
            x.AttributeClass.MatchAny(types)).ToList();

        // By matching the found attributes against the given names...
        foreach (var name in names)
        {
            var temps = ats.Where(x => x.AttributeClass?.Name == name);
            foreach (var temp in temps) if (!list.Contains(temp)) list.Add(temp);
        }

        return list;
    }

    /// <summary>
    /// Gets the collection of all attributes that decorates the symbol, obtained through the ones
    /// found at the given syntax. For whatever reasons, <see cref="ISymbol.GetAttributes"/> does
    /// not return all attributes when the symbol appears in different places (ie: partial types).
    /// </summary>
    static IEnumerable<AttributeData> FindAttributes(
        ISymbol symbol,
        MemberDeclarationSyntax syntax)
    {
        var atsyntaxes = syntax.AttributeLists.SelectMany(static x => x.Attributes);
        foreach (var atsyntax in atsyntaxes)
        {
            var atd = symbol.GetAttributes().FirstOrDefault(
                x => x.ApplicationSyntaxReference?.GetSyntax() == atsyntax);

            if (atd is not null) yield return atd;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a new hierarchy node based upon the given candidate.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    protected virtual TypeNode CreateNode(INode parent, TypeCandidate candidate)
    {
        var item = new TypeNode(parent, candidate.Symbol);
        if (candidate.Syntax != null) item.SyntaxNodes.Add(candidate.Syntax);
        item.Attributes.AddRange(candidate.Attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a new hierarchy node based upon the given candidate.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    protected virtual PropertyNode CreateNode(TypeNode parent, PropertyCandidate candidate)
    {
        var item = new PropertyNode(parent, candidate.Symbol);
        if (candidate.Syntax != null) item.SyntaxNodes.Add(candidate.Syntax);
        item.Attributes.AddRange(candidate.Attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a new hierarchy node based upon the given candidate.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    protected virtual FieldNode CreateNode(TypeNode parent, FieldCandidate candidate)
    {
        var item = new FieldNode(parent, candidate.Symbol);
        if (candidate.Syntax != null) item.SyntaxNodes.Add(candidate.Syntax);
        item.Attributes.AddRange(candidate.Attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a new hierarchy node based upon the given candidate.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    protected virtual MethodNode CreateNode(TypeNode parent, MethodCandidate candidate)
    {
        var item = new MethodNode(parent, candidate.Symbol);
        if (candidate.Syntax != null) item.SyntaxNodes.Add(candidate.Syntax);
        item.Attributes.AddRange(candidate.Attributes);
        return item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the code of the collection of captured candidates, using the given source
    /// production context, and the compilation object. This method, by default, arranges these
    /// candidates in a file-based hierarchy and, once created, validates its nodes and emit their
    /// associated source code.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="source"></param>
    protected virtual void EmitCode(
        SourceProductionContext context, (Compilation, ImmutableArray<ICandidate>) source)
    {
        var compilation = source.Item1;
        var candidates = source.Item2;
        var comparer = SymbolEqualityComparer.Default;
        List<FileNode> files = [];

        // Error candidates...
        candidates.ForEach(
            x => x is ErrorCandidate,
            x => ((ErrorCandidate)x).Diagnostic.Report(context));

        // Creating hierarchy...
        candidates.ForEach(
            x => x is IValidCandidate,
            x => Capture((IValidCandidate)x));

        // Generating source code...
        foreach (var file in files)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (!file.Validate(context)) continue;
            var cb = new CodeBuilder();
            file.Emit(context, cb);

            var code = cb.ToString();
            var name = file.FileName + ".g.cs";
            context.AddSource(name, code);
        }

        /// <summary>
        /// Invoked to capture the given candidate in the appropriate hierarchy node.
        /// </summary>
        void Capture(IValidCandidate candidate)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var tpcandidate = candidate as TypeCandidate;
            var tpsymbol = candidate.Symbol is INamedTypeSymbol named
                ? named
                : candidate.Symbol.ContainingType;

            var options = new SymbolDisplayFormat(
                globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
                miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

            var name = tpsymbol.ToDisplayString(options);
            name = ToReverseFileName(name);

            // Capturing the appropriate file...
            var file = files.Find(x => name == x.FileName);
            if (file == null)
            {
                // Capturing a new file...
                file = new(compilation, name);
                files.Add(file);

                // Setting its node, either auto-generated or not...
                var node = tpcandidate is null
                    ? new TypeNode(file, tpsymbol) { AutoGenerated = true }
                    : CreateNode(file, tpcandidate);

                file.Node = node;
            }
            else
            {
                // If existing was auto-generated...
                if (file.Node is null || file.Node.AutoGenerated)
                {
                    if (tpcandidate is not null)
                    {
                        var node = CreateNode(file, tpcandidate);
                        if (file.Node is not null) node.Augment(file.Node);

                        files.Remove(file);
                        files.Add(file = new(compilation, name));
                        file.Node = node;
                    }
                }

                // Existing already was a valid one...
                else
                {
                    if (tpcandidate is not null) file.Node.Augment(tpcandidate);
                }
            }

            // Capturing the appropriate child...
            if (candidate is PropertyCandidate pcandidate) CaptureProperty(file, pcandidate);
            if (candidate is FieldCandidate fcandidate) CaptureField(file, fcandidate);
            if (candidate is MethodCandidate mcandidate) CaptureMethod(file, mcandidate);
        }

        /// <summary>
        /// Invoked to capture the given candidate in the appropriate hierarchy node.
        /// </summary>
        void CaptureProperty(FileNode file, PropertyCandidate candidate)
        {
            var node = file.Node!.ChildProperties.Find(
                x => comparer.Equals(x.Symbol, candidate.Symbol));

            if (node is null)
            {
                node = CreateNode(file.Node, candidate);
                file.Node.ChildProperties.Add(node);
            }
            else node.Augment(candidate);
        }

        /// <summary>
        /// Invoked to capture the given candidate in the appropriate hierarchy node.
        /// </summary>
        void CaptureField(FileNode file, FieldCandidate candidate)
        {
            var node = file.Node!.ChildFields.Find(
                x => comparer.Equals(x.Symbol, candidate.Symbol));

            if (node is null)
            {
                node = CreateNode(file.Node, candidate);
                file.Node.ChildFields.Add(node);
            }
            else node.Augment(candidate);
        }

        /// <summary>
        /// Invoked to capture the given candidate in the appropriate hierarchy node.
        /// </summary>
        void CaptureMethod(FileNode file, MethodCandidate candidate)
        {
            var node = file.Node!.ChildMethods.Find(
                x => comparer.Equals(x.Symbol, candidate.Symbol));

            if (node is null)
            {
                node = CreateNode(file.Node, candidate);
                file.Node.ChildMethods.Add(node);
            }
            else node.Augment(candidate);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Takes the given type's display string, and reverses its first-level dot separated parts
    /// to return a suitable file name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual string ToReverseFileName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

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
}