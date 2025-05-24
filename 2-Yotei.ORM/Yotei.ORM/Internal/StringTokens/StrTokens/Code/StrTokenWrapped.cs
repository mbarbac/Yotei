namespace Yotei.ORM.Internal;

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

        static char Validate(char c) => c >= 32
            ? c
            : throw new ArgumentException("Invalid character.").WithData(c);
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Head}{Payload}{Tail}";

    /// <inheritdoc/>
    public string Head
    {
        get => _Head;
        init => _Head = value.NotNullNotEmpty(trim: true);
    }
    string _Head = default!;

    /// <inheritdoc/>
    public IStrToken Payload
    {
        get => _Payload;
        init => _Payload = value.ThrowWhenNull();
    }
    IStrToken _Payload = default!;
    object? IStrToken.Payload => _Payload;

    /// <inheritdoc/>
    public string Tail
    {
        get => _Tail;
        init => _Tail = value.NotNullNotEmpty(trim: true);
    }
    string _Tail = default!;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IStrToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null || other is not IStrTokenWrapped valid) return false;

        if (Head != valid.Head) return false;
        if (Tail != valid.Tail) return false;
        if (!Payload.Equals(valid.Payload)) return false;

        return true;
    }

    /// <summary>
    /// Indicates whether this instance is equal to the given one, using the given comparison.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public virtual bool Equals(IStrToken? other, StringComparison comparison)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null || other is not IStrTokenWrapped valid) return false;

        if (string.Compare(Head, valid.Head, comparison) != 0) return false;
        if (string.Compare(Tail, valid.Tail, comparison) != 0) return false;
        if (!Payload.Equals(valid.Payload)) return false;

        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IStrToken);

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
        code = HashCode.Combine(code, Tail);
        code = HashCode.Combine(code, Payload.GetHashCode());

        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken Reduce(StringComparison comparison)
    {
        var item = Payload.Reduce(comparison);

        if (item is StrTokenChain range)
        {
            if (range.Count == 0) item = StrTokenText.Empty;
            if (range.Count == 1) item = range[0];
        }

        if (item is StrTokenWrapped temp &&
            string.Compare(Head, temp.Head, comparison) == 0 &&
            string.Compare(Tail, temp.Tail, comparison) == 0)
            return item;

        return Payload.Equals(item)
            ? this
            : new StrTokenWrapped(Head, item, Tail);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken TokenizeWith(Func<string, IStrToken> tokenizer)
    {
        tokenizer.ThrowWhenNull();

        var item = Payload.TokenizeWith(tokenizer);

        if (item is StrTokenChain range)
        {
            if (range.Count == 0) item = StrTokenText.Empty;
            if (range.Count == 1) item = range[0];
        }

        return Payload.Equals(item) ? this : new StrTokenWrapped(Head, item, Tail);
    }
}