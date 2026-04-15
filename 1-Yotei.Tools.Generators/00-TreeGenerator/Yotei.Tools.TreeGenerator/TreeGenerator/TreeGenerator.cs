namespace Yotei.Tools.Generators;

/* To DEBUG:
 * - Install the .NET Compiler SDK (in addition to Roslyn components).
 * - Make sure the derived generator project is a Roslyn component:
 *   <IsRoslynComponent>true</IsRoslynComponent>
 * - In the derived generator project's properties, add a Roslyn debug profile, whose target is the
 *   project that when compiled will be debugged (ie: a test project).
 * - Mark the derived generator project as the startup one.
 * - In the play button, select the debug profile.
 * - Click F5 (run) to compile (F6 does nothing).
 */

/* DESIGN NOTES:
 * - Even if it is architecturally prepared to do so, for simplicity reasons this version does not
 *   follow the recommended rule of only caching the string elements to emit later. For simplicity
 *   reasons, it takes a ISymbol-oriented approach. It it is supposed that this invalidates the
 *   generator's cache mechanism, but I have not (yet) hit any performance problems.
 */

// ========================================================
/// <summary>
/// Represents a tree-oriented incremental source generator that, when capturing its relevant nodes
/// organizes them by the types they refer to (which either are the types by themselves, or the
/// declaring ones for its childs). Each node is then emitted in its own file.
/// <para>
/// Derived types need to be decorated with the <see cref="GeneratorAttribute"/> attribute to be
/// recognized by the compiler. In is also expected that <see cref="LanguageNames.CSharp"/> is used
/// as its argument.
/// </para>
/// </summary>
public partial class TreeGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Determines if the files emitted by this source generator are all them placed in the same
    /// root folder, rather than in the folder hierarchy each specifies (which is usefull, for
    /// instance, for debug purposes).
    /// </summary>
    protected virtual bool EmitFilesInFolders => false;

    /// <summary>
    /// Determines if the <see cref="IsNullable{T}"/> and the <see cref="IsNullableAttribute"/>
    /// types are emitted in the namespace of the derived generator.
    /// </summary>
    protected virtual bool EmitNullabilityHelpers => true;

    /// <summary>
    /// Invoked to register post-initialization actions such as reading external files, generating
    /// code for marker attributes, and so on. Marker attribute classes can be decorated with the
    /// '[Microsoft.CodeAnalysis.Embedded]' attribute, as it is automatically added to the emitted
    /// source code.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialize(IncrementalGeneratorPostInitializationContext context) { }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of attribute types used to identify decorated type-alike elements.
    /// </summary>
    protected virtual List<Type> TypeAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types used to identify decorated property-alike elements.
    /// </summary>
    protected virtual List<Type> PropertyAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types used to identify decorated field-alike elements.
    /// </summary>
    protected virtual List<Type> FieldAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types used to identify decorated method-alike elements.
    /// </summary>
    protected virtual List<Type> MethodAttributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// The collection of fully qualified attribute type names used to identify decorated type
    /// -alike elements.
    /// </summary>
    protected virtual List<string> TypeAttributeNames { get; } = [];

    /// <summary>
    /// The collection of fully qualified attribute type names used to identify decorated property
    /// -alike elements.
    /// </summary>
    protected virtual List<string> PropertyAttributeNames { get; } = [];

    /// <summary>
    /// The collection of fully qualified attribute type names used to identify decorated field
    /// -alike elements.
    /// </summary>
    protected virtual List<string> FieldAttributeNames { get; } = [];

    /// <summary>
    /// The collection of fully qualified attribute type names used to identify decorated method
    /// -alike elements.
    /// </summary>
    protected virtual List<string> MethodAttributeNames { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to quickly determine if the given syntax node shall be considered as a potential
    /// candidate for source code generation, or not. By default, this base method validates that
    /// the node is among the recognized ones, and that the generator has specified attributes
    /// for its kind.
    /// <br/> Inheritors may override this method to, for instance, support other syntax node
    /// kinds. The ones supported by default are: <see cref="BaseTypeDeclarationSyntax"/>, 
    /// <see cref="BasePropertyDeclarationSyntax"/>, <see cref="BaseFieldDeclarationSyntax"/>,
    /// and <see cref="BaseMethodDeclarationSyntax"/>. Event-alike ones are covered by either
    /// the property or field ones
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0078")]
    protected virtual bool FilterNode(SyntaxNode node, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        return node switch
        {
            BaseTypeDeclarationSyntax => TypeAttributes.Count > 0 || TypeAttributeNames.Count > 0,
            BasePropertyDeclarationSyntax => PropertyAttributes.Count > 0 || PropertyAttributes.Count > 0,
            BaseFieldDeclarationSyntax => FieldAttributes.Count > 0 || FieldAttributes.Count > 0,
            BaseMethodDeclarationSyntax => MethodAttributes.Count > 0 || MethodAttributes.Count > 0,

            _ => false
        };
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture the relevant information into a new detached source generation node.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual TypeNode CreateNode(
        INamedTypeSymbol symbol,
        BaseTypeDeclarationSyntax syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new TypeNode(symbol);
        item.SyntaxNodes.Add(syntax);
        item.Attributes.AddRange(attributes);
        return item;
    }

    /// <summary>
    /// Invoked to capture the relevant information into a new detached source generation node.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual PropertyNode CreateNode(
        IPropertySymbol symbol,
        BasePropertyDeclarationSyntax syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new PropertyNode(symbol);
        item.SyntaxNodes.Add(syntax);
        item.Attributes.AddRange(attributes);
        return item;
    }

    /// <summary>
    /// Invoked to capture the relevant information into a new detached source generation node.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual FieldNode CreateNode(
        IFieldSymbol symbol,
        BaseFieldDeclarationSyntax syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new FieldNode(symbol);
        item.SyntaxNodes.Add(syntax);
        item.Attributes.AddRange(attributes);
        return item;
    }

    /// <summary>
    /// Invoked to capture the relevant information into a new detached source generation node.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual MethodNode CreateNode(
        IMethodSymbol symbol,
        BaseMethodDeclarationSyntax syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new MethodNode(symbol);
        item.SyntaxNodes.Add(syntax);
        item.Attributes.AddRange(attributes);
        return item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture and return the given syntax node as a souce code generation candidate,
    /// an error one, or a <see langword="null"/> one if the syntax node is to be ignored.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0019")]
    protected virtual INode CaptureNode(GeneratorSyntaxContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var node = context.Node;
        var model = context.SemanticModel;

        // Prevents previously generated code to be processed twice...
        var path = node.SyntaxTree.FilePath;
        if (path.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase) ||
            path.EndsWith(".generated.cs", StringComparison.OrdinalIgnoreCase))
            return null!;

        // Type-alike nodes...
        while (node is BaseTypeDeclarationSyntax syntax && (
            syntax is TypeDeclarationSyntax ||
            syntax is EnumDeclarationSyntax))
        {
            var symbol = model.GetDeclaredSymbol(syntax, token);
            if (symbol == null) break;

            var atx = FindSyntaxAttributes(symbol, syntax);
            var ats = FilterAttributes(atx, TypeAttributes, TypeAttributeNames);
            if (ats.Count == 0) break;

            var temp = CreateNode(symbol, syntax, ats, model);
            return temp;
        }

        // Property-alike nodes...
        while (node is BasePropertyDeclarationSyntax syntax && (
            syntax is PropertyDeclarationSyntax ||
            syntax is IndexerDeclarationSyntax ||
            syntax is EventDeclarationSyntax))
        {
            var symbol = model.GetDeclaredSymbol(syntax, token) as IPropertySymbol;
            if (symbol == null) break;

            var atx = FindSyntaxAttributes(symbol, syntax);
            var ats = FilterAttributes(atx, PropertyAttributes, PropertyAttributeNames);
            if (ats.Count == 0) break;

            var temp = CreateNode(symbol, syntax, ats, model);
            return temp;
        }

        // Field-alike nodes...
        while (node is BaseFieldDeclarationSyntax syntax && (
            syntax is FieldDeclarationSyntax ||
            syntax is EventFieldDeclarationSyntax))
        {
            var items = syntax.Declaration.Variables;
            foreach (var item in items)
            {
                var symbol = model.GetDeclaredSymbol(item, token) as IFieldSymbol;
                if (symbol == null) continue;

                var atx = FindSyntaxAttributes(symbol, syntax);
                var ats = FilterAttributes(atx, FieldAttributes, FieldAttributeNames);
                if (ats.Count == 0) break;

                var temp = CreateNode(symbol, syntax, ats, model);
                return temp;
            }
            break;
        }

        // Method-alike nodes...
        while (node is BaseMethodDeclarationSyntax syntax && (
            syntax is MethodDeclarationSyntax ||
            syntax is ConstructorDeclarationSyntax ||
            syntax is DestructorDeclarationSyntax ||
            syntax is OperatorDeclarationSyntax ||
            syntax is ConversionOperatorDeclarationSyntax))
        {
            var symbol = model.GetDeclaredSymbol(syntax, token) as IMethodSymbol;
            if (symbol == null) break;

            var atx = FindSyntaxAttributes(symbol, syntax);
            var ats = FilterAttributes(atx, MethodAttributes, MethodAttributeNames);
            if (ats.Count == 0) break;

            var temp = CreateNode(symbol, syntax, ats, model);
            return temp;
        }

        // Finishing ignoring the syntax node...
        return null!;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to process the captured source code generation candidates.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="nodes"></param>
    protected virtual void EmitNodes(
        SourceProductionContext context, ImmutableArray<INode> nodes)
    {
        List<TypeNode> files = [];

        // Reporting errors and creating the files-based hierarchy...
        foreach (var node in nodes)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (node is ErrorNode error)
            {
                foreach (var diag in error.Diagnostics) diag.Report(context);
            }
            else if (node is not null)
            {
                CaptureHierarchy(context, files, node);
            }
        }

        // Emitting source code...
        foreach (var type in files)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var cb = new CodeBuilder();
            var done = type.Emit(context, cb);
            if (done)
            {
                /* TODO: Emit source code of top-most files...
                var code = cb.ToString();
                var name = GetFileName(type.Symbol) + ".g.cs";
                context.AddSource(name, code);
                 */
            }
        }
    }

    /// <summary>
    /// Invoked to capture the given node into the given hierarchy.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="files"></param>
    /// <param name="node"></param>
    protected virtual void CaptureHierarchy(
        SourceProductionContext context, List<TypeNode> files, INode node)
    {
    }
}