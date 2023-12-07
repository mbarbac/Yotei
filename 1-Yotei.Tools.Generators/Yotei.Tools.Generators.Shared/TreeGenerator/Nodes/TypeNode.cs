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
    /// <inheritdoc cref="Node.Candidate"/>
    /// </summary>
    public new TypeCandidate? Candidate => (TypeCandidate?)base.Candidate;

    /// <summary>
    /// <inheritdoc cref="Node.Symbol"/>
    /// </summary>
    public new INamedTypeSymbol Symbol => (INamedTypeSymbol)base.Symbol;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        // Chain of types must be partial ones...
        var type = Symbol;
        while (type != null)
        {
            if (!type.TypeIsPartial(context)) return false;
            type = type.ContainingType;
        }

        // Type must be of a supported kind...
        if (!Symbol.TypeIsSupported(context)) return false;

        // Validated...
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="file"></param>
    public override void Print(SourceProductionContext context, FileBuilder file)
    {
        file.AppendLine($"// {ToString()}");
    }
}