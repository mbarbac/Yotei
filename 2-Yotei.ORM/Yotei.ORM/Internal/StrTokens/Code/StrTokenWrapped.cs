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
    }

    static char Validate(char c) => c >= 32
        ? c
        : throw new ArgumentException("Invalid character.").WithData(c);

    /// <inheritdoc/>
    public override string ToString() => $"{Head}{Payload}{Tail}";

    /// <inheritdoc/>
    public string Head
    {
        get => _Head;
        init => _Head = value.NotNullNotEmpty();
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
        init => _Tail = value.NotNullNotEmpty();
    }
    string _Tail = default!;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken Reduce(StringComparison comparison)
    {
        var item = Payload.Reduce(comparison);

        if (item is IStrTokenWrapped temp &&
            string.Compare(temp.Head, Head, comparison) == 0 &&
            string.Compare(temp.Tail, Tail, comparison) == 0)
            return item;

        return ReferenceEquals(Payload, item)
            ? this
            : new StrTokenWrapped(Head, item, Tail);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken Tokenize(Func<string, IStrToken> tokenizer)
    {
        tokenizer.ThrowWhenNull();

        var item = Payload.Tokenize(tokenizer);
        return ReferenceEquals(Payload, item) ? this : new StrTokenWrapped(Head, item, Tail);
    }
}