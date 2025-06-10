using IHost = Yotei.ORM.Internals.IStrTokenChain;
using IItem = Yotei.ORM.Internals.IStrToken;

namespace Yotei.ORM.Internals;

// ========================================================
/// <inheritdoc cref="IHost"/>
[DebuggerDisplay("{ToDebugString(5)}")]
[InvariantList<IItem>]
public partial class StrTokenChain : IHost
{
    /// <inheritdoc/>
    protected override Builder Items { get; }

    /// <summary>
    /// Invoked to create the initial repository of contents of this instance.
    /// </summary>
    protected virtual Builder OnInitialize() => new();

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public StrTokenChain() => Items = OnInitialize();

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    public StrTokenChain(IEnumerable<IItem> range) : this() => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected StrTokenChain(StrTokenChain source) => Items = source.Items.Clone();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IItem? other) // Using 'IItem' instead of 'IHost'...
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

    public static bool operator ==(StrTokenChain? host, IHost? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(StrTokenChain? host, IHost? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc cref="IHost.CreateBuilder"/>
    public virtual Builder CreateBuilder() => Items.Clone();
    IHost.IBuilder IHost.CreateBuilder() => CreateBuilder();

    /// <inheritdoc/>
    public IEnumerable<IItem> Payload => Items;
    object? IItem.Payload => Payload;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IItem Reduce(StringComparison comparison)
    {
        var builder = CreateBuilder(); builder.Clear(); // Empty builder needed...
        var changed = false;

        // Recursively reducing...
        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var temp = item.Reduce(comparison);
            if (temp is IHost range && range.Count == 1) temp = range[0];

            builder.Add(temp);
            if (!item.Equals(temp)) changed = true;
        }

        var token = changed ? builder.CreateInstance() : this;
        return token.ReduceText();
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IItem ReduceText()
    {
        var token = this;

        // Reducing only when needed to save GC allocations...
        if (token.Count(x => x is IStrTokenText) > 1)
        {
            var builder = CreateBuilder(); // Populated builder needed...
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
    public virtual IItem TokenizeWith(Func<string, IItem> tokenizer)
    {
        tokenizer.ThrowWhenNull();

        var builder = CreateBuilder(); builder.Clear(); // Empty builder needed...
        var changed = false;

        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var temp = item.TokenizeWith(tokenizer);
            if (temp is IHost range && range.Count == 1) temp = range[0];

            builder.Add(temp);
            if (!item.Equals(temp)) changed = true;
        }

        return changed ? builder.CreateInstance() : this;
    }
}