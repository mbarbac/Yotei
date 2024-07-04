namespace Yotei.Tools.Generators.Dev;

// =========================================================
/// <inheritdoc/>
[Generator(LanguageNames.CSharp)]
internal class DevGenerator : TreeGenerator
{
#if DEBUG_DEV_GENERATOR && DEBUG
    /// <inheritdoc/>
    protected override bool LaunchDebugger => true;
#endif

    /// <inheritdoc/>
    protected override Type[] TypeAttributes { get; } = [typeof(FakeAttribute)];

    /// <inheritdoc/>
    protected override TypeNode CreateNode(
        INode parent, TypeCandidate candidate) => new XTypeNode(parent, candidate);
}