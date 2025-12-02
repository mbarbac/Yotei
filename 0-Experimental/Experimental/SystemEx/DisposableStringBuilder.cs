//#pragma warning disable IDE0079
//#pragma warning disable CA1822
//#pragma warning disable CA1861
//#pragma warning disable IDE0060
//#pragma warning disable CS8602
//#pragma warning disable IDE0251

using System.Buffers;

namespace Experimental;

// ========================================================
public class Example
{
    public static void M1() { using var builder = M3(); M2(builder); }
    public static void M2(DisposableStringBuilder builder) => builder.AppendLine("Hello");
    public static DisposableStringBuilder M3() => new();
    public static void M4()
    {
        using var builder = M3();
        var span = items.AsMemory();
        builder.Append(span.Span);
    }
    static readonly char[] items = ['a', 'b'];
}

// ========================================================
public ref struct DisposableStringBuilder : IDisposable
{
    static readonly int MINLEN = 8;
    char[]? _array;
    int _len;

    public DisposableStringBuilder() { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (_array is not null) ArrayPool<char>.Shared.Return(_array);
        _array = null;
        _len = 0;
    }

    public override string ToString()
    {
        var str = _array is null || _len == 0 ? string.Empty : new(_array, 0, _len);
        Dispose();
        return str;
    }

    public readonly Span<char> AsSpan() => (_array ?? []).AsSpan();

    public readonly ReadOnlySpan<char> AsReadOnlySpan() => AsSpan();

    // ----------------------------------------------------

    int Max(int x, int y) => x < y ? y : x;

    void Increase(int value)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(value, 0);
        ArgumentOutOfRangeException.ThrowIfZero(value);

        if (_array is null) _array = ArrayPool<char>.Shared.Rent(Max(value, MINLEN));
        else
        {
            var size = _array.Length;
            var sum = _len + value;
            
            if (size > sum) return;
            while (size < sum) size *= 2;

            var old = _array; _array = ArrayPool<char>.Shared.Rent(size);
            old.CopyTo(_array, 0);
            ArrayPool<char>.Shared.Return(old);
        }
    }

    public DisposableStringBuilder Clear()
    {
        if (_array is not null) Array.Clear(_array);
        _len = 0;
        return this;
    }

    // ----------------------------------------------------

    public DisposableStringBuilder Append(char value) => Append(value, 1);

    public DisposableStringBuilder AppendLine(char value)
    {
        Append(value);
        return Append(Environment.NewLine);
    }

    public DisposableStringBuilder Append(char value, int count)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 0);

        if (count > 0)
        {
            Increase(count);
            for (int i = 0; i < count; i++) _array![_len] = value;
            _len += count;
        }
        return this;
    }

    public DisposableStringBuilder AppendLine(char value, int count)
    {
        Append(value, count);
        return Append(Environment.NewLine);
    }

    // ----------------------------------------------------

    public DisposableStringBuilder Append(char[] value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length > 0)
        {
            Increase(value.Length);
            Array.Copy(value, 0, _array!, _len, value.Length);
            _len += value.Length;
        }
        return this;
    }

    public DisposableStringBuilder AppendLine(char[] value)
    {
        Append(value);
        return Append(Environment.NewLine);
    }

    public DisposableStringBuilder Append(char[] value, int index, int count)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length > 0)
        {
            Increase(value.Length);
            Array.Copy(value, index, _array!, _len, count);
            _len += value.Length;
        }
        return this;
    }

    public DisposableStringBuilder AppendLine(char[] value, int index, int count)
    {
        Append(value, index, count);
        return Append(Environment.NewLine);
    }

    // ----------------------------------------------------

    public DisposableStringBuilder Append(ReadOnlySpan<char> value)
    {
        if (value.Length > 0)
        {
            Increase(value.Length);
            value.CopyTo(_array);
            _len += value.Length;
        }
        return this;
    }

    public DisposableStringBuilder AppendLine(ReadOnlySpan<char> value)
    {
        Append(value);
        return Append(Environment.NewLine);
    }

    // ----------------------------------------------------

    public DisposableStringBuilder Append(
        string value) => value is null ? this : Append(value.AsSpan());

    public DisposableStringBuilder AppendLine(string value)
    {
        Append(value);
        return Append(Environment.NewLine);
    }

    // ----------------------------------------------------

    //public DisposableStringBuilder Append(StringBuilder value) => throw null;

    //public DisposableStringBuilder AppendLine(StringBuilder value) => throw null;

    // ----------------------------------------------------

    //public DisposableStringBuilder Append(byte value) => throw null;

    //public DisposableStringBuilder AppendLine(byte value) => throw null;

    // ----------------------------------------------------

    //public DisposableStringBuilder Append(bool value) => throw null;

    //public DisposableStringBuilder AppendLine(bool value) => throw null;

    // ----------------------------------------------------

    //public DisposableStringBuilder Append(short value) => throw null;

    //public DisposableStringBuilder AppendLine(short value) => throw null;

    //public DisposableStringBuilder Append(ushort value) => throw null;

    //public DisposableStringBuilder AppendLine(ushort value) => throw null;

    // ----------------------------------------------------

    //public DisposableStringBuilder Append(int value) => throw null;

    //public DisposableStringBuilder AppendLine(int value) => throw null;

    //public DisposableStringBuilder Append(uint value) => throw null;

    //public DisposableStringBuilder AppendLine(uint value) => throw null;

    // ----------------------------------------------------

    //public DisposableStringBuilder Append(long value) => throw null;

    //public DisposableStringBuilder AppendLine(long value) => throw null;

    //public DisposableStringBuilder Append(ulong value) => throw null;

    //public DisposableStringBuilder AppendLine(ulong value) => throw null;

    // ----------------------------------------------------

    //public DisposableStringBuilder Append(float value) => throw null;

    //public DisposableStringBuilder AppendLine(double value) => throw null;

    //public DisposableStringBuilder Append(decimal value) => throw null;
}