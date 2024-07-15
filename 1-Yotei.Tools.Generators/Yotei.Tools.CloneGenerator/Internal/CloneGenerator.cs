namespace Yotei.Tools.CloneGenerator;

// =========================================================
/// <inheritdoc cref="TreeGenerator"/>
[Generator(LanguageNames.CSharp)]
internal class CloneGenerator : TreeGenerator
{
#if DEBUG_CLONE_GENERATOR && DEBUG__
    /// <inheritdoc/>
    protected override bool LaunchDebugger => true;
#endif

    /// <inheritdoc/>
    protected override Type[] TypeAttributes { get; } = [typeof(CloneableAttribute)];

    /// <inheritdoc/>
    protected override TypeNode CreateNode(
        INode parent, TypeCandidate candidate) => new XTypeNode(parent, candidate);
}