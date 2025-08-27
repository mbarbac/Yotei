namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Generates 'With[Name](value)' methods for members decorated with <see cref="WithAttribute"/>
/// attributes, or types decorated with <see cref="InheritWithsAttribute"/>.
/// </summary>
[Generator(LanguageNames.CSharp)]
internal class WithGenerator : TreeGenerator
{
#if DEBUG_WITH_GENERATOR
    /// <inheritdoc/>
    protected override bool LaunchDebugger => true;
#endif

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override Type[] TypeAttributes { get; } = [
        typeof(InheritWithsAttribute),
        typeof(InheritWithsAttribute<>)];

    /// <inheritdoc/>
    protected override Type[] PropertyAttributes { get; } = [
        typeof(WithAttribute),
        typeof(WithAttribute<>)];

    /// <inheritdoc/>
    protected override Type[] FieldAttributes { get; } = [
        typeof(WithAttribute),
        typeof(WithAttribute<>)];

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