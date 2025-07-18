namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// Generates 'Clone()' methods for types decorated with the <see cref="CloneableAttribute"/>.
/// </summary>
[Generator(LanguageNames.CSharp)]
internal class CloneGenerator : TreeGenerator
{
#if DEBUG_CLONE_GENERATOR
    /// <inheritdoc/>
    protected override bool LaunchDebugger => true;
#endif

    /// <inheritdoc/>
    protected override Type[] TypeAttributes { get; } = [typeof(CloneableAttribute)];

    /// <inheritdoc/>
    protected override TypeNode CreateNode(
        INode parent, TypeCandidate candidate) => new XTypeNode(parent, candidate);
}