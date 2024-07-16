#pragma warning disable RS1024
#pragma warning disable IDE0019

namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Compares two candidate instances to facilitate caching.
/// </summary>
internal class CandidateComparer : IEqualityComparer<ICandidate>
{
    /// <inheritdoc/>
    public bool Equals(ICandidate x, ICandidate y)
    {
        if (x is null && y is null) return true;
        if (x is null) return false;
        if (y is null) return false;

        var xx = x as INodeCandidate;
        var yy = y as INodeCandidate;
        if (xx is null || yy is null) return false;

        if (!SymbolComparer.Default.Equals(xx.Symbol, yy.Symbol)) return false;
        return true;
    }

    /// <inheritdoc/>
    public int GetHashCode(ICandidate obj)
    {
        var code = HashCode.Combine(0);
        if (obj is not null)
        {
            if (obj is IErrorCandidate error) code = HashCode.Combine(code, error.Diagnostic);
            else if (obj is INodeCandidate node) code = HashCode.Combine(code, node.Symbol);
        }
        return code;
    }
}