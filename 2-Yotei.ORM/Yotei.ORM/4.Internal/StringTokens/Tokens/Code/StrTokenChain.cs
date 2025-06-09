using IHost = Yotei.ORM.Internal.IStrTokenChain;
using THost = Yotei.ORM.Internal.Code.StrTokenChain;
using IItem = Yotei.ORM.Internal.IStrToken;

namespace Yotei.ORM.Internal.Code;

// ========================================================
/// <inheritdoc cref="IHost"/>
[DebuggerDisplay("{ToDebugString(5)}")]
[InvariantList<IItem>]
public partial class StrTokenChain : IHost
{
    /// <inheritdoc/>
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
    public StrTokenChain(IEnumerable<IItem> range) : this() => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected StrTokenChain(THost source) : this() => Items.AddRange(source);

    /// <inheritdoc/>
    public override string ToString() => Count == 0
        ? string.Empty
        : string.Concat(Items.Select(x => x.ToString()));

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IItem? other) // Special case, we need to use 'IItem'...
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not IHost valid) return false;

        for (int i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            var equal = item.Equals(valid[i]);
            if (!equal) return false;
        }
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IHost);

    public static bool operator ==(THost? host, IHost? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(THost? host, IHost? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc cref="IHost.GetBuilder"/>
    public virtual Builder GetBuilder() => new(this);
    IHost.IBuilder IHost.GetBuilder() => GetBuilder();

    /// <inheritdoc/>
    public IEnumerable<IItem> Payload => Items;

    object? IItem.Payload { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IStrToken Reduce(StringComparison comparison)
    {
        var builder = GetBuilder(); builder.Clear(); // Fresh empty builder...
        var changed = false;

        // Recursively reducing...
        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var temp = item.Reduce(comparison);
            if (temp is IStrTokenChain range && range.Count == 1) temp = range[0];

            builder.Add(temp);
            if (!item.Equals(temp)) changed = true;
        }

        // Finalizing...
        var token = changed ? builder.ToInstance() : this;
        return token.ReduceText();
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IStrToken ReduceText()
    {
        var token = this;

        // Reducing only when needed to save GC allocations...
        if (token.Count(x => x is IStrTokenText) > 1)
        {
            var builder = GetBuilder(); // Temporal but populated builder...
            var changed = false;

            // Combing starts from 1...
            for (int i = 1; i < builder.Count; i++)
            {
                if (builder[i - 1] is IStrTokenText prev && builder[i] is IStrTokenText item)
                {
                    var temp = new StrTokenText($"{prev.Payload}{item.Payload}");

                    builder.Replace(i - 1, temp);
                    builder.RemoveAt(i);
                    changed = true;
                    i--;
                }
            }

            // Recreating if needed...
            if (changed) token = builder.ToInstance();
        }

        // Finishing reducing the resulting chain...
        return
            token.Count == 0 ? StrTokenText.Empty :
            token.Count == 1 ? token[0] :
            token;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IStrToken TokenizeWith(Func<string, IStrToken> tokenizer)
    {
        tokenizer.ThrowWhenNull();

        var builder = GetBuilder(); builder.Clear(); // Fresh empty builder...
        var changed = false;

        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var temp = item.TokenizeWith(tokenizer);
            if (temp is IStrTokenChain range && range.Count == 1) temp = range[0];

            builder.Add(temp);
            if (!item.Equals(temp)) changed = true;
        }

        return changed ? builder.ToInstance() : this;
    }
}
