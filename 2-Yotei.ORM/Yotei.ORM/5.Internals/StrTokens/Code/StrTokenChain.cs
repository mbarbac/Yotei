namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a string token composed by a flat collection of arbitrary string tokens.
/// <para>Instances of this class are intended to be immutable ones.</para>
/// </summary>
[DebuggerDisplay("{ToDebugString(5)}")]
[InvariantList<IStrToken>]
public partial class StrTokenChain : IStrToken
{
    protected override Builder Items { get; }
    protected virtual Builder OnInitialize() => new();

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public StrTokenChain() => Items = OnInitialize();

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    public StrTokenChain(IEnumerable<IStrToken> range) : this() => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected StrTokenChain(StrTokenChain source) : this() => Items = source.Items.Clone();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <inheritdoc cref="IStrToken.Payload"/>
    public IEnumerable<IStrToken> Payload => Items;
    object? IStrToken.Payload => Payload;

    /// <summary>
    /// Returns a new builder using the contents captured by this collection.
    /// </summary>
    /// <returns></returns>
    public virtual Builder CreateBuilder() => Items.Clone();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IStrToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not StrTokenChain valid) return false;

        for (int i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            var equal = item.Equals(valid[i]);
            if (!equal) return false;
        }
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
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reduces this instance to a simpler from, if possible, by combining adjacent text elements
    /// in this collection.
    /// </summary>
    /// <returns></returns>
    public virtual IStrToken ReduceText()
    {
        var token = this;

        // Reducing only when needed to save GC allocations...
        if (token.Count(x => x is StrTokenText) > 1)
        {
            var builder = CreateBuilder(); // Populated builder needed...
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
    public IStrToken Reduce(StringComparison comparison)
    {
        var builder = CreateBuilder(); builder.Clear(); // Empty builder needed...
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
    public IStrToken TokenizeWith(Func<string, IStrToken> tokenizer)
    {
        tokenizer.ThrowWhenNull();

        var builder = CreateBuilder(); builder.Clear(); // Empty builder needed...
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