namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a type-alike source generation node.
/// </summary>
internal class TypeNode : Node
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="candidate"></param>
    public TypeNode(TypeCandidate candidate) : base(candidate) { }

    /// <summary>
    /// Initializes a new instance not associated with a given candidate.
    /// </summary>
    /// <param name="symbol"></param>
    public TypeNode(INamedTypeSymbol symbol) : base(symbol) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Type: {Symbol.Name}";

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    public new INamedTypeSymbol Symbol => (INamedTypeSymbol)base.Symbol;

    /// <summary>
    /// The candidate this instance is associated with. The value of this property is null if
    /// this instance was not created by a generator tranforming a syntax node.
    /// </summary>
    public new TypeCandidate? Candidate => (TypeCandidate?)base.Candidate;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        var type = Symbol;
        while (type != null)
        {
            if (!context.TypeIsPartial(type)) return false;
            type = type.ContainingType;
        }

        if (!context.TypeIsSupported(Symbol)) return false;

        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="file"></param>
    public override void Print(SourceProductionContext context, FileBuilder file)
        => file.AppendLine($"// {ToString()}");
}