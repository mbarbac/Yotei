
namespace Yotei.ORM.Internals;

partial class StrTokenChain
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> instances.
    /// </summary>
    [DebuggerDisplay("{ToDebugString(5)}")]
    public class Builder : CoreList<IStrToken>
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        public Builder() : base() { }

        /// <summary>
        /// Initializes a new instance with the given initial capacity.
        /// </summary>
        /// <param name="capacity"></param>
        public Builder(int capacity) : this() => Capacity = capacity;

        /// <summary>
        /// Initializes a new instance with the elements from the given range.
        /// </summary>
        /// <param name="range"></param>
        public Builder(IEnumerable<IStrToken> range) : this() => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) : this() => AddRange(source);

        /// <inheritdoc/>
        public override string ToString() => Count == 0
            ? string.Empty
            : string.Concat(this.Select(x => x.ToString()));

        /// <inheritdoc/>
        public override Builder Clone() => new Builder(this);

        // ------------------------------------------------

        /// <inheritdoc/>
        public override IStrToken ValidateItem(IStrToken item) => item.ThrowWhenNull();

        /// <inheritdoc/>
        public override bool ExpandItems => true;

        /// <inheritdoc/>
        public override bool IsValidDuplicate(IStrToken source, IStrToken item) => true;

        /// <inheritdoc/>
        public override IEqualityComparer<IStrToken> Comparer => _Comparer ??= new();
        MyComparer? _Comparer;
        readonly struct MyComparer() : IEqualityComparer<IStrToken>
        {
            public bool Equals(IStrToken? x, IStrToken? y)
            {
                if (x is null && y is null) return true;
                if (x is null || y is null) return false;

                return x?.Equals(y) ?? false;
            }

            public int GetHashCode(IStrToken obj) => throw new NotImplementedException();
        }

        /// <inheritdoc/>
        /// Since we accept all duplicates there is not need to compute it.
        protected override List<int> FindDuplicates(IStrToken item) => [];

        // ------------------------------------------------

        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        public virtual StrTokenChain CreateInstance() => new(this);

        // ------------------------------------------------

        /// <summary>
        /// Returns a new instance where adjacent text elements have been combined, or otherwise
        /// this instance is no changes were made.
        /// </summary>
        /// <returns></returns>
        public Builder CombineTextElements()
        {
            // Only if needed...
            if (this.Count(x => x is StrTokenText) > 1)
            {
                var cloned = Clone();
                var changed = false;

                for (int i = 1; i < cloned.Count; i++) // Starts at 1...
                {
                    if (cloned[i - 1] is StrTokenText prev && cloned[i] is StrTokenText item)
                    {
                        var temp = new StrTokenText($"{prev.Payload}{item.Payload}");

                        cloned.Replace(i - 1, temp);
                        cloned.RemoveAt(i);
                        changed = true;
                        i--;
                    }
                }

                if (changed) return cloned;
            }

            // Finishing...
            return this;
        }
    }
}