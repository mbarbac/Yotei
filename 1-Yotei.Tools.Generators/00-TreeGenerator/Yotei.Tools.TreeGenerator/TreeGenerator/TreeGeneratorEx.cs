namespace Yotei.Tools.Generators;

// ========================================================
partial class TreeGenerator
{
    /// <summary>
    /// <inheritdoc/>
    /// This method is INFRASTRUCTURE only, and it is only intended to be invoked by the compiler.
    /// Application code shall not invoke it.
    /// </summary>
    /// <param name="context"></param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Registering post-initialization actions...
        context.RegisterPostInitializationOutput(DispatchInitialize);

        // Registering pipeline steps...
        var items = context.SyntaxProvider
            .CreateSyntaxProvider(FastPredicate, CaptureCandidate)
            .Where(static x => x is not null)
            .Collect();

        // Registering source code emit actions...
        context.RegisterSourceOutput(items, EmitCandidates);
    }

    /* NOTE:
     * As per my understanding of the documentation, if we capture the 'Compilation' object we
     * will then loose the incremental nature of the generator (so that it says that the slightest
     * change or user typing will drive a full source generation over and over again).
     * What I cannot understand is why then it has been made possible:
     *      var combined = context.CompilationProvider.Combine(items);
     *      context.RegisterSourceOutput(combined, EmitNodes);
     * (provided an appropriate signature for the EmitNodes method).
     */

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to dispatch post-initialization actions.
    /// </summary>
    /// <param name="context"></param>
    void DispatchInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        // HIGH: DispatchInitialize
        return;
    }

    // ----------------------------------------------------
}