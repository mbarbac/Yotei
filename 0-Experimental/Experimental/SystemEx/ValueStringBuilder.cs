#pragma warning disable CS8597

using System.Buffers;

namespace Experimental;

// https://steven-giesel.com/blogPost/4cada9a7-c462-4133-ad7f-e8b671987896

// ========================================================
/// <summary>
/// Represents a disposable struct-based builder of <see cref="String"/> instances.
/// </summary>
public struct ValueStringBuilder : IDisposable
{
    static readonly int MINLEN = 16;
    char[]? _array;
    int _len;
    bool _disposed;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public ValueStringBuilder() { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Dispose()
    {
        if (_array is not null) ArrayPool<char>.Shared.Return(_array);
        _array = null;
        _disposed = true;
    }

    // Invoked to throw an exception if this instance is disposed.
    readonly void ThrowIfDisposed()
    { if (_disposed) throw new ObjectDisposedException("This instance is disposed."); }

    /// <summary>
    /// Obtains the string representation of this instance.
    /// </summary>
    /// <returns></returns>
    public readonly override string ToString()
        => _array is null || _len == 0 ? string.Empty : new(_array, 0, _len);

    /// <summary>
    /// The length of this instance.
    /// </summary>
    public readonly int Length => _len;

    /// <summary>
    /// The maximun number of characters this instance can hold without resizing the internal
    /// members.
    /// </summary>
    public readonly int Capacity => _array is null ? 0 : _array.Length;

    /// <summary>
    /// Returns a <see cref="Span{T}"/> over the contents of this instance.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    public readonly Span<char> AsSpan()
    {
        ThrowIfDisposed();
        return (_array ?? []).AsSpan(0, _len);
    }

    /// <summary>
    /// Returns a <see cref="ReadOnlySpan{T}"/> over the contents of this instance.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    public readonly ReadOnlySpan<char> AsReadOnlySpan() => AsSpan();

    /// <summary>
    /// Returns a <see cref="Memory{T}"/> over the contents of this instance.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    public readonly Memory<char> AsMemory()
    {
        ThrowIfDisposed();
        return (_array ?? []).AsMemory(0, _len);
    }

    /// <summary>
    /// Returns a <see cref="ReadOnlyMemory{T}"/> over the contents of this instance.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    public readonly ReadOnlyMemory<char> AsReadOnlyMemory() => AsMemory();

    // ----------------------------------------------------

    /// <summary> Appends the given value to this instance. </summary>
    /// <param name="value"></param>
    /// <exception cref="ObjectDisposedException"></exception>
    public void Append(char value) => Append(value, 1);

    /// <summary> Appends the given number of copies of the value to this instance. </summary>
    /// <param name="value"></param>
    /// <param name="count"></param>
    /// <exception cref="ObjectDisposedException"></exception>
    public void Append(char value, int count)
    {
        ThrowIfDisposed();
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "Count is negative.").WithData(count);
        if (count == 0) return;

        Ensure(Capacity + count);
        for (int i = 0; i < count; i++) { _array![_len] = value; _len++; }
    }

    public void Append(char[] value) => throw null;
    public void Append(char[] value, int index, int count) => throw null;

    public void Append(object value) => throw null;

    public void Append(bool value) => throw null;

    public void Append(byte value) => throw null;
    public void Append(sbyte value) => throw null;

    public void Append(short value) => throw null;
    public void Append(ushort value) => throw null;

    public void Append(int value) => throw null;
    public void Append(uint value) => throw null;

    public void Append(long value) => throw null;
    public void Append(ulong value) => throw null;

    public void Append(float value) => throw null;
    public void Append(double value) => throw null;
    public void Append(decimal value) => throw null;

    public void Append(StringBuilder value) => throw null;
    public void Append(string value) => throw null;
    public void Append(ReadOnlySpan<char> value) => throw null;

    // ----------------------------------------------------

    public void AppendLine(char value) => throw null;
    public void AppendLine(char value, int count) => throw null;

    public void AppendLine(char[] value) => throw null;
    public void AppendLine(char[] value, int index, int count) => throw null;

    public void AppendLine(object value) => throw null;

    public void AppendLine(bool value) => throw null;

    public void AppendLine(byte value) => throw null;
    public void AppendLine(sbyte value) => throw null;

    public void AppendLine(short value) => throw null;
    public void AppendLine(ushort value) => throw null;

    public void AppendLine(int value) => throw null;
    public void AppendLine(uint value) => throw null;

    public void AppendLine(long value) => throw null;
    public void AppendLine(ulong value) => throw null;

    public void AppendLine(float value) => throw null;
    public void AppendLine(double value) => throw null;
    public void AppendLine(decimal value) => throw null;

    public void AppendLine(StringBuilder value) => throw null;
    public void AppendLine(string value) => throw null;
    public void AppendLine(ReadOnlySpan<char> value) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Clears this instance.
    /// </summary>
    /// We don't need to care if the instance is disposed or not.
    public void Clear()
    {
        if (_array is not null) Array.Clear(_array);
        _len = 0;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Ensures that the internal array has at least the given capacity.
    /// </summary>
    /// <param name="capacity"></param>
    void Ensure(int capacity)
    {
        if (capacity < MINLEN) capacity = MINLEN; else capacity = Capacity * 2;
        if (capacity < Capacity) return;

        var old = _array; _array = ArrayPool<char>.Shared.Rent(capacity);
        if (old != null)
        {
            old.CopyTo(_array, 0);
            ArrayPool<char>.Shared.Return(old);
        }
    }

    /******************************************************/
    static void XX()
    {
        var sb = new ValueStringBuilder();
        sb.Append('1');
    }
}