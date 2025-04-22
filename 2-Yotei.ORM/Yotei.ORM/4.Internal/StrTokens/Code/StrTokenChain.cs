using Builder = Yotei.ORM.Internal.StrTokenChainBuilder;
using IBuilder = Yotei.ORM.Internal.IStrTokenChainBuilder;

namespace Yotei.ORM.Internal;

// ========================================================
/// <inheritdoc cref="IStrTokenchain"/>
[DebuggerDisplay("{ToDebugString(5)}")]
[InvariantList<IStrToken>]
public partial class StrTokenChain : IStrTokenChain
{
    protected override Builder Items => _Items ??= new();
    Builder? _Items = null;

    /// <inheritdoc/>
    public override Builder GetBuilder() => Items.Clone();
    IBuilder IStrTokenChain.GetBuilder() => GetBuilder();

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public StrTokenChain() : base() { }

    /// <summary>
    /// Initializes a new empty instance with the given initial capacity.
    /// </summary>
    /// <param name="capacity"></param>
    public StrTokenChain(int capacity) : this() => Items.Capacity = capacity;

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    public StrTokenChain(IEnumerable<IStrToken> range) : this() => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected StrTokenChain(StrTokenChain source) : this() => Items.AddRange(source);

    /// <inheritdoc/>
    public override string ToString() => Count == 0
        ? string.Empty
        : string.Concat(Items.Select(x => x.ToString()));

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEnumerable<IStrToken> Payload => Items;
    object? IStrToken.Payload => Payload;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IStrToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null || other is not StrTokenChain valid) return false;

        if (Count != valid.Count) return false;
        for (int i = 0; i < Count; i++) if (!this[i].Equals(valid[i])) return false;
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as StrTokenChain);

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
    public IStrToken Reduce(StringComparison comparison)
    {
        var builder = new Builder();
        var changed = false;

        // Reducing...
        for (int i = 0; i < Count; i++)
        {
            var item = this[i];
            var temp = item.Reduce(comparison);
            if (temp is StrTokenChain range && range.Count == 1) temp = range[0];

            builder.Add(temp);
            if (!item.Equals(temp)) changed = true;
        }

        // Combining...
        var token = changed ? builder.ToInstance() : this;
        return StrTokenizer.ReduceTextTokens(token);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken TokenizeWith(Func<string, IStrToken> tokenizer)
    {
        tokenizer.ThrowWhenNull();

        var builder = new Builder();
        var changed = false;

        for (int i = 0; i < Count; i++)
        {
            var item = this[i];
            var temp = item.TokenizeWith(tokenizer);
            if (temp is StrTokenChain range && range.Count == 1) temp = range[0];

            builder.Add(temp);
            if (!item.Equals(temp)) changed = true;
        }

        return changed ? builder.ToInstance() : this;
    }
}