#pragma warning disable IDE0028

namespace Yotei.ORM.Internal;

// ========================================================
/// <inheritdoc cref="IStrTokenChain"/>
[DebuggerDisplay("{ToDebugString(5)}")]
[Cloneable]
public partial class StrTokenChain : InvariantList<IStrToken>, IStrTokenChain
{
    protected override Builder Items => _Items ??= new Builder();
    Builder? _Items = null;

    public StrTokenChain() : base() { }
    public StrTokenChain(int capacity) : this() => Items.Capacity = capacity;
    public StrTokenChain(IEnumerable<IStrToken> range) : this() => Items.AddRange(range);
    protected StrTokenChain(StrTokenChain source) : this() => Items.AddRange(source.Items);

    public override string ToString() => Count == 0
        ? string.Empty
        : string.Concat(Items.Select(x => x.ToString()));

    public override Builder GetBuilder() => (Builder)base.GetBuilder();

    // ----------------------------------------------------

    public IEnumerable<IStrToken> Payload => Items;
    object? IStrToken.Payload => Payload;

    // ----------------------------------------------------
    public IStrToken Reduce(StringComparison comparison)
    {
        var builder = GetBuilder();
        var changed = false;

        // Reducing elements...
        for (int i = 0; i < builder.Count; i++)
        {
            var item = builder[i];
            var temp = item.Reduce(comparison);

            if (!ReferenceEquals(item, temp))
            {
                builder[i] = temp;
                changed = true;
            }
        }

        // Combining text elements, starting from '1'...
        for (int i = 1; i < builder.Count; i++)
        {
            var prev = builder[i - 1];
            var item = builder[i];

            if (prev is IStrTokenText xprev && item is IStrTokenText xitem)
            {
                builder[i - 1] = new StrTokenText($"{xprev.Payload}{xitem.Payload}");
                builder.RemoveAt(i);
                i--;
                changed = true;
            }
        }

        // Finishing...
        return
            builder.Count == 0 ? StrTokenText.Empty :
            builder.Count == 1 ? builder[0] :
            changed ? builder.ToInstance() :
            this;
    }

    // ----------------------------------------------------

    public IStrToken Tokenize(Func<string, IStrToken> tokenizer)
    {
        tokenizer.ThrowWhenNull();

        var builder = GetBuilder();
        var changed = false;

        for (int i = 0; i < builder.Count; i++)
        {
            var item = builder[i];
            var temp = item.Tokenize(tokenizer);

            if (!ReferenceEquals(item, temp))
            {
                builder[i] = temp;
                changed = true;
            }
        }

        return changed ? builder.ToInstance() : this;
    }

    // ----------------------------------------------------

    public override IStrTokenChain GetRange(int index, int count) => (IStrTokenChain)base.GetRange(index, count);
    public override IStrTokenChain Replace(int index, IStrToken item) => (IStrTokenChain)base.Replace(index, item);
    public override IStrTokenChain Add(IStrToken item) => (IStrTokenChain)base.Add(item);
    public override IStrTokenChain AddRange(IEnumerable<IStrToken> range) => (IStrTokenChain)base.AddRange(range);
    public override IStrTokenChain Insert(int index, IStrToken item) => (IStrTokenChain)base.Insert(index, item);
    public override IStrTokenChain InsertRange(int index, IEnumerable<IStrToken> range) => (IStrTokenChain)base.InsertRange(index, range);
    public override IStrTokenChain RemoveAt(int index) => (IStrTokenChain)base.RemoveAt(index);
    public override IStrTokenChain RemoveRange(int index, int count) => (IStrTokenChain)base.RemoveRange(index, count);
    public override IStrTokenChain Remove(IStrToken item) => (IStrTokenChain)base.Remove(item);
    public override IStrTokenChain RemoveLast(IStrToken item) => (IStrTokenChain)base.RemoveLast(item);
    public override IStrTokenChain RemoveAll(IStrToken item) => (IStrTokenChain)base.RemoveAll(item);
    public override IStrTokenChain Remove(Predicate<IStrToken> predicate) => (IStrTokenChain)base.Remove(predicate);
    public override IStrTokenChain RemoveLast(Predicate<IStrToken> predicate) => (IStrTokenChain)base.RemoveLast(predicate);
    public override IStrTokenChain RemoveAll(Predicate<IStrToken> predicate) => (IStrTokenChain)base.RemoveAll(predicate);
    public override IStrTokenChain Clear() => (IStrTokenChain)base.Clear();
}