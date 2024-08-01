namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IEngineComparer"/>
public class EngineComparer : IEngineComparer
{
    /// <summary>
    /// A common shared instance.
    /// </summary>
    public static EngineComparer Instance { get; } = new();

    /// <inheritdoc/>
    public virtual bool Equals(IEngine? x, IEngine? y)
    {
        if (x is null && y is null) return true;
        if (x is null) return false;
        if (y is null) return false;

        if (x.CaseSensitiveNames != y.CaseSensitiveNames) return false;
        if (string.Compare(x.NullValueLiteral, y.NullValueLiteral, !x.CaseSensitiveNames) != 0) return false;
        if (x.PositionalParameters != y.PositionalParameters) return false;
        if (string.Compare(x.ParametersPrefix, y.ParametersPrefix, !x.CaseSensitiveNames) != 0) return false;
        if (x.UseTerminators != y.UseTerminators) return false;
        if (x.LeftTerminator != y.LeftTerminator) return false;
        if (x.RightTerminator != y.RightTerminator) return false;

        return true;
    }

    /// <inheritdoc/>
    public virtual int GetHashCode(IEngine obj)
    {
        var code = 0; if (obj is not null)
        {
            code = HashCode.Combine(obj.CaseSensitiveNames);
            code = HashCode.Combine(obj.NullValueLiteral);
            code = HashCode.Combine(obj.PositionalParameters);
            code = HashCode.Combine(obj.ParametersPrefix);
            code = HashCode.Combine(obj.UseTerminators);
            code = HashCode.Combine(obj.LeftTerminator);
            code = HashCode.Combine(obj.RightTerminator);
        }
        return code;
    }
}