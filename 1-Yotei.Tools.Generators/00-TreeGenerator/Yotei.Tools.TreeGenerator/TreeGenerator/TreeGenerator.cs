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
 * To follow the recommended approach of only capturing in the transformed nodes generator cache
 * friendly elements, the nodes themselves must implement their 'IEquatable<INode>' capabilities
 * in such way. By default they take a symbol-oriented approach that define how their identity
 * shall be compared. If equality by the captured syntax nodes or attributes is also needed,
 * then they must override the related methods.
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
    /// Invoked to create a derived <see cref="TreeOptions"/> instance to hold the options used
    /// by this generator, that will either have their default values or the ones read from the
    /// consuming project's csproj file.
    /// </summary>
    /// <returns></returns>
    protected virtual TreeGeneratorOptions CreateTreeOptions() => new();

    /// <summary>
    /// Invoked to register post-initialization actions, such as generating code for marker
    /// attributes, reading external files, and so forth.
    /// <br/> By default, this method adds the the 'Microsoft.CodeAnalysis.Embedded' attribute.
    /// <br/> Inheritors will typically invoke their base methods first.
    /// <para></para>
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddEmbeddedAttributeDefinition();
    }

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
    /// source code generation candidate, or not. By default, this method validates that the node
    /// kind is among the recognized ones, and that the list of attribute classes or names for
    /// that kind is not an empty one.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual bool FastPredicate(SyntaxNode node, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        return node switch
        {
            BaseTypeDeclarationSyntax => TypeAttributes.Count > 0 || TypeAttributeNames.Count > 0,
            BasePropertyDeclarationSyntax => PropertyAttributes.Count > 0 || PropertyAttributeNames.Count > 0,
            BaseFieldDeclarationSyntax => FieldAttributes.Count > 0 || FieldAttributeNames.Count > 0,
            BaseMethodDeclarationSyntax => MethodAttributes.Count > 0 || MethodAttributeNames.Count > 0,
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
    /// Invoked to either capture and return a valid source code generation node, or a null one
    /// if the given element shall be ignored.
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

            if (symbol.Name == "MyType") { } // DEBUG-ONLY

            var atx = syntax.AttributeLists.SelectMany(x => x.Attributes).FirstOrDefault();
            if (atx is null) break;
            var type = model.GetTypeInfo(atx).Type;
            var name = type.EasyName(EasyTypeOptions.Full);
            
            
            //var atx = FindSyntaxAttributes(symbol, syntax);
            //var ats = FilterAttributes(atx, TypeAttributes, TypeAttributeNames);
            //if (ats.Count == 0) break;

            //var temp = CreateNode(symbol, syntax, ats, model);
            //return temp;

            return null;
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
    /// Invoked to emit common files (as, for instance, marker attributes), when the options read
    /// from the consuming project csproj file are needed.
    /// <br/> Inheritors will typically invoke their base methods first.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnEmitContext(ref TreeContext context)
    {
        var usefolders = context.Options.UseFileFolders;
        var reversenames = context.Options.ReverseFileNames;

        if (context.Options.EmitNullabilityHelpers)
        {
            var code = GetNullabilityHelpers();
            var name = "Yotei.Tools.NullabilityHelpers.cs";
            name = NormalizeFileName(name, usefolders, reversenames);
            context.Context.AddSource(name, code);
        }
    }

    /// <summary>
    /// Obtains the code for the nullability helpers, to be emitted under the namespace of this
    /// generator.
    /// </summary>
    string GetNullabilityHelpers() => $$"""
        namespace {{GetType().Namespace!}}
        {
            /// <summary>
            /// Used to wrap types for which nullability information shall be persisted.
            /// <para>
            /// Nullable annotations on value types are always translated by the compiler into instances
            /// of the <see cref="Nullable{T}"/> struct. By contrast, nullable annotations on reference
            /// types are just syntactic sugar used by the compiler but, in general, either they are not
            /// persisted in metadata or in custom attributes, or they are not allowed in certain contexts
            /// (e.g., generic type arguments).
            /// <br/> The <see cref="IsNullable{T}"/> and <see cref="IsNullableAttribute"/> types can be
            /// used as workarounds when there is the need to persist nullability information for their
            /// associated types, or when there is the need to specify it in those not-allowed contexts.
            /// </para>
            /// </summary>
            [Microsoft.CodeAnalysis.Embedded]
            public class IsNullable<T> { }
            
            /// <summary>
            /// <inheritdoc cref="IsNullable{T}"/>
            /// </summary>
            [Microsoft.CodeAnalysis.Embedded]
            [AttributeUsage(AttributeTargets.All)]
            public class IsNullableAttribute : Attribute { }
        }
        """;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to either generate the source code of the captured nodes, or to report the error
    /// conditions that may have been captured.
    /// <br/> Inheritors' note: the second argument of this method is a tuple that carryies as its
    /// left value the captured collection of nodes, and as its right one the options read from the
    /// consuming project.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="source"></param>
    protected virtual void EmitNodes(
        SourceProductionContext context, (ImmutableArray<INode>, TreeGeneratorOptions) source)
    {
        var nodes = source.Item1;
        var options = source.Item2;
        var extended = new TreeContext(context, options);
        var files = new List<TypeNode>();

        // First, emit common code...
        OnEmitContext(ref extended);

        // Processing nodes...
        foreach (var node in nodes)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (node is ErrorNode error) error.Report(context);
            else CaptureHierarchy(files, node, context);
        }

        // Generating source code...
        foreach (var type in files)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var cb = new CodeBuilder();
            var ok = type.Emit(ref extended, cb); if (ok)
            {
                var code = cb.ToString();
                var name = type.GetFileName();
                name = NormalizeFileName(name, options.UseFileFolders, options.ReverseFileNames);
                context.AddSource(name, code);
            }
        }
    }

    /// <summary>
    /// Invoked to capture the given source code generation node into the hierarchy given by the
    /// given collection of file-level types.
    /// </summary>
    /// <param name="files"></param>
    /// <param name="node"></param>
    /// <param name="context"></param>
    protected virtual void CaptureHierarchy(
        List<TypeNode> files, INode node, SourceProductionContext context)
    {
    }
}