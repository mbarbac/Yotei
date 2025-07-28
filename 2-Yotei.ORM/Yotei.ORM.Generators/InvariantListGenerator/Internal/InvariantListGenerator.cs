namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Manages the implementation of the 'IInvariantList' or 'InvariantList' types on the decorated
/// host ones.
/// </summary>
[Generator(LanguageNames.CSharp)]
internal class InvariantListGenerator : TreeGenerator
{
#if DEBUG_INVARIANTLIST_GENERATOR
    /// <inheritdoc/>
    protected override bool LaunchDebugger => true;
#endif

    /// <inheritdoc/>
    protected override Type[] TypeAttributes { get; } = [
        typeof(IInvariantListAttribute),
        typeof(IInvariantListAttribute<>),
        typeof(IInvariantListAttribute<,>),
        typeof(InvariantListAttribute),
        typeof(InvariantListAttribute<>),
        typeof(InvariantListAttribute<,>)];

    /// <inheritdoc/>
    protected override TypeNode CreateNode(
        INode parent, TypeCandidate candidate) => new XTypeNode(parent, candidate);
}