namespace Yotei.Tools.FrozenGenerator;

// =========================================================
/// <inheritdoc cref="TreeGenerator"/>
[Generator(LanguageNames.CSharp)]
internal class FrozenGenerator : TreeGenerator
{
#if DEBUG_FROZEN_GENERATOR && DEBUG
    /// <inheritdoc/>
    protected override bool LaunchDebugger => true;
#endif

    /// <inheritdoc/>
    protected override Type[] TypeAttributes { get; } = [
        typeof(IFrozenListAttribute),
        typeof(IFrozenListAttribute<>),
        typeof(IFrozenListAttribute<,>),
        typeof(FrozenListAttribute),
        typeof(FrozenListAttribute<>),
        typeof(FrozenListAttribute<,>),];

    /// <inheritdoc/>
    protected override TypeNode CreateNode(
        INode parent, TypeCandidate candidate) => new XTypeNode(parent, candidate);
}