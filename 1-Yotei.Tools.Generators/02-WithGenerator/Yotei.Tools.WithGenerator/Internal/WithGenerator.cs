namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
[Generator(LanguageNames.CSharp)]
internal class WithGenerator : CoreGenerator.CoreGenerator
{
#if DEBUG_WITH_GENERATOR_
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override bool LaunchDebugger => true;
#endif

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override bool UseTypeKind => true;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override bool UsePropertyKind => true;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override bool UseFieldKind => true;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override List<Type> TypeAttributes { get; } = [
        typeof(InheritsWithAttribute),
        typeof(InheritsWithAttribute<>),];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override List<Type> PropertyAttributes { get; } = [
        typeof(WithAttribute),
        typeof(WithAttribute<>),];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override List<Type> FieldAttributes { get; } = [
        typeof(WithAttribute),
        typeof(WithAttribute<>),];

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="candidate"></param>
    /// <returns></returns>
    protected override TypeNode CreateNode(TypeCandidate candidate)
    {
        var item = new XTypeNode(candidate.Symbol);
        if (candidate.Syntax != null) item.SyntaxNodes.Add(candidate.Syntax);
        item.Attributes.AddRange(candidate.Attributes);
        return item;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    protected override PropertyNode CreateNode(TypeNode parent, PropertyCandidate candidate)
    {
        var item = new XPropertyNode(parent, candidate.Symbol);
        if (candidate.Syntax != null) item.SyntaxNodes.Add(candidate.Syntax);
        item.Attributes.AddRange(candidate.Attributes);
        return item;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    protected override FieldNode CreateNode(TypeNode parent, FieldCandidate candidate)
    {
        var item = new XFieldNode(parent, candidate.Symbol);
        if (candidate.Syntax != null) item.SyntaxNodes.Add(candidate.Syntax);
        item.Attributes.AddRange(candidate.Attributes);
        return item;
    }
}