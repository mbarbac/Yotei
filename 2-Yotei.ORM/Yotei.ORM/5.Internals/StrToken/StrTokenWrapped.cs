namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a string token that carries an arbitrary payload between its head and tail
/// sequences, which must not be null or empty. Head and tail sequences might be spaces-only
/// ones.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class StrTokenWrapped : IStrToken
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

    /// <inheritdoc cref="IStrToken.Payload"/>
    public IStrToken Payload
    {
        get => _Payload;
        init => _Payload = value.ThrowWhenNull();
    }
    IStrToken _Payload = default!;
    object? IStrToken.Payload => Payload;

    /// <summary>
    /// The head sequence.
    /// </summary>
    public string Head
    {
        get => _Head;
        init => _Head = value.NotNullNotEmpty(trim: false);
    }
    string _Head = default!;

    /// <summary>
    /// The tail sequence.
    /// </summary>
    public string Tail
    {
        get => _Tail;
        init => _Tail = value.NotNullNotEmpty(trim: false);
    }
    string _Tail = default!;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance shall be considered equal to the other given one, using the
    /// given string comparison mode.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public virtual bool Equals(IStrToken? other, StringComparison comparison)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not StrTokenWrapped valid) return false;

        if (string.Compare(Head, valid.Head, comparison) != 0) return false;
        if (string.Compare(Tail, valid.Tail, comparison) != 0) return false;
        if (!Payload.Equals(valid.Payload)) return false;

        return true;
    }

    /// <inheritdoc/>
    public virtual bool Equals(IStrToken? other) => Equals(other, StringComparison.CurrentCulture);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as StrTokenWrapped);

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
    public IStrToken Reduce(StringComparison comparison)
    {
        var item = Payload.Reduce(comparison);

        if (item is StrTokenWrapped temp &&
            string.Compare(Head, temp.Head, comparison) == 0 &&
            string.Compare(Tail, temp.Tail, comparison) == 0)
            return temp;

        return Payload.Equals(item) ? this : new StrTokenWrapped(Head, item, Tail);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken TokenizeWith(Func<string, IStrToken> tokenizer)
    {
        tokenizer.ThrowWhenNull();

        var item = Payload.TokenizeWith(tokenizer);
        return Payload.Equals(item) ? this : new StrTokenWrapped(Head, item, Tail);
    }
}