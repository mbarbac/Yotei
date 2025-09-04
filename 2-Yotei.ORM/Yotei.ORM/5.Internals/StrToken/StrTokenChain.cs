namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents an immutable list-alike collection of elements identified by their keys.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[DebuggerDisplay("{Items.ToDebugString(5)}")]
[InvariantList<IStrToken>]
public partial class StrTokenChain : IStrToken
{
    protected override Builder Items { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public StrTokenChain() => Items = new();

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
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

    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    public Builder CreateBuilder() => Items.Clone();

    /// <inheritdoc cref="IStrToken.Payload"/>
    public IEnumerable<IStrToken> Payload => Items;
    object? IStrToken.Payload => Payload;

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
    public override bool Equals(object? obj) => Equals(obj as StrTokenText);

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
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reduces this instance to a simpler form by combining adjacent elements that strictly are
    /// text ones, if such is possible.
    /// </summary>
    /// <returns></returns>
    public IStrToken ReduceTextElements()
    {
        var token = this;

        // Reducing, but only when it might be needed...
        if (token.Count(x => x is StrTokenText) > 1)
        {
            var builder = CreateBuilder(); // Populated!
            var changed = false;

            // Combining starts at 1...
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

        // Finishing...
        return
            token.Count == 0 ? StrTokenText.Empty :
            token.Count == 1 ? token[0] :
            token;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken Reduce(StringComparison comparison)
    {
        var token = this;

        // Reducing but only if needed...
        if (Count > 0)
        {
            var builder = new Builder(); // Empty!
            var changed = false;

            // Iterating...
            for (int i = 0; i < Count; i++)
            {
                var item = Items[i];
                var temp = item.Reduce(comparison);
                if (temp is StrTokenChain range && range.Count == 1) temp = range[0];

                builder.Add(temp);
                if (!item.Equals(temp)) changed = true;
            }

            // Recreating if needed...
            if (changed)
            {
                switch (builder.Count)
                {
                    case 0: return StrTokenText.Empty;
                    case 1: return builder[0];
                    default: token = builder.CreateInstance(); break;
                }
            }
        }

        // Finishing by combining text elements...
        return token.ReduceTextElements();
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken TokenizeWith(Func<string, IStrToken> tokenizer)
    {
        tokenizer.ThrowWhenNull();
        var token = this;

        // Only if needed...
        if (Count > 0)
        {
            var builder = new Builder(); // Empty...
            var changed = false;

            // Iterating...
            for (int i = 0; i < Count; i++)
            {
                var item = Items[i];
                var temp = item.TokenizeWith(tokenizer);
                if (temp is StrTokenChain range && range.Count == 1) temp = range[0];

                builder.Add(temp);
                if (!item.Equals(temp)) changed = true;
            }

            // Recreating if needed...
            if (changed)
            {
                switch (builder.Count)
                {
                    case 0: return StrTokenText.Empty;
                    case 1: return builder[0];
                    default: token = builder.CreateInstance(); break;
                }
            }
        }

        // Finishing...
        return token;
    }
}