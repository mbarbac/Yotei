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
    /// Determines if the '[Microsoft.CodeAnalysis.Embedded]' attribute is added to the generated
    /// source code. This attribute is typically used to decorate marker attributes to prevent
    /// duplicates.
    /// </summary>
    public virtual bool EmitEmbeddedAttribute => true;

    /// <summary>
    /// Determines if the <see cref="IsNullable{T}"/> and the <see cref="IsNullableAttribute"/>
    /// nullability helpers are added to the generated code.
    /// </summary>
    public virtual bool EmitNullabilityHelpers => true;

    /// <summary>
    /// Invoked to register post-initialization actions, such as generating code for marker
    /// attributes, reading external files, and so forth.
    /// <para></para>
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
    protected virtual INode CaptureNode(GeneratorSyntaxContext context, CancellationToken token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to either generate the source code of the captured nodes, or to report the error
    /// conditions that may have been captured.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="nodes"></param>
    protected virtual void EmitNodes(
        SourceProductionContext context, ImmutableArray<INode> nodes)
    {
        throw null;
    }
}