namespace Yotei.Tools.Generators;

/* To DEBUG:
 * - Install the .NET Compiler SDK (in addition to Roslyn components).
 * - Make sure the derived generator project is a Roslyn component:
 *      <IsRoslynComponent>true</IsRoslynComponent>
 * - In the derived generator project's properties, add a Roslyn debug profile, whose target being
 *   the project that, when compiled, will be debugged (ie: a test project). This, in turn, debugs
 *   the generator project as well as a kind-of side-effect.
 * - Mark the derived generator project as the startup one (not the test one!).
 * - In the play button, select the debug profile.
 * - Click F5 (run) to compile (F6 does nothing).
 */

// ========================================================
/// <summary>
/// Represents a tree-oriented incremental source generator that organizes the captured nodes in
/// a hierarchycal tree where each top-most element will be emitted in its own file and correspond
/// to a captured type along with its child elements, if any.
/// <para>
/// Derived types need to be decorated with the <see cref="GeneratorAttribute"/> attribute to be
/// recognized by the compiler. In is also expected that <see cref="LanguageNames.CSharp"/> is used
/// as its argument.
/// </para>
/// </summary>
public partial class TreeGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Invoked to register post-initialization actions, such as reading external source files, or
    /// generating code for marker attributes, among others. By default, this base method adds the
    /// <see langword="Microsoft.CodeAnalysis.Embedded"/> attribute to the compilation.
    /// <br/> Inheritors may want to invoke their base method first.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddEmbeddedAttributeDefinition();
    }

    // ----------------------------------------------------

    protected virtual bool TreePredicate(SyntaxNode node, CancellationToken token)
    {
        // TODO: FastPredicate
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to transform the potential syntax node candidate carried by the given context into
    /// a source code tree-oriented generator one. This method may also return error nodes that
    /// carry diagnostics to be reported, or <see langword="null"/> if the syntax node is to be
    /// completely ignored.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual INode CaptureNode(GeneratorSyntaxContext context, CancellationToken token)
    {
        // TODO: CaptureNode
        return null!;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to both generate the source code of the captured nodes, and to report the error
    /// conditions the error nodes may carry with them.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="source"></param>
    protected virtual void EmitNodes(
        SourceProductionContext context,
        ImmutableArray<INode> source)
    {
        // TODO: EmitNodes
        return;
    }
}