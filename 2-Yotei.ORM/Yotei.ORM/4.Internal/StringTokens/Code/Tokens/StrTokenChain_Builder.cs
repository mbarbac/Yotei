namespace Yotei.ORM.Internal;

// ========================================================
public partial class StrTokenChain
{
    [DebuggerDisplay("{ToDebugString(5)}")]
    [Cloneable]
    public partial class Builder : CoreList<IStrToken>
    {
        public Builder() : base() { }
        public Builder(int capacity) : this() => Capacity = capacity;
        public Builder(IEnumerable<IStrToken> range) : this() => AddRange(range);
        protected Builder(Builder source) : this() => AddRange(source);

        public override IStrToken ValidateItem(IStrToken item) => item.ThrowWhenNull();
        public override bool ExpandItems => true;
        public override bool IncludeDuplicated(IStrToken item, IStrToken source) => true;
        public override IEqualityComparer<IStrToken> Comparer => EqualityComparer<IStrToken>.Default;

        public virtual StrTokenChain ToInstance() => new(this);
    }
}