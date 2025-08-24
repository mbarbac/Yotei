namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <inheritdoc/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        return base.Validate(context);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override string? GetHeader(SourceProductionContext context)
    {
        return base.GetHeader(context);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        base.EmitCore(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is an interface.
    /// </summary>
    void EmitHostInterface(SourceProductionContext _, CodeBuilder cb)
    {
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is an abstract type.
    /// </summary>
    void EmitHostAbstract(SourceProductionContext _, CodeBuilder cb)
    {
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is a concrete type.
    /// </summary>
    void EmitHostConcrete(SourceProductionContext _, CodeBuilder cb)
    {
    }

    // ----------------------------------------------------

    
}