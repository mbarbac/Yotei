namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Manages the implementation of 'InvariantList' or 'IInvariantList' derived classes and
/// interfaces.
/// </summary>
[Generator(LanguageNames.CSharp)]
internal class InvariantListGenerator : TreeGenerator
{
#if DEBUG_INVARIANT_GENERATOR
    /// <inheritdoc/>
    protected override bool LaunchDebugger => true;
#endif

    /// <inheritdoc/>
    protected override Type[] TypeAttributes { get; } = [
        typeof(IInvariantListAttribute),
        typeof(InvariantListAttribute),
        typeof(IInvariantListAttribute<>),
        typeof(IInvariantListAttribute<,>),
        typeof(InvariantListAttribute<>),
        typeof(InvariantListAttribute<,>),
    ];

    /// <inheritdoc/>
    protected override TypeNode CreateNode(
        INode parent, TypeCandidate candidate) => new XTypeNode(parent, candidate);
}