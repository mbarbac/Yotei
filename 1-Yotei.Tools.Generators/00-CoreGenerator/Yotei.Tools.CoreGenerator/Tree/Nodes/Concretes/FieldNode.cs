namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a field-alike source code generation element.
/// </summary>
internal class FieldNode : ITreeNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public FieldNode(IFieldSymbol symbol) => Symbol = symbol;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Field: {Symbol.Name}";

    // ----------------------------------------------------

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    public IFieldSymbol Symbol { get; }

    /// <summary>
    /// By default, the collection of syntax nodes where the associated symbol was found, or an
    /// empty one. Its members are instances of the <see cref="FieldDeclarationSyntax"/> type.
    /// </summary>
    public List<FieldDeclarationSyntax> SyntaxNodes { get; } = [];

    /// <summary>
    /// By default, the collection of attributes by which the associated symbol was found, or an
    /// empty one.
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Augments the contents of this instance with the ones obtained from the given one. The
    /// default implementation just adds the syntax nodes and attributes to their respective
    /// collections.
    /// </summary>
    /// <param name="node"></param>
    public virtual void Augment(FieldNode node)
    {
        foreach (var syntax in node.SyntaxNodes)
            if (SyntaxNodes.Find(x => x.IsEquivalentTo(syntax)) == null)
                SyntaxNodes.Add(syntax);

        foreach (var at in node.Attributes)
            if (Attributes.Find(x => x.EqualsTo(at)) == null)
                Attributes.Add(at);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual bool Validate(SourceProductionContext context) => true;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public virtual void Emit(
        SourceProductionContext context, CodeBuilder cb) => cb.AppendLine($"// {this}");
}