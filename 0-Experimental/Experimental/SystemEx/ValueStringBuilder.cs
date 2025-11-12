namespace Experimental;

// https://steven-giesel.com/blogPost/4cada9a7-c462-4133-ad7f-e8b671987896

// ========================================================
public ref struct ValueStringBuilder
{
    Span<char> _buffer;

    public override string ToString() => throw new NotImplementedException();

    public Span<char> AsSpan() => _buffer;

    public void Append(ReadOnlySpan<char> value) => throw new NotImplementedException();

    public void AppendLine(ReadOnlySpan<char> value)
    {
        ref var valueRef = ref MemoryMarshal.GetReference(value);
        ref var bufferRef = ref MemoryMarshal.GetReference(_buffer);

        Unsafe.CopyBlock(
            ref Unsafe.As<char, byte>(ref bufferRef),
            ref Unsafe.As<char, byte>(ref valueRef),
            (uint)(value.Length * sizeof(char)));

        StringBuilder
    }
}