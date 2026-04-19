using System.ComponentModel;
using System.Net.Http.Headers;

namespace Yotei.Tools.Generators;

/* To DEBUG:
 * - Install the .NET Compiler SDK (in addition to Roslyn components).
 * - Make sure the derived generator project is a Roslyn component:
 *      <IsRoslynComponent>true</IsRoslynComponent>
 * - In the derived generator project's properties, add a Roslyn debug profile, whose target being
 *   the project that, when compiled, will be debugged (ie: a test project). This, in turn, debugs
 *   the generator project as well as a kind-of side-effect.
 * - Mark the derived generator project as the startup one (not the test one!).
 * - In the play button, select the debug profile.
 * - Click F5 (run) to compile (F6 does nothing).
 */

// ========================================================
/// <summary>
/// Represents a tree-oriented incremental source generator that organizes the captured nodes in
/// a hierarchycal tree where each top-most element will be emitted in its own file and correspond
/// to a captured type along with its child elements, if any.
/// <para>
/// Derived types need to be decorated with the <see cref="GeneratorAttribute"/> attribute to be
/// recognized by the compiler. In is also expected that <see cref="LanguageNames.CSharp"/> is used
/// as its argument.
/// </para>
/// </summary>
public partial class TreeGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Invoked to register post-initialization actions, such as reading external source files, or
    /// generating code for marker attributes, among others. By default, this base method adds the
    /// <see langword="Microsoft.CodeAnalysis.Embedded"/> attribute to the compilation.
    /// <br/> Inheritors will typically invoke their base method first.
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
    /// is among the recognized ones, and that the list of attribute types or attribute names for
    /// its kind is any not an empty one.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual bool TreePredicate(SyntaxNode node, CancellationToken token)
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
    /// Tries to transform the potential syntax node candidate carried by the given context into
    /// a source code tree-oriented generator one. This method may also return error nodes that
    /// carry diagnostics to be reported, or <see langword="null"/> if the syntax node is to be
    /// completely ignored.
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

        // Type-alike nodes.
        while (node is BaseTypeDeclarationSyntax syntax && (
            syntax is TypeDeclarationSyntax ||
            syntax is EnumDeclarationSyntax))
        {
            var symbol = model.GetDeclaredSymbol(syntax, token);
            if (symbol == null) return null!;

            var atx = FindSyntaxAttributes(symbol, syntax);
            var ats = FilterAttributes(atx, TypeAttributes, TypeAttributeNames);
            if (ats.Count == 0) return null!;

            var temp = CreateNode(symbol, syntax, ats, model);
            return temp;
        }

        // Property-alike nodes.
        while (node is BasePropertyDeclarationSyntax syntax && (
            syntax is PropertyDeclarationSyntax ||
            syntax is IndexerDeclarationSyntax ||
            syntax is EventDeclarationSyntax))
        {
            var symbol = model.GetDeclaredSymbol(syntax, token) as IPropertySymbol;
            if (symbol == null) return null!;

            var atx = FindSyntaxAttributes(symbol, syntax);
            var ats = FilterAttributes(atx, PropertyAttributes, PropertyAttributeNames);
            if (ats.Count == 0) return null!;

            var temp = CreateNode(symbol, syntax, ats, model);
            return temp;
        }

        // Property-alike nodes.
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
                if (ats.Count == 0) continue;

                var temp = CreateNode(symbol, syntax, ats, model);
                return temp;
            }
            return null!;
        }

        // Method-alike nodes.
        while (node is BaseMethodDeclarationSyntax syntax && (
            syntax is MethodDeclarationSyntax ||
            syntax is ConstructorDeclarationSyntax ||
            syntax is DestructorDeclarationSyntax ||
            syntax is OperatorDeclarationSyntax ||
            syntax is ConversionOperatorDeclarationSyntax))
        {
            var symbol = model.GetDeclaredSymbol(syntax, token);
            if (symbol == null) return null!;

            var atx = FindSyntaxAttributes(symbol, syntax);
            var ats = FilterAttributes(atx, PropertyAttributes, PropertyAttributeNames);
            if (ats.Count == 0) return null!;

            var temp = CreateNode(symbol, syntax, ats, model);
            return temp;
        }

        // Finishing...
        return null!;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to both generate the source code of the captured nodes, and to report the error
    /// conditions the error nodes may carry with them.
    /// </summary>
    /// <param name="gencontext"></param>
    /// <param name="source"></param>
    protected void EmitNodes(
        SourceProductionContext gencontext,
        (ImmutableArray<INode>, ImmutableArray<TreeOptions>) source)
    {
        var nodes = source.Item1;
        var options = source.Item2.Length > 0 ? source.Item2[0] : new TreeOptions();

        // TODO: EmitNodes
        return;
    }
}