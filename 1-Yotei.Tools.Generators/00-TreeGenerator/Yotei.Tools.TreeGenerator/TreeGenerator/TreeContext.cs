namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Augments the standard source code generation context with the options read from the consuming
/// project's csproj file.
/// </summary>
public struct TreeContext
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="options"></param>
    [SuppressMessage("", "IDE0290")]
    public TreeContext(SourceProductionContext context, TreeGeneratorOptions options)
    {
        Context = context;
        Options = options.ThrowWhenNull();
    }

    // ----------------------------------------------------

    /// <summary>
    /// The actual source code generator context of the associated generator.
    /// </summary>
    public SourceProductionContext Context { get; }

    /// <summary>
    /// The options read from the csproj file of the consuming project. The actual type of
    /// this property may be a derived type appropriate for the concrete generator.
    /// </summary>
    public TreeGeneratorOptions Options { get; }
}