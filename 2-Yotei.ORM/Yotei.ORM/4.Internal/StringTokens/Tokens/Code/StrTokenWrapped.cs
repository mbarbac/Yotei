namespace Yotei.ORM.Internal.Code;

// ========================================================
/// <inheritdoc cref="IStrTokenWrapped"/>
public class StrTokenWrapped : IStrTokenWrapped
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="head"></param>
    /// <param name="payload"></param>
    /// <param name="tail"></param>
    public StrTokenWrapped(string head, IStrToken payload, string tail)
    {
        Head = head;
        Payload = payload;
        Tail = tail;
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="head"></param>
    /// <param name="payload"></param>
    /// <param name="tail"></param>
    public StrTokenWrapped(char head, IStrToken payload, char tail)
    {
        Head = Validate(head).ToString();
        Payload = payload;
        Tail = Validate(tail).ToString();

        static char Validate(char c) => c >= ' '
            ? c
            : throw new ArgumentException("Invalid character.").WithData(c);
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Head}{Payload}{Tail}";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IStrToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not IStrTokenWrapped valid) return false;

        if (Head != valid.Head) return false;
        if (Tail != valid.Tail) return false;
        if (!Payload.Equals(valid.Payload)) return false;

        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IStrTokenText);

    // Equality operator.
    public static bool operator ==(StrTokenWrapped? x, IStrToken? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;
        return x.Equals(y);
    }

    // Inequality operator.
    public static bool operator !=(StrTokenWrapped? x, IStrToken? y) => !(x == y);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Head);
        code = HashCode.Combine(code, Payload);
        code = HashCode.Combine(code, Tail);
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public string Head
    {
        get => _Head;
        init => _Head = value.NotNullNotEmpty(trim: false);
    }
    string _Head = default!;

    /// <inheritdoc/>
    public IStrToken Payload
    {
        get => _Payload;
        init => _Payload = value.ThrowWhenNull();
    }
    IStrToken _Payload = default!;
    object? IStrToken.Payload => Payload;

    /// <inheritdoc/>
    public string Tail
    {
        get => _Tail;
        init => _Tail = value.NotNullNotEmpty(trim: false);
    }
    string _Tail = default!;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IStrToken Reduce(StringComparison comparison)
    {
        var item = Payload.Reduce(comparison);

        if (item is IStrTokenChain range)
        {
            if (range.Count == 0) item = StrTokenText.Empty;
            if (range.Count == 1) item = range[0];
        }

        if (item is IStrTokenWrapped temp &&
            string.Compare(Head, temp.Head, comparison) == 0 &&
            string.Compare(Tail, temp.Tail, comparison) == 0)
            return temp;

        return Payload.Equals(item)
            ? this
            : new StrTokenWrapped(Head, item, Tail);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IStrToken TokenizeWith(Func<string, IStrToken> tokenizer)
    {
        tokenizer.ThrowWhenNull();

        var item = Payload.TokenizeWith(tokenizer);

        if (item is IStrTokenChain range)
        {
            if (range.Count == 0) item = StrTokenText.Empty;
            if (range.Count == 1) item = range[0];
        }

        return Payload.Equals(item) ? this : new StrTokenWrapped(Head, item, Tail);
    }
}