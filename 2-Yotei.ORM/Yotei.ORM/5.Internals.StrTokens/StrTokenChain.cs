namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a string token that is itself a flat collection of arbitrary tokens.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString(5)}")]
[InvariantList<IStrToken>]
public partial class StrTokenChain : IStrToken
{
    protected override Builder Items { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public StrTokenChain() => Items = new Builder();

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
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IStrToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not StrTokenChain valid) return false;

        if (Count != valid.Count) return false;
        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var temp = valid[i];
            var same = item.Equals(temp);
            if (!same) return false;
        }
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IStrToken);

    public static bool operator ==(StrTokenChain? host, StrTokenChain? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(StrTokenChain? host, StrTokenChain? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual Builder CreateBuilder() => Items.Clone();

    /// <inheritdoc/>
    public IEnumerable<IStrToken> Payload => Items;
    object? IStrToken.Payload => Payload;

    // ----------------------------------------------------

    /// <summary>
    /// Reduces this instance to a simpler form, if possible, by combining adjacent elements that
    /// are text ones, and then reducing the resulting chain.
    /// </summary>
    /// <returns></returns>
    public virtual IStrToken ReduceText()
    {
        var token = this;

        // Reducing only when needed to save GC allocations...
        if (token.Count(x => x is StrTokenText) > 1)
        {
            var builder = CreateBuilder(); // Populated builder...
            var changed = false;

            // Combing starts from 1...
            for (int i = 1; i < builder.Count; i++)
            {
                if (builder[i - 1] is StrTokenText prev && builder[i] is StrTokenText item)
                {
                    var temp = new StrTokenText($"{prev.Payload}{item.Payload}");

                    builder.Replace(i - 1, temp);
                    builder.RemoveAt(i);
                    changed = true;
                    i--;
                }
            }

            // Recreating if needed...
            if (changed) token = builder.CreateInstance();
        }

        // Finishing by reducing the resulting chain...
        return
            token.Count == 0 ? StrTokenText.Empty :
            token.Count == 1 ? token[0] :
            token;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IStrToken Reduce(StringComparison comparison)
    {
        var builder = CreateBuilder(); builder.Clear(); // Empty builder...
        var changed = false;

        // Recursively reducing...
        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var temp = item.Reduce(comparison);
            if (temp is StrTokenChain range && range.Count == 1) temp = range[0];

            builder.Add(temp);
            if (!item.Equals(temp)) changed = true;
        }

        var token = changed ? builder.CreateInstance() : this;
        return token.ReduceText();
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IStrToken TokenizeWith(Func<string, IStrToken> tokenizer)
    {
        tokenizer.ThrowWhenNull();

        var builder = CreateBuilder(); builder.Clear(); // Empty builder...
        var changed = false;

        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var temp = item.TokenizeWith(tokenizer);
            if (temp is StrTokenChain range && range.Count == 1) temp = range[0];

            builder.Add(temp);
            if (!item.Equals(temp)) changed = true;
        }

        return changed ? builder.CreateInstance() : this;
    }
}