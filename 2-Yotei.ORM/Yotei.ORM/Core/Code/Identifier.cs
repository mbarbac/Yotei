namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IIdentifier"/>
/// </summary>
[Cloneable(ReturnType = typeof(IIdentifier))]
public partial class Identifier : IIdentifier
{
    protected IIdentifier.IBuilder Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public Identifier(IEngine engine) => Items = new Builder(engine);

    /// <summary>
    /// Initializes a new instance with the parts obtained from the given range of values.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    /// <param name="reduce"></param>
    public Identifier(
        IEngine engine, IEnumerable<string?> range, bool reduce = true)
        => Items = new Builder(engine, range, reduce);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected Identifier(Identifier other) => Items = other.ThrowWhenNull().Items.Clone();

    /// <summary>
    /// <inheritdoc/> This method returns a reduced string by removing the null heading parts,
    /// if any.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Items.ToString() ?? string.Empty;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="reduce"></param>
    /// <param name="wrap"></param>
    /// <returns></returns>
    public virtual string ToStringEx(bool reduce = true, bool wrap = true) => Items.ToStringEx(reduce, wrap);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(IIdentifier? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        if (!Engine.Equals(other.Engine)) return false;

        var source = Reduce();
        var target = other.Reduce();
        if (source.Count != target.Count) return false;
        for (int i = 0; i < source.Count; i++)
        {
            var xsource = source[i];
            var xtarget = target[i];
            if (string.Compare(xsource, xtarget, Engine.IgnoreCase) != 0) return false;
        }

        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as IMetadataTag);

    public static bool operator ==(Identifier? host, IIdentifier? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(Identifier? host, IIdentifier? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Engine);
        code = HashCode.Combine(code, Count);
        foreach (var item in Items.Enumerate(useTerminators: false)) code = HashCode.Combine(code, item);
        return code;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine => Items.Engine;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? Value => field ??= Items.Value;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public string? this[int index] => Items[index];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="useTerminators"></param>
    /// <returns></returns>
    public string? this[int index, bool useTerminators] => Items[index, useTerminators];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="useTerminators"></param>
    /// <returns></returns>
    public IEnumerable<string?> Enumerate(bool useTerminators = false) => Items.Enumerate(useTerminators);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public bool Contains(string? part) => Items.Contains(part);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public int IndexOf(string? part) => Items.IndexOf(part);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public int LastIndexOf(string? part) => Items.LastIndexOf(part);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public List<int> IndexesOf(string? part) => Items.IndexesOf(part);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<string?> predicate) => Items.Contains(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<string?> predicate) => Items.IndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<string?> predicate) => Items.LastIndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<string?> predicate) => Items.IndexesOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public bool Match(string? specs) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IIdentifier.IBuilder ToBuilder() => Items.Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IIdentifier Reduce()
    {
        var cloned = (Builder)Clone();
        var done = cloned.Reduce();
        return done ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IIdentifier Replace(int index, string? value)
    {
        var cloned = (Builder)Clone();
        var num = cloned.Replace(index, value);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IIdentifier Add(string? value)
    {
        var cloned = (Builder)Clone();
        var num = cloned.Add(value);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IIdentifier AddRange(IEnumerable<string?> range)
    {
        var cloned = (Builder)Clone();
        var num = cloned.AddRange(range);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IIdentifier Insert(int index, string? value)
    {
        var cloned = (Builder)Clone();
        var num = cloned.Insert(index, value);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IIdentifier InsertRange(int index, IEnumerable<string?> range)
    {
        var cloned = (Builder)Clone();
        var num = cloned.InsertRange(index, range);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual IIdentifier RemoveAt(int index)
    {
        var cloned = (Builder)Clone();
        var num = cloned.RemoveAt(index);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IIdentifier RemoveRange(int index, int count)
    {
        var cloned = (Builder)Clone();
        var num = cloned.RemoveRange(index, count);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public virtual IIdentifier Remove(string? part)
    {
        var cloned = (Builder)Clone();
        var num = cloned.Remove(part);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public virtual IIdentifier RemoveLast(string? part)
    {
        var cloned = (Builder)Clone();
        var num = cloned.RemoveLast(part);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public virtual IIdentifier RemoveAll(string? part)
    {
        var cloned = (Builder)Clone();
        var num = cloned.RemoveAll(part);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IIdentifier Remove(Predicate<string?> predicate)
    {
        var cloned = (Builder)Clone();
        var num = cloned.Remove(predicate);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IIdentifier RemoveLast(Predicate<string?> predicate)
    {
        var cloned = (Builder)Clone();
        var num = cloned.RemoveLast(predicate);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IIdentifier RemoveAll(Predicate<string?> predicate)
    {
        var cloned = (Builder)Clone();
        var num = cloned.RemoveAll(predicate);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IIdentifier Clear()
    {
        var cloned = (Builder)Clone();
        var num = cloned.Clear();
        return num > 0 ? cloned.ToInstance() : this;
    }
}