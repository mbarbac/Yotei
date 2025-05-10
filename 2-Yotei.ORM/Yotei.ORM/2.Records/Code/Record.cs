namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IRecord"/>
[Cloneable]
[InheritWiths]
[DebuggerDisplay("{Items.ToDebugString(5)}")]
public partial class Record : IRecord
{
    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Record(Record source) => throw null;

    /// <inheritdoc/>
    public IEnumerator<object?> GetEnumerator() => throw null;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IRecord? other)
    {
        throw null;
    }

    /*
     /// <inheritdoc/>
    public bool Equals(IHost? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null) return false;

        if (!Engine.Equals(other.Engine)) return false;
        if (Count != other.Count) return false;

        for (int i = 0; i < Count; i++)
            if (!Items[i].EqualsEx(other[i])) return false;

        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IHost);

    public static bool operator ==(THost? host, IHost? item) // Use 'is' instead of '=='...
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(THost? host, IHost? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Engine);
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }
     */

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IRecord.IBuilder GetBuilder() => throw null;
}