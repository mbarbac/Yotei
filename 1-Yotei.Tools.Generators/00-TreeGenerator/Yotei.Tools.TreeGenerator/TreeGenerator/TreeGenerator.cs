namespace Yotei.Tools.TreeGenerator;

// ========================================================
/// <summary>
/// Represents the base class for tree-oriented incremental source code generators that arrange
/// their captured elements in a hierarchical tree structure where each top node corresponds to
/// a single type, along with its child elements (if any), and its emitted in its own file.
/// <para>
/// Derived types shall be be decorated with the <see cref="GeneratorAttribute"/> attribute to be
/// recognized by the compiler. It is also expected that the <see cref="LanguageNames.CSharp"/>
/// value is used as that attribute's argument.
/// </para>
/// </summary>
internal partial class TreeGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Determines if this instance tries to launch a compile-time debug session when compiling.
    /// <br/> The default value of this property is <see langword="false"/>.
    /// </summary>
    protected virtual bool LaunchDebugger => false;

    /// <summary>
    /// Determines if the <see cref="IsNullable{T}"/> and <see cref="IsNullableAttribute"/> types
    /// shall be emitted in the namespace of the derived generator. The default value of this
    /// property is <see langword="true"/>.
    /// </summary>
    protected virtual bool EmitNullabilityHelpers => true;

    /// <summary>
    /// Determines if the source code generation files will all be emitted in the same folder, or
    /// rather in a hierarchy of folder using all the dot-separated parts of their names (except
    /// the last one) as the folders' names.
    /// </summary>
    protected virtual bool FlatFileNames => false;

    /// <summary>
    /// Invoked to register post-initialization actions such as reading external files, generating
    /// code for marker attributes, and so on. By default this method does nothing.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialize(IncrementalGeneratorPostInitializationContext context) { }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to quickly determine if the given syntax node shall be consider as a potential
    /// candidate for source code generation or not.
    /// <br/> Note that, later, the node will be validated further using the semantic model and
    /// the node's contents and, eventually, a hierarchy element created for it.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual bool FastPredicate(SyntaxNode node, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        return node is
            BaseTypeDeclarationSyntax or
            BasePropertyDeclarationSyntax or
            BaseFieldDeclarationSyntax or
            BaseMethodDeclarationSyntax or
            EventDeclarationSyntax or
            EventFieldDeclarationSyntax;
    }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of attribute types used by default by this generator to identify decorated
    /// types (enums and regular ones) as source code generation elements. If their types are used
    /// then there is no need to specify their fully qualified names.
    /// </summary>
    protected virtual List<Type> TypeAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types used by default by this generator to identify decorated
    /// properties (indexed and regular ones) as source code generation elements. If their types
    /// are used then there is no need to specify their fully qualified names.
    /// </summary>
    protected virtual List<Type> PropertyAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types used by default by this generator to identify decorated
    /// fields as source code generation elements. If their types are used then there is no need
    /// to specify their fully qualified names.
    /// </summary>
    protected virtual List<Type> FieldAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types used by default by this generator to identify decorated
    /// methods (constructors, destructors, conversions, operators and regular ones) as source code
    /// generation elements. If their types are used then there is no need to specify their fully
    /// qualified names.
    /// </summary>
    protected virtual List<Type> MethodAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types used by default by this generator to identify decorated
    /// events (field and regular ones) as source code generation elements. If their types are used
    /// then there is no need to specify their fully qualified names.
    /// </summary>
    protected virtual List<Type> EventAttributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// The collection of fully qualified attribute type names used by default by this generator
    /// to identify decorated types as source code generation candidates. This property is only
    /// used when their types can only be specified by using these names.
    /// </summary>
    protected virtual List<string> TypeAttributeNames { get; } = [];

    /// <summary>
    /// The collection of fully qualified attribute type names used by default by this generator
    /// to identify decorated properties as source code generation candidates. This property is
    /// only used when their types can only be specified by using these names.
    /// </summary>
    protected virtual List<string> PropertyAttributeNames { get; } = [];

    /// <summary>
    /// The collection of fully qualified attribute type names used by default by this generator
    /// to identify decorated fields as source code generation candidates. This property is only
    /// used when their types can only be specified by using these names.
    /// </summary>
    protected virtual List<string> FieldAttributeNames { get; } = [];

    /// <summary>
    /// The collection of fully qualified attribute type names used by default by this generator
    /// to identify decorated methods as source code generation candidates. This property is only
    /// used when their types can only be specified by using these names.
    /// </summary>
    protected virtual List<string> MethodAttributeNames { get; } = [];

    /// <summary>
    /// The collection of fully qualified attribute type names used by default by this generator
    /// to identify decorated events as source code generation candidates. This property is only
    /// used when their types can only be specified by using these names.
    /// </summary>
    protected virtual List<string> EventAttributeNames { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a detached node (not yet captured in the hierarchy) for code generation
    /// purposes. This method won't be invoked if the syntax kind is not among the supported ones.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual TypeNode CreateNode(
        INamedTypeSymbol symbol,
        BaseTypeDeclarationSyntax? syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new TypeNode(symbol);
        if (syntax != null) item.SyntaxNodes.Add(syntax);
        item.Attributes.AddRange(attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a detached node (not yet captured in the hierarchy) for code generation
    /// purposes. This method won't be invoked if the syntax kind is not among the supported ones.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual PropertyNode CreateNode(
        IPropertySymbol symbol,
        BasePropertyDeclarationSyntax? syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new PropertyNode(symbol);
        if (syntax != null) item.SyntaxNodes.Add(syntax);
        item.Attributes.AddRange(attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a detached node (not yet captured in the hierarchy) for code generation
    /// purposes. This method won't be invoked if the syntax kind is not among the supported ones.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual FieldNode CreateNode(
        IFieldSymbol symbol,
        BaseFieldDeclarationSyntax? syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new FieldNode(symbol);
        if (syntax != null) item.SyntaxNodes.Add(syntax);
        item.Attributes.AddRange(attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a detached node (not yet captured in the hierarchy) for code generation
    /// purposes. This method won't be invoked if the syntax kind is not among the supported ones.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual MethodNode CreateNode(
        IMethodSymbol symbol,
        BaseMethodDeclarationSyntax? syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new MethodNode(symbol);
        if (syntax != null) item.SyntaxNodes.Add(syntax);
        item.Attributes.AddRange(attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a detached node (not yet captured in the hierarchy) for code generation
    /// purposes. This method won't be invoked if the syntax kind is not among the supported ones.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual EventNode CreateNode(
        IEventSymbol symbol,
        MemberDeclarationSyntax? syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new EventNode(symbol);
        if (syntax != null) item.SyntaxNodes.Add(syntax);
        item.Attributes.AddRange(attributes);
        return item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a detached hierarchy element for source code generation purposes. This
    /// method can also return <see langword="null"/> to completely ignore the syntax node, or an
    /// error candidate instance to report an error conditions at code generation time.
    /// <para>
    /// This method works by using the context's semantic model to create detached nodes of the
    /// appropriate types, using the 'CreateNode' virtual methods. Inheritors may override this
    /// method completely, or invoke their base ones, as they need.
    /// </para>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual INode CreateNode(GeneratorSyntaxContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var node = context.Node;
        var model = context.SemanticModel;

        // Types...
        while (node is BaseTypeDeclarationSyntax syntax)
        {
            if (syntax is not EnumDeclarationSyntax and not TypeDeclarationSyntax)
                break;

            var symbol = model.GetDeclaredSymbol(syntax, token);
            if (symbol is null) break;

            var atx = FindSyntaxAttributes(symbol, syntax);
            var ats = FilterAttributes(atx, TypeAttributes, TypeAttributeNames);
            if (ats.Count == 0) break;

            var candidate = CreateNode(symbol, syntax, ats, model);
            return candidate;
        }

        // Properties...
        while (node is BasePropertyDeclarationSyntax syntax)
        {
            if (syntax is not IndexerDeclarationSyntax and not PropertyDeclarationSyntax)
                break;

            var symbol = model.GetDeclaredSymbol(syntax, token) as IPropertySymbol;
            if (symbol is null) break;

            var atx = FindSyntaxAttributes(symbol, syntax).ToDebugArray();
            var ats = FilterAttributes(atx, PropertyAttributes, PropertyAttributeNames);
            if (ats.Count == 0) break;

            var candidate = CreateNode(symbol, syntax, ats, model);
            return candidate;
        }

        // Fields...
        while (node is FieldDeclarationSyntax syntax)
        {
            var items = syntax.Declaration.Variables;
            foreach (var item in items)
            {
                var symbol = model.GetDeclaredSymbol(item, token) as IFieldSymbol; // item!
                if (symbol is null) break;

                var atx = FindSyntaxAttributes(symbol, syntax);
                var ats = FilterAttributes(atx, FieldAttributes, FieldAttributeNames);
                if (ats.Count == 0) break;

                var candidate = CreateNode(symbol, syntax, ats, model);
                return candidate;
            }
            break;
        }

        // Methods...
        while (node is BaseMethodDeclarationSyntax syntax)
        {
            if (syntax is not MethodDeclarationSyntax
                and not ConstructorDeclarationSyntax and not DestructorDeclarationSyntax
                and not OperatorDeclarationSyntax and not ConversionOperatorDeclarationSyntax)
                break;

            var symbol = model.GetDeclaredSymbol(syntax, token);
            if (symbol is null) break;

            var atx = FindSyntaxAttributes(symbol, syntax);
            var ats = FilterAttributes(atx, MethodAttributes, MethodAttributeNames);
            if (ats.Count == 0) break;

            var candidate = CreateNode(symbol, syntax, ats, model);
            return candidate;
        }

        // Property-alike events...
        while (node is EventDeclarationSyntax syntax)
        {
            var symbol = model.GetDeclaredSymbol(syntax, token);
            if (symbol is null) break;

            var atx = FindSyntaxAttributes(symbol, syntax);
            var ats = FilterAttributes(atx, EventAttributes, EventAttributeNames);
            if (ats.Count == 0) break;

            var candidate = CreateNode(symbol, syntax, ats, model);
            return candidate;
        }

        // Field-alike events...
        while (node is EventFieldDeclarationSyntax syntax)
        {
            var items = syntax.Declaration.Variables;
            foreach (var item in items)
            {
                var symbol = model.GetDeclaredSymbol(item, token) as IEventSymbol;
                if (symbol is null) break;

                var atx = FindSyntaxAttributes(symbol, syntax);
                var ats = FilterAttributes(atx, EventAttributes, EventAttributeNames);
                if (ats.Count == 0) break;

                var candidate = CreateNode(symbol, syntax, ats, model);
                return candidate;
            }
            break;
        }

        // Finishing by ignoring the node...
        return null!;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture the given node into the hierarchy of source code generation elements.
    /// This hierarchy is represented by the given list of top-most type-alike nodes where the
    /// given one will find out which one it will be added to, or the existing child node that
    /// will be augmented with the given node's information.
    /// <para>
    /// All the well-known node types (for Types, Properties, Fields, Methods and Events) are
    /// already treated by this base method. Inheritors may override it to treat their specific
    /// node types, and then call their base methods to treat all the standard ones.
    /// </para>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="hierarchy"></param>
    /// <param name="node"></param>
    protected virtual void CaptureNode(
        SourceProductionContext context, List<TypeNode> hierarchy, INode node)
    {
        switch (node)
        {
            case TypeNode item: OnCaptureHierarchy(hierarchy, item); break;
            case PropertyNode item: OnCaptureHierarchy(hierarchy, item); break;
            case FieldNode item: OnCaptureHierarchy(hierarchy, item); break;
            case MethodNode item: OnCaptureHierarchy(hierarchy, item); break;
            case EventNode item: OnCaptureHierarchy(hierarchy, item); break;
        }
    }
}