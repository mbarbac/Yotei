namespace Yotei.Tools.TreeGenerator;

// ========================================================
/// </summary>
partial class TreeGenerator
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    public virtual void Initialize(IncrementalGeneratorInitializationContext context)
    {
        try
        {
            // Launching a compile-time debug session if requested...
            if (LaunchDebugger && !Debugger.IsAttached) Debugger.Launch();

            // Registering post-initialization actions...
            context.RegisterPostInitializationOutput(OnInitialize);

            // Registering pipeline steps...
            var items = context.SyntaxProvider
                .CreateSyntaxProvider(FastPredicate, CaptureNode)
                .Where(static x => x is not null)
                .Collect();

            // Registering source code generation actions...
            context.RegisterSourceOutput(items, EmitNodes);
        }
        catch (Exception ex)
        {
            context.RegisterPostInitializationOutput(
                x => x.AddSource("Error.g.cs", $"/* {ex.ToDisplayString()} */"));
        }

        /* NOTE:
         * It looks like that if we capture the 'Compilation' object, we'll loose the incremental
         * nature of the generator and smallest change will trigger a full source code generation
         * over and over again. What I cannot understand is why then there is an API that permit
         * precissely that, as follows (with an appropriate 'EmitNodes' signature):
         *      var combined = context.CompilationProvider.Combine(items);
         *      context.RegisterSourceOutput(combined items, EmitNodes);
        */
    }

    // ----------------------------------------------------
}