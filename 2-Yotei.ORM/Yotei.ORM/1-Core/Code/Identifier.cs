namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IIdentifier"/>
/// </summary>
[Cloneable(ReturnType = typeof(IIdentifier))]
public sealed partial class Identifier : IIdentifier
{
    readonly Builder Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public Identifier(IEngine engine) => Items = new(engine);

    /// <summary>
    /// Initializes a new instance with the given collection of parts.
    /// </summary>
    /// <param name="engine"></param>
    public Identifier(IEngine engine, IEnumerable<string?> range)
    {
        Items = new(engine, range);
        Items.Trim();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    Identifier(Identifier other)
    {
        Items = (Builder)other.ThrowWhenNull().Items.Clone();
        Items.Trim();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => _ToString ??= Items.ToString();
    string? _ToString;

    /// <summary>
    /// <inheritdoc/>
    /// <param name="reduce"></param>
    /// <param name="useTerminators"></param>
    /// <returns></returns>
    public string ToString(
        bool reduce, bool useTerminators) => Items.ToString(reduce, useTerminators);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public IEnumerator<string?> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IIdentifier.IBuilder ToBuilder() => Items.Clone();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns><inheritdoc/></returns>
    public bool Equals(IIdentifier? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not IIdentifier valid) return false;

        if (!Engine.Equals(other.Engine)) return false;

        if (Count != valid.Count) return false;
        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var temp = other[i];
            if (string.Compare(item, temp, Engine.IgnoreCase) != 0) return false;
        }
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns><inheritdoc/></returns>
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
    /// <returns><inheritdoc/></returns>
    public override int GetHashCode()
    {
        var code = Engine.GetHashCode();
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, this[i]);
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
    public string? Value => _Value ??= Items.Value;
    string? _Value;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="reduce"></param>
    /// <param name="useTerminators"></param>
    /// <returns></returns>
    public IEnumerable<string?> Enumerate(
        bool reduce, bool useTerminators) => Items.Enumerate(reduce, useTerminators);

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
    public void Trim() => Items.Trim();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public IIdentifier Reduce()
    {
        var builer = ToBuilder();
        var done = builer.Reduce();
        return done == 0 ? this : builer.ToInstance();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns><inheritdoc/></returns>
    public IIdentifier GetRange(int index, int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(count, Count - index);

        if (index == 0 && count == Count) return this; // Requesting same instance...
        if (index == 0 && count == 0) return Clear(); // Requesting cleared instance...

        var items = Items.Enumerate(false, false).ToList().GetRange(index, count);
        return new Identifier(Engine, items);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns><inheritdoc/></returns>
    public IIdentifier Replace(int index, string? value)
    {
        var builer = ToBuilder();
        var done = builer.Replace(index, value);
        return done == 0 ? this : builer.ToInstance();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns><inheritdoc/></returns>
    public IIdentifier Add(string? value)
    {
        var builer = ToBuilder();
        var done = builer.Add(value);
        return done == 0 ? this : builer.ToInstance();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns><inheritdoc/></returns>
    public IIdentifier AddRange(IEnumerable<string?> range)
    {
        var builer = ToBuilder();
        var done = builer.AddRange(range);
        return done == 0 ? this : builer.ToInstance();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns><inheritdoc/></returns>
    public IIdentifier Insert(int index, string? value)
    {
        var builer = ToBuilder();
        var done = builer.Insert(index, value);
        return done == 0 ? this : builer.ToInstance();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns><inheritdoc/></returns>
    public IIdentifier InsertRange(int index, IEnumerable<string?> range)
    {
        var builer = ToBuilder();
        var done = builer.InsertRange(index, range);
        return done == 0 ? this : builer.ToInstance();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns><inheritdoc/></returns>
    public IIdentifier RemoveAt(int index)
    {
        var builer = ToBuilder();
        var done = builer.RemoveAt(index);
        return done == 0 ? this : builer.ToInstance();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns><inheritdoc/></returns>
    public IIdentifier RemoveRange(int index, int count)
    {
        var builer = ToBuilder();
        var done = builer.RemoveRange(index, count);
        return done == 0 ? this : builer.ToInstance();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns><inheritdoc/></returns>
    public IIdentifier Remove(string? part)
    {
        var builer = ToBuilder();
        var done = builer.Remove(part);
        return done == 0 ? this : builer.ToInstance();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns><inheritdoc/></returns>
    public IIdentifier RemoveLast(string? part)
    {
        var builer = ToBuilder();
        var done = builer.RemoveLast(part);
        return done == 0 ? this : builer.ToInstance();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns><inheritdoc/></returns>
    public IIdentifier RemoveAll(string? part)
    {
        var builer = ToBuilder();
        var done = builer.RemoveAll(part);
        return done == 0 ? this : builer.ToInstance();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns><inheritdoc/></returns>
    public IIdentifier Remove(Predicate<string?> predicate)
    {
        var builer = ToBuilder();
        var done = builer.Remove(predicate);
        return done == 0 ? this : builer.ToInstance();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns><inheritdoc/></returns>
    public IIdentifier RemoveLast(Predicate<string?> predicate)
    {
        var builer = ToBuilder();
        var done = builer.RemoveLast(predicate);
        return done == 0 ? this : builer.ToInstance();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns><inheritdoc/></returns>
    public IIdentifier RemoveAll(Predicate<string?> predicate)
    {
        var builer = ToBuilder();
        var done = builer.RemoveAll(predicate);
        return done == 0 ? this : builer.ToInstance();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public IIdentifier Clear()
    {
        var builer = ToBuilder();
        var done = builer.Clear();
        return done == 0 ? this : builer.ToInstance();
    }
}