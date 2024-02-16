namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a type in the source code generation hierarchy.
/// </summary>
/// <param name="model"></param>
/// <param name="syntax"></param>
/// <param name="symbol"></param>
internal class TypeNode(
    SemanticModel model, TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
    : Candidate(model, syntax, symbol), INode, ITypeCandidate
{
    /// <inheritdoc/>
    public override string ToString() => $"Type: {Symbol.Name}";

    /// <inheritdoc/>
    public new TypeDeclarationSyntax Syntax { get; } = syntax.ThrowWhenNull();

    /// <inheritdoc/>
    public new INamedTypeSymbol Symbol { get; } = symbol.ThrowWhenNull();

    /// <summary>
    /// The list of child types.
    /// <br/> Default equality: symbol comparison.
    /// </summary>
    public List<TypeNode> ChildTypes { get; } = [];

    /// <summary>
    /// The list of child properties.
    /// <br/> Default equality: symbol comparison.
    /// </summary>
    public List<PropertyNode> ChildProperties { get; } = [];

    /// <summary>
    /// The list of child fields.
    /// <br/> Default equality: symbol comparison.
    /// </summary>
    public List<FieldNode> ChildFields { get; } = [];

    /// <summary>
    /// The list of child methods.
    /// <br/> Default equality: symbol comparison.
    /// </summary>
    public List<MethodNode> ChildMethods { get; } = [];

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Validate(SourceProductionContext context) => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual void Emit(SourceProductionContext context, CodeBuilder cb) => throw null;
}