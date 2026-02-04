namespace Yotei.Tools.CoreGenerator;

// It seems that if we capture the 'Compilation' object then we'll loose the incremental nature of
// the generator, so that even the smallest change will cause a full generator execution. Therefore,
// we shall not use it here. What I don't understand is why the 'context.CompilationProvider' can
// be used if such can happen.

// ========================================================
/// <summary>
/// Represents the base class for tree-oriented incremental source code generators that arrange
/// their captured elements in a hierarchical tree structure where each top node corresponds to
/// a single type, along with its child elements (if any), and its emitted in its own file.
/// <para>
/// Derived types shall be be decorated with the <see cref="GeneratorAttribute"/> attribute to be
/// recognized by the compiler. It is also expected that the <see cref="LanguageNames.CSharp"/>
/// value is used as that attribute's argument.
/// </para>
/// </summary>
internal class TreeGenerator
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Launching a compile-time debug session if needed...
        if (LaunchDebugger && !Debugger.IsAttached) Debugger.Launch();

        // Registering post-initialization actions....
        context.RegisterPostInitializationOutput(OnInitialize);

        // Registering pipeline steps...
        var items = context.SyntaxProvider
            .CreateSyntaxProvider(SyntaxPredicate, CreateNode)
            .Where(static x => x is not null)
            .Collect();

        // Registering source code emit actions...
        // var combined = context.CompilationProvider.Combine(items);
        context.RegisterSourceOutput(/*combined*/ items, EmitNodes);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance tries to launch a compile-time debug session when compiling.
    /// <br/> The default value of this property is <see langword="false"/>.
    /// </summary>
    protected virtual bool LaunchDebugger => false;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to register post-initialization actions such as reading external files, generating
    /// code for marker attributes, and so on. Derived types shall invoke this base method.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to quickly determine if the given syntax node shall be consider as a potential
    /// candidate for source code generation, or not. By default this method filters out those
    /// elements whose syntax kind is not allowed.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual bool SyntaxPredicate(SyntaxNode node, CancellationToken token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture a element for source code generation purposes. This method can also
    /// return <see langword="null"/> if the syntax node shall be ignored, or an error candidate
    /// to report error conditions at code generation time.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0019")]
    protected virtual INode CreateNode(
        GeneratorSyntaxContext context, CancellationToken token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to generate the source code of the given collection of nodes.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="nodes"></param>
    protected virtual void EmitNodes(
        SourceProductionContext context, ImmutableArray<INode> nodes)
    {
        throw null;
    }
}