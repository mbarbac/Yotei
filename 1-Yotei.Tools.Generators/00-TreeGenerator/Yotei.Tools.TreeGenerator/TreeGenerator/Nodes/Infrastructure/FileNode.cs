namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents the top-most node in a hierarchy, which essentially is the file where the generated
/// source code of its child elements will be emitted at.
/// </summary>
public sealed class FileNode : INode
{

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public CustomList<Diagnostic> Diagnostics { get; } = [];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public INode? Parent { get; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// Equality semantics are customized for generator caching purposes.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(INode other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not FileNode valid) return false;

        // TODO: FileNode Equals...
        if (!Diagnostics.SequenceEqual(valid.Diagnostics)) return false;
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        int code = HashCode.Combine(Diagnostics);
        foreach (var item in Diagnostics) code = HashCode.Combine(code, item);

        // TODO: FileNode GetHashCode...
        return code;
    }
}