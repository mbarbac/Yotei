namespace Experimental.Generator;

// =========================================================
/// <inheritdoc/>
[Generator(LanguageNames.CSharp)]
internal class XCloneGenerator : TreeGenerator
{
#if DEBUG_EXPERIMENTAL_GENERATOR && DEBUG
    /// <inheritdoc/>
    protected override bool LaunchDebugger => true;
#endif

    /// <inheritdoc/>
    protected override Type[] TypeAttributes { get; } = [typeof(UpCastAttribute<>)];

    /// <inheritdoc/>
    protected override TypeNode CreateNode(
        INode parent, TypeCandidate candidate) => new XTypeNode(parent, candidate);
}