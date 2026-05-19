namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a tree-oriented source code generator's context, composed by the standard source
/// production context augmented with the options read from a consuming project configuration
/// file.
/// </summary>
public readonly struct TreeContext
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="options"></param>
    [SuppressMessage("", "IDE0290")]
    public TreeContext(SourceProductionContext context, TreeOptions options)
    {
        Context = context;
        Options = options.ThrowWhenNull();
    }

    // ----------------------------------------------------

    /// <summary>
    /// The actual source code generator context of the associated generator.
    /// </summary>
    public readonly SourceProductionContext Context { get; }

    /// <summary>
    /// The options read from the csproj file of the consuming project.
    /// </summary>
    public readonly TreeOptions Options { get; }
}