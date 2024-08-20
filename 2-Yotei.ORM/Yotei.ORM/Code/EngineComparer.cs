using T = Yotei.ORM.IEngine;

namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// Defines methods that support comparison of <see cref="T"/> instances.
/// </summary>
public class EngineComparer : IEqualityComparer<T>
{
    /// <summary>
    /// A common shared instance.
    /// </summary>
    public static EngineComparer Instance { get; } = new();

    /// <inheritdoc/>
    public virtual bool Equals(T? x, T? y)
    {
        if (x is null && y is null) return true;
        if (x is null) return false;
        if (y is null) return false;

        if (x.CaseSensitiveNames != y.CaseSensitiveNames) return false;
        if (string.Compare(x.NullValueLiteral, y.NullValueLiteral, !x.CaseSensitiveNames) != 0) return false;
        if (x.PositionalParameters != y.PositionalParameters) return false;
        if (string.Compare(x.ParametersPrefix, y.ParametersPrefix, !x.CaseSensitiveNames) != 0) return false;
        if (x.NativePaging != y.NativePaging) return false;
        if (x.UseTerminators != y.UseTerminators) return false;
        if (x.LeftTerminator != y.LeftTerminator) return false;
        if (x.RightTerminator != y.RightTerminator) return false;

        return true;
    }

    /// <inheritdoc/>
    public virtual int GetHashCode(T obj)
    {
        var code = 0; if (obj is not null)
        {
            code = HashCode.Combine(code, obj.CaseSensitiveNames);
            code = HashCode.Combine(code, obj.NullValueLiteral);
            code = HashCode.Combine(code, obj.PositionalParameters);
            code = HashCode.Combine(code, obj.ParametersPrefix);
            code = HashCode.Combine(code, obj.NativePaging);
            code = HashCode.Combine(code, obj.UseTerminators);
            code = HashCode.Combine(code, obj.LeftTerminator);
            code = HashCode.Combine(code, obj.RightTerminator);
        }
        return code;
    }
}