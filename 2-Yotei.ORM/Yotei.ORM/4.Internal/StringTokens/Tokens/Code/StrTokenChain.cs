namespace Yotei.ORM.Internal.Code;

// ========================================================
/// <inheritdoc cref="IStrTokenChain"/>
[DebuggerDisplay("{ToDebugString(5)}")]
[InvariantList<IStrToken>]
public partial class StrTokenChain : IStrTokenChain
{
    protected override Builder Items => _Items ??= new();
    Builder? _Items = null;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public StrTokenChain() { }

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public StrTokenChain(IEnumerable<IStrToken> range) : this() => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    public StrTokenChain(StrTokenChain source) : this() => Items.AddRange(source);

    /// <inheritdoc/>
    public override string ToString() => Count == 0
        ? string.Empty
        : string.Concat(Items.Select(x => x.ToString()));

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IStrToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null || other is not IStrTokenChain valid) return false;

        if (Count != valid.Count) return false;
        for (int i = 0; i < Count; i++) if (!this[i].Equals(valid[i])) return false;
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IStrToken);

    // Equality operator.
    public static bool operator ==(StrTokenChain? x, IStrToken? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;
        return x.Equals(y);
    }

    // Inequality operator.
    public static bool operator !=(StrTokenChain? x, IStrToken? y) => !(x == y);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, this);
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, this[i]);

        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual Builder GetBuilder() => Items.Clone();
    IStrTokenChain.IBuilder IStrTokenChain.GetBuilder() => GetBuilder();

    /// <inheritdoc/>
    public IEnumerable<IStrToken> Payload => Items;
    object? IStrToken.Payload => Payload;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken Reduce(StringComparison comparison)
    {
        var builder = GetBuilder(); builder.Clear();
        var changed = false;

        // Reducing...
        for (int i = 0; i < Count; i++)
        {
            var item = this[i];
            var temp = item.Reduce(comparison);
            if (temp is IStrTokenChain range && range.Count == 1) temp = range[0];

            builder.Add(temp);
            if (!item.Equals(temp)) changed = true;
        }

        // Combining...
        var token = changed ? builder.ToInstance() : this;
        return token.ReduceTextTokens();
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken ReduceTextTokens()
    {
        var token = this;

        // Only combining if needed to save GC allocations...
        if (token.Count(x => x is IStrTokenText) > 1)
        {
            var builder = GetBuilder(); builder.Clear();
            var changed = false;

            // Combining starts from 1...
            for (int i = 1; i < Count; i++)
            {
            }

            // Recreating if needed...
            if (changed) token = builder.ToInstance();
        }

        // Finishing...
        return
            token.Count == 0 ? StrTokenText.Empty :
            token.Count == 1 ? token[0] :
            token;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken TokenizeWith(Func<string, IStrToken> tokenizer)
    {
        tokenizer.ThrowWhenNull();

        var builder = GetBuilder(); builder.Clear();
        var changed = false;

        for (int i = 0; i < Count; i++)
        {
            var item = this[i];
            var temp = item.TokenizeWith(tokenizer);
            if (temp is IStrTokenChain range && range.Count == 1) temp = range[0];

            builder.Add(temp);
            if (!item.Equals(temp)) changed = true;
        }

        return changed ? builder.ToInstance() : this;
    }
}