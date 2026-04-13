namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a generic error node that produces no generated code, but only carries diagnostics.
/// </summary>
public sealed class ErrorNode : INode
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public ErrorNode() { }

    /// <summary>
    /// Initializes a new instance with the given diagnostic.
    /// </summary>
    /// <param name="item"></param>
    public ErrorNode(Diagnostic item) => Diagnostics.Add(item);

    /// <summary>
    /// Initializes a new instance with the diagnostics of the given range.
    /// </summary>
    /// <param name="range"></param>
    public ErrorNode(IEnumerable<Diagnostic> range)
    {
        ArgumentNullException.ThrowIfNull(range);
        Diagnostics.AddRange(range);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Diagnostics.Count}";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public List<Diagnostic> Diagnostics { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(INode other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not ErrorNode valid) return false;

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
        return code;
    }
}