namespace Yotei.ORM.Internal;

// ========================================================
/// <inheritdoc cref="IStrTokenChain"/>
[FrozenList<IStrToken>]
public partial class StrTokenChain : IStrTokenChain
{
    /// <summary>
    /// Represents a builder of <see cref="IStrTokenChain"/> instances.
    /// </summary>
    [Cloneable]
    public partial class Builder : CoreList<IStrToken>
    {
        /// <inheritdoc/>
        public Builder() : base()
        {
            ValidateItem = (x) => x.ThrowWhenNull();
            GetDuplicates = (x) => [];
            ExpandItems = true;
        }

        /// <inheritdoc/>
        public Builder(IStrToken item) : this() => Add(item);

        /// <inheritdoc/>
        public Builder(IEnumerable<IStrToken> range) : this() => AddRange(range);

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) : this() => AddRange(source);

        /// <summary>
        /// Returns a new instance based upon the captured contents.
        /// </summary>
        /// <returns></returns>
        public virtual IStrTokenChain ToInstance() => new StrTokenChain(this);
    }

    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    public virtual Builder ToBuilder() => Items.Clone();

    // ====================================================

    protected override Builder Items { get; }

    /// <inheritdoc/>
    public StrTokenChain() : base() => Items = [];

    /// <inheritdoc/>
    public StrTokenChain(IStrToken item) : this() => Items.Add(item);

    /// <inheritdoc/>
    public StrTokenChain(IEnumerable<IStrToken> range) : this() => Items.AddRange(range);

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="source"></param>
    protected StrTokenChain(StrTokenChain source) : this() => Items.AddRange(source);

    /// <inheritdoc/>
    public override string ToString()
    {
        return Count == 0
            ? string.Empty
            : string.Concat(Items.Select(x => x.ToString()));
    }

    /// <inheritdoc/>
    public IEnumerable<IStrToken> Payload => this;
    object? IStrToken.Payload => Payload;

    /// <inheritdoc/>
    public IStrToken Reduce(StringComparison comparison)
    {
        var builder = ToBuilder();
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
}