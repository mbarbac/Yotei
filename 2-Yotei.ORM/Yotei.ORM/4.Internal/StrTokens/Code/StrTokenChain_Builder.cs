namespace Yotei.ORM.Internal;

partial class StrTokenChain
{
    // ====================================================
    [DebuggerDisplay("{ToDebugString(5)}")]
    [Cloneable]
    public partial class Builder : CoreList<IStrToken>
    {
        public Builder() : base() { }
        public Builder(int capacity) : this() => Capacity = capacity;
        public Builder(IEnumerable<IStrToken> range) : this() => AddRange(range);
        protected Builder(Builder source) : this() => AddRange(source);

        public override IStrToken ValidateItem(IStrToken item) => item.ThrowWhenNull();
        public override IEqualityComparer<IStrToken> Comparer => EqualityComparer<IStrToken>.Default;
        public override bool CanInclude(IStrToken item, IStrToken source) => true;
        public override bool ExpandItems => true;

        /// <summary>
        /// Returns a new instance based upon the captured contents.
        /// </summary>
        /// <returns></returns>
        public virtual IStrTokenChain ToInstance() => new StrTokenChain(this);
    }
}