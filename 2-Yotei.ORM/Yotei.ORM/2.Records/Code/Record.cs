namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IRecord"/>
public sealed class Record : IRecord
{
    object?[] Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public Record() => Items = [];

    /// <summary>
    /// Initializes a new instance with the given value.
    /// </summary>
    /// <param name="value"></param>
    public Record(object? value) => Items = [value];

    /// <summary>
    /// Initializes a new instance with the given range of values.
    /// </summary>
    /// <param name="range"></param>
    public Record(IEnumerable<object?> range) => Items = range.ThrowWhenNull().ToArray();

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    Record(Record source) => Items = source.Items;

    /// <inheritdoc/>
    public IEnumerator<object?> GetEnumerator() => Items.GetTypedEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => Items.Sketch();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public int Count => Items.Length;

    /// <inheritdoc/>
    public object? this[int index] => Items[index];

    // ----------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Validate(int index, bool insert = false)
    {
        if (index < 0) throw new IndexOutOfRangeException("Index is negative.").WithData(index);

        var value = Items.Length + (insert ? 1 : 0);
        if (index >= value) throw new IndexOutOfRangeException("Index greater than or equal the number of elements.").WithData(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Validate(int index, int count, bool insert = false)
    {
        Validate(index, insert);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(count, Items.Length - index);
    }

    /// <inheritdoc/>
    public IRecord GetRange(int index, int count)
    {
        Validate(index, count);

        if (count == 0 && Count == 0) return this;
        if (index == 0 && count == Count) return this;

        var temps = Items.GetRange(index, count);
        return new Record(temps);
    }

    /// <inheritdoc/>
    public IRecord Replace(int index, object? value)
    {
        Validate(index);

        var source = Items[index];
        if (source.EquivalentTo(value)) return this;

        var temps = Items.Duplicate();
        temps[index] = value;
        return new Record(temps);
    }

    /// <inheritdoc/>
    public IRecord Add(object? value)
    {
        var temps = Items.Add(value);
        return new Record(temps);
    }

    /// <inheritdoc/>
    public IRecord AddRange(IEnumerable<object?> range)
    {
        range.ThrowWhenNull();

        if (range is object?[] others && others.Length == 0) return this;

        var temps = Items.ToList();
        var num = 0;
        foreach (var item in range) { temps.Add(item); num++; }

        return num == 0 ? this : new Record(temps);
    }

    /// <inheritdoc/>
    public IRecord Insert(int index, object? value)
    {
        Validate(index, insert: true);

        var temps = Items.Insert(index, value);
        return new Record(temps);
    }

    /// <inheritdoc/>
    public IRecord InsertRange(int index, IEnumerable<object?> range)
    {
        Validate(index, insert: true);
        range.ThrowWhenNull();

        if (range is object?[] others && others.Length == 0) return this;

        var temps = Items.ToList();
        var num = 0;
        foreach (var item in range) { temps.Insert(index, item); num++; index++; }

        return num == 0 ? this : new Record(temps);
    }

    /// <inheritdoc/>
    public IRecord RemoveAt(int index)
    {
        Validate(index);

        var temps = Items.RemoveAt(index);
        return new Record(temps);
    }

    /// <inheritdoc/>
    public IRecord RemoveRange(int index, int count)
    {
        Validate(index, count);

        if (count == 0) return this;
        if (index == 0 && count == Count) return Clear();

        var temps = Items.RemoveRange(index, count);
        return new Record(temps);
    }

    /// <inheritdoc/>
    public IRecord Clear() => Count == 0 ? this : new Record();
}