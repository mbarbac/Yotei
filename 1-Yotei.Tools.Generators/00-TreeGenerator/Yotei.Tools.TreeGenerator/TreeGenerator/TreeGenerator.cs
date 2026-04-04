namespace Yotei.Tools.TreeGenerator;

// ========================================================
/// <summary>
/// Represents the base class for tree-oriented incremental source code generators that arrange
/// their captured elements in a tree structure where each top node corresponds to a single type
/// that also contains its captured child elements, if any. Then, each top node is emitted in its
/// own file.
/// <para>
/// Derived types shall be be decorated with the <see cref="GeneratorAttribute"/> attribute to be
/// recognized by the compiler. It is also expected that the <see cref="LanguageNames.CSharp"/>
/// value is used as that attribute's argument.
/// </para>
/// </summary>
internal partial class TreeGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Determines if this instance tries to launch a compile-time debug session when compiling.
    /// </summary>
    protected virtual bool LaunchDebugger => false;

    /// <summary>
    /// Determines if the <see cref="IsNullable{T}"/> and the <see cref="IsNullableAttribute"/>
    /// types shall be emitted in the namespace of the derived generator.
    /// </summary>
    protected virtual bool EmitNullabilityHelpers => true;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to register post-initialization actions such as reading external files, generating
    /// code for marker attributes, and so on.
    /// <br/> Inheritors must guarantee they invoke this base method.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        // TODO - OnInitialize
        return;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to quickly determine if the given syntax node shall be considered as a potential
    /// source code generation candidate, or not.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual bool FastPredicate(SyntaxNode node, CancellationToken token)
    {
        // TODO - FastPredicate
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// xxx
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual INode CaptureNode(GeneratorSyntaxContext context, CancellationToken token)
    {
        // TODO - CaptureNode
        return null!;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to generate the source code of the given collection of captured nodes.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="nodes"></param>
    protected virtual void EmitNodes(SourceProductionContext context, ImmutableArray<INode> nodes)
    {
        // TODO - EmitNodes
        return;
    }
}