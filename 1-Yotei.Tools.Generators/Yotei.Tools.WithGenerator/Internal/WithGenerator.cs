namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Generates 'With()' methods for types or member decorated with the <see cref="WithAttribute"/>.
/// </summary>
//[Generator(LanguageNames.CSharp)]
internal class WithGenerator : TreeGenerator
{
#if DEBUG_WITH_GENERATOR
    /// <inheritdoc/>
    //protected override bool LaunchDebugger => true;
#endif

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override Type[] TypeAttributes { get; } = [typeof(WithAttribute)];

    /// <inheritdoc/>
    protected override Type[] PropertyAttributes { get; } = [typeof(WithAttribute)];

    /// <inheritdoc/>
    protected override Type[] FieldAttributes { get; } = [typeof(WithAttribute)];

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override TypeNode CreateNode(
        INode parent, TypeCandidate candidate) => new XTypeNode(parent, candidate);

    /// <inheritdoc/>
    protected override PropertyNode CreateNode(
        TypeNode parent, PropertyCandidate candidate) => new XPropertyNode(parent, candidate);

    /// <inheritdoc/>
    protected override FieldNode CreateNode(
        TypeNode parent, FieldCandidate candidate) => new XFieldNode(parent, candidate);
}