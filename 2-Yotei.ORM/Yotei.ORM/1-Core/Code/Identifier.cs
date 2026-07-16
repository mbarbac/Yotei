namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IIdentifier"/>
/// </summary>
/// <remarks>
/// This type is not intended to be inherited.
/// </remarks>
[Cloneable(ReturnType = typeof(IIdentifier), UseVirtual = false)]
public sealed partial class Identifier : IIdentifier
{
    readonly IIdentifier.IBuilder Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public Identifier(IEngine engine) => Items = new Builder(engine);

    /// <summary>
    /// Initializes a new instance with the parts obtained from the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    /// <param name="reduce"></param>
    public Identifier(IEngine engine, string? value, bool reduce = true)
        => Items = new Builder(engine, value, reduce);

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
    Identifier(Identifier other) => Items = other.ThrowWhenNull().Items.Clone();

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
    /// <param name="useTerminators"></param>
    /// <returns></returns>
    public string ToStringEx(
        bool reduce = true, bool useTerminators = true) => Items.ToStringEx(reduce, useTerminators);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(IIdentifier? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        if (!Engine.Equals(other.Engine)) return false;

        // We control the underlying 'string?[]' type of the cached enumeration...
        var sources = (string?[])Enumerate(useTerminators: false);
        var targets = (string?[])other.Enumerate(useTerminators: false);
        if (sources.Length != targets.Length) return false;

        for (int i = 0; i < sources.Length; i++)
        {
            var xsource = sources[i];
            var xtarget = targets[i];
            if (string.Compare(xsource, xtarget, Engine.IgnoreCase) != 0) return false;
        }

        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as IIdentifier);

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
    public string? Value => field ??= Items.Value; // We'll recreate for null values, but's cheap!

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
    public IEnumerable<string?> Enumerate(bool useTerminators = false)
        => useTerminators
        ? _WithTerminators ??= [.. Items.Enumerate(true)]
        : _NoTerminators ??= [.. Items.Enumerate(false)];

    // Used to cache the parts of this instance, generating them once and by demand.
    string?[] _NoTerminators = null!;
    string?[] _WithTerminators = null!;

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
    public bool Match(string? specs)
    {
        var target = new Identifier(Engine, [specs]);

        // We cannot use the chached enumerations because we'll compare from right to left...
        var targets = target.Enumerate(useTerminators: false).ToList(); targets.Reverse();
        var sources = Enumerate(useTerminators: false).ToList(); sources.Reverse();

        var count = Math.Max(sources.Count, target.Count);
        for (var i = 0; i < count; i++)
        {
            var svalue = i < sources.Count ? sources[i] : null;
            var tvalue = i < targets.Count ? targets[i] : null;

            if (tvalue is null) continue;
            if (string.Compare(svalue, tvalue, Engine.IgnoreCase) != 0) return false;
        }

        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IIdentifier.IBuilder ToBuilder() => Items.Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IIdentifier Reduce()
    {
        var cloned = (Builder)Items.Clone();
        var done = cloned.Reduce();
        return done ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public IIdentifier Replace(int index, string? value, bool reduce = true)
    {
        var cloned = (Builder)Items.Clone();
        var num = cloned.Replace(index, value, reduce);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public IIdentifier Add(string? value, bool reduce = true)
    {
        var cloned = (Builder)Items.Clone();
        var num = cloned.Add(value, reduce);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// , bool reduce = true
    /// <returns></returns>
    public IIdentifier AddRange(IEnumerable<string?> range, bool reduce = true)
    {
        var cloned = (Builder)Items.Clone();
        var num = cloned.AddRange(range, reduce);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public IIdentifier Insert(int index, string? value, bool reduce = true)
    {
        var cloned = (Builder)Items.Clone();
        var num = cloned.Insert(index, value, reduce);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public IIdentifier InsertRange(int index, IEnumerable<string?> range, bool reduce = true)
    {
        var cloned = (Builder)Items.Clone();
        var num = cloned.InsertRange(index, range, reduce);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public IIdentifier RemoveAt(int index, bool reduce = true)
    {
        var cloned = (Builder)Items.Clone();
        var num = cloned.RemoveAt(index, reduce);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public IIdentifier RemoveRange(int index, int count, bool reduce = true)
    {
        var cloned = (Builder)Items.Clone();
        var num = cloned.RemoveRange(index, count, reduce);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public IIdentifier Remove(string? part, bool reduce = true)
    {
        var cloned = (Builder)Items.Clone();
        var num = cloned.Remove(part, reduce);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public IIdentifier RemoveLast(string? part, bool reduce = true)
    {
        var cloned = (Builder)Items.Clone();
        var num = cloned.RemoveLast(part, reduce);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public IIdentifier RemoveAll(string? part, bool reduce = true)
    {
        var cloned = (Builder)Items.Clone();
        var num = cloned.RemoveAll(part, reduce);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public IIdentifier Remove(Predicate<string?> predicate, bool reduce = true)
    {
        var cloned = (Builder)Items.Clone();
        var num = cloned.Remove(predicate, reduce);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public IIdentifier RemoveLast(Predicate<string?> predicate, bool reduce = true)
    {
        var cloned = (Builder)Items.Clone();
        var num = cloned.RemoveLast(predicate, reduce);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public IIdentifier RemoveAll(Predicate<string?> predicate, bool reduce = true)
    {
        var cloned = (Builder)Items.Clone();
        var num = cloned.RemoveAll(predicate, reduce);
        return num > 0 ? cloned.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IIdentifier Clear()
    {
        var cloned = (Builder)Items.Clone();
        var num = cloned.Clear();
        return num > 0 ? cloned.ToInstance() : this;
    }
}