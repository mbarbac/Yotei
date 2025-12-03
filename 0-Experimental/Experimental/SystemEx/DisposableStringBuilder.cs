#pragma warning disable CA8597
#pragma warning disable CA1822

namespace Experimental;

// ========================================================
public ref struct DisposableStringBuilder : IDisposable
{
    static readonly int MINLEN = 8;
    char[]? _array;
    int _len;

    public DisposableStringBuilder() { }

    public void Dispose() => Clear();

    public override string ToString() => ToString(true);

    public string ToString(bool dispose)
    {
        var str = _array is null || _len == 0 ? string.Empty : new(_array, 0, _len);
        if (dispose) Dispose();
        return str;
    }

    public readonly Span<char> AsSpan() => (_array ?? []).AsSpan();

    public readonly ReadOnlySpan<char> AsReadOnlySpan() => AsSpan();

    // ----------------------------------------------------

    void Ensure(int size)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 0);
        ArgumentOutOfRangeException.ThrowIfZero(size);

        var len = _array is null ? 0 : _array.Length;
        if (size >= (len - 1)) Increase(size - len);
    }

    void Increase(int add)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(add, 0);

        var size = Math.Max(add + _len, Math.Max(MINLEN, _len * 2));

        var old = _array; _array = ArrayPool<char>.Shared.Rent(size);
        if (old is not null)
        {
            old.CopyTo(_array);
            ArrayPool<char>.Shared.Return(old);
        }
    }

    // ----------------------------------------------------

    public void Clear()
    {
        if (_array is not null) ArrayPool<char>.Shared.Return(_array);
        _array = null;
        _len = 0;
    }

    // ----------------------------------------------------

    public void Append(char value) => Append(value, 1);

    public void AppendLine(char value)
    {
        Append(value);
        Append(Environment.NewLine);
    }

    public void Append(char value, int count)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 0);

        if (count == 0) return;

        Ensure(count);
        for (int i = 0; i < count; i++) _array![_len] = value;
        _len += count;
    }

    public void AppendLine(char value, int count)
    {
        Append(value, count);
        Append(Environment.NewLine);
    }

    public void Insert(int index, char value) => throw null!;

    public void Insert(int index, char value, int count) => throw null!;

    // ----------------------------------------------------

    public void Append(char[] value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length == 0) return;

        Ensure(value.Length);
        value.CopyTo(_array!, _len);
        _len += value.Length;
    }

    public void AppendLine(char[] value)
    {
        Append(value);
        Append(Environment.NewLine);
    }

    public void Insert(int index, char[] value) => throw null!;

    // ----------------------------------------------------

    public void Append(ReadOnlySpan<char> value)
    {
        if (value.IsEmpty) return;

        Ensure(value.Length);
        value.CopyTo(_array);
        _len += value.Length;
    }

    public void AppendLine(ReadOnlySpan<char> value)
    {
        Append(value);
        Append(Environment.NewLine);
    }

    // ----------------------------------------------------

    public void Append(byte value) => throw null!;

    public void AppendLine(byte value) => throw null!;

    // ----------------------------------------------------

    public void Append(bool value) => throw null!;

    public void AppendLine(bool value) => throw null!;

    // ----------------------------------------------------

    public void Append(short value) => throw null!;

    public void AppendLine(short value) => throw null!;

    public void Append(ushort value) => throw null!;

    public void AppendLine(ushort value) => throw null!;

    // ----------------------------------------------------

    public void Append(int value) => throw null!;

    public void AppendLine(int value) => throw null!;

    public void Append(uint value) => throw null!;

    public void AppendLine(uint value) => throw null!;

    // ----------------------------------------------------

    public void Append(long value) => throw null!;

    public void AppendLine(long value) => throw null!;

    public void Append(ulong value) => throw null!;

    public void AppendLine(ulong value) => throw null!;

    // ----------------------------------------------------

    public void Append(float value) => throw null!;

    public void AppendLine(double value) => throw null!;

    public void Append(decimal value) => throw null!;

    // ----------------------------------------------------

    public void Append(DateTime value) => throw null!;

    public void AppendLine(DateTime value) => throw null!;

    public void Append(DateOnly value) => throw null!;

    public void AppendLine(DateOnly value) => throw null!;

    public void Append(TimeOnly value) => throw null!;

    public void AppendLine(TimeOnly value) => throw null!;

    public void Append(TimeSpan value) => throw null!;

    public void AppendLine(TimeSpan value) => throw null!;
}