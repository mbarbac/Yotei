using T = Yotei.ORM.IIdentifierChain;

namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// Defines methods that support comparison of <see cref="T"/> instances.
/// </summary>
public class IdentifierChainComparer : IEqualityComparer<T>
{
    /// <summary>
    /// A common shared instance.
    /// </summary>
    public static IdentifierChainComparer Instance { get; } = new();

    /// <inheritdoc/>
    public virtual bool Equals(T? x, T? y)
    {
        if (x is null && y is null) return true;
        if (x is null) return false;
        if (y is null) return false;

        if (!EngineComparer.Instance.Equals(x.Engine, y.Engine)) return false;
        return string.Compare(x.Value, y.Value, x.Engine.CaseSensitiveNames) == 0;
    }

    /// <inheritdoc/>
    public virtual int GetHashCode(T obj)
    {
        var code = 0; if (obj is not null)
        {
            code = HashCode.Combine(code, obj.Value);
        }
        return code;
    }
}