namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents the source code generation production context augmented with the options read from
/// the consuming project configuration file.
/// </summary>
public struct TreeContext
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
    public SourceProductionContext Context { get; }

    /// <summary>
    /// The options read from the csproj file of the consuming project.
    /// </summary>
    public TreeOptions Options { get; }
}