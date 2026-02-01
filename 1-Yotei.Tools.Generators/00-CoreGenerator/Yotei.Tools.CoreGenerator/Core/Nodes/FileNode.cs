namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a file in a source code generation hierarchy. Files are typically associated with
/// individual types, either by they being decorated with recognized attributes, and/or because
/// their child elements.
/// </summary>
internal class FileNode : INode
{
    /// <summary>
    /// Initializes a new instance. Note that its <see cref="Node"/> property must be set
    /// before this instance can be used.
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="filename"></param>
    [SuppressMessage("", "IDE0290")]
    public FileNode(Compilation compilation, string filename)
    {
        Compilation = compilation.ThrowWhenNull();
        FileName = filename;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"File: {FileName}";

    /// <summary>
    /// The compilation captured for this instance.
    /// </summary>
    public Compilation Compilation { get; }

    /// <summary>
    /// The name of the file represented by this instance. The actual name used by the generator
    /// will be the value of this property followed by ".g.cs".
    /// </summary>
    public string FileName { get; init => field = value.NotNullNotEmpty(true); }

    /// <summary>
    /// The type node this instance is associated with.
    /// If <see langword="null"/>, then this instance is not a valid one.
    /// </summary>
    public TypeNode? Node { get; set => field = value.ThrowWhenNull(); }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual bool Validate(SourceProductionContext context) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public virtual void Emit(SourceProductionContext context, CodeBuilder cb) => throw null;
}