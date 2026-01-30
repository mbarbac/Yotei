namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal class CoreGenerator : IIncrementalGenerator
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
            .CreateSyntaxProvider(ValidateNode, CaptureCandidate)
            .Where(static x => x is not null)
            .Collect();

        var combined = context.CompilationProvider.Combine(items);

        // Registering source code emit actions...
        context.RegisterSourceOutput(combined, EmitCode);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance tries to launch a compile-time debug session when compiling.
    /// <br/> The default value of this property is <see langword="false"/>.
    /// </summary>
    protected virtual bool LaunchDebugger => false;

    /// <summary>
    /// If true, emits the <see cref="IsNullable{T}"/> and <see cref="IsNullableAttribute"/> types
    /// on the namespace of the generator.
    /// <br/> The default value of this property is <see langword="false"/>.
    /// </summary>
    protected virtual bool EmitTypeNullabilityHelpers => false;

    /// <summary>
    /// Invoked to register post-initialization actions, such as reading external files, or
    /// generating code for marker attributes.
    ///     /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        if (EmitTypeNullabilityHelpers)
        {
            var ns = GetType().Namespace!;
            var str = $$"""
                using System;

                namespace {{ns}}
                {
                    /// <summary>
                    /// <inheritdoc cref="Yotei.Tools.CoreGenerator.IsNullable{T}"/>
                    /// </summary>
                    public class IsNullable<T> { }
                    
                    /// <summary>
                    /// <inheritdoc cref="IsNullable{T}"/>
                    /// </summary>
                    [AttributeUsage(AttributeTargets.All)]
                    public class IsNullableAttribute : Attribute { }
                }
                """;

            context.AddEmbeddedAttributeDefinition();
            context.AddSource("IsNullable[T].g.cs", str);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to quickly determine if the given syntax node shall be considered as a potential
    /// candidate for source code generation, or not. Typically, this method just filters out all
    /// syntax kinds except the ones the generator is interested in.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual bool ValidateNode(SyntaxNode node, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        return node
            is TypeDeclarationSyntax
            or BasePropertyDeclarationSyntax
            or FieldDeclarationSyntax
            or BaseMethodDeclarationSyntax;
    }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of attribute types this generator uses, by default, to identify decorated
    /// types as source code generation candidates.
    /// </summary>
    protected virtual List<Type> TypeAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types this generator uses, by default, to identify decorated
    /// properties as source code generation candidates.
    /// </summary>
    protected virtual List<Type> PropertyAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types this generator uses, by default, to identify decorated
    /// fields as source code generation candidates.
    /// </summary>
    protected virtual List<Type> FieldAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types this generator uses, by default, to identify decorated
    /// methods as source code generation candidates.
    /// </summary>
    protected virtual List<Type> MethodAttributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// The collection of fully qualified attribute type names used by this generator, by default,
    /// to identify decorated types as source code generation candidates. This default generator
    /// uses these names only if there is no match for the specified types, if any.
    /// </summary>
    protected virtual List<string> TypeAttributeNames { get; } = [];

    /// <summary>
    /// The collection of fully qualified attribute type names used by this generator, by default,
    /// to identify decorated properties as source code generation candidates. This default generator
    /// uses these names only if there is no match for the specified types, if any.
    /// </summary>
    protected virtual List<string> PropertyAttributeNames { get; } = [];

    /// <summary>
    /// The collection of fully qualified attribute type names used by this generator, by default,
    /// to identify decorated fields as source code generation candidates. This default generator
    /// uses these names only if there is no match for the specified types, if any.
    /// </summary>
    protected virtual List<string> FieldAttributeNames { get; } = [];

    /// <summary>
    /// The collection of fully qualified attribute type names used by this generator, by default,
    /// to identify decorated methods as source code generation candidates. This default generator
    /// uses these names only if there is no match for the specified types, if any.
    /// </summary>
    protected virtual List<string> MethodAttributeNames { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture a candidate for source code generation purposes using the given context.
    /// This method may also return <see langword="null"/> if the syntax node carried by the context
    /// shall be ignored, or an error candidate if there is the need to report an error condition.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICandidate CaptureCandidate(GeneratorSyntaxContext context, CancellationToken token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the code of the collection of captured candidates, using the given source
    /// production context, and the compilation object.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="source"></param>
    protected virtual void EmitCode(
        SourceProductionContext context, (Compilation, ImmutableArray<ICandidate>) source)
    {
        throw null;
    }
}