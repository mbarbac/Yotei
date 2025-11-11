using THost = Yotei.ORM.Code.IdentifierChain;
using IHost = Yotei.ORM.IIdentifierChain;
using IItem = Yotei.ORM.IIdentifierUnit;
using TKey = string;

namespace Yotei.ORM.Code;

partial class IdentifierChain
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="IHost.IBuilder"/>
    /// </summary>
    [Cloneable<IHost.IBuilder>]
    [DebuggerDisplay("{ToDebugString(5)}")]
    public partial class Builder : CoreList<TKey?, IItem>, IHost.IBuilder
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) : base() => Engine = engine.ThrowWhenNull();

        /// <summary>
        /// Initializes a new empty instance with the given initial capacity.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="capacity"></param>
        public Builder(IEngine engine, int capacity) : this(engine) => Capacity = capacity;

        /// <summary>
        /// Initializes a new instance with the elements from the given range.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(
            IEngine engine, IEnumerable<IItem> range) : this(engine) => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) : this(source.Engine) => AddRange(source);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Value ?? string.Empty;

        // ------------------------------------------------

        /// <summary>
        /// Initializes a new instance with the elements obtained from the given value.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="value"></param>
        public Builder(IEngine engine, string? value) : this(engine) => Add(value);

        /// <summary>
        /// Initializes a new instance with the elements obtained from the given range of values.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(
            IEngine engine, IEnumerable<string?> range) : this(engine) => AddRange(range);

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override IItem ValidateItem(IItem item)
        {
            item.ThrowWhenNull();

            if (!Engine.Equals(item.Engine)) throw new ArgumentException(
                "The engine of the given element is not the same as the one of this instance.")
                .WithData(item)
                .WithData(this);

            return item;
        }

        /// <inheritdoc/>
        protected override TKey? GetKey(IItem item) => item.ThrowWhenNull().Value;

        /// <inheritdoc/>
        protected override TKey? ValidateKey(TKey? key) => key is null
            ? null
            : new IdentifierUnit(Engine, key).Value;

        /// <inheritdoc/>
        protected override bool ExpandElements => true;

        /// <inheritdoc/>
        protected override bool IsValidDuplicate(IItem source, IItem item) => true;

        /// <inheritdoc/>
        protected override IEqualityComparer<TKey?> Comparer => _Comparer ??= new(Engine.CaseSensitiveNames);
        MyComparer? _Comparer;

        readonly struct MyComparer(bool Sensitive) : IEqualityComparer<TKey?>
        {
            public bool Equals(TKey? x, TKey? y) => string.Compare(x, y, !Sensitive) == 0;
            public int GetHashCode(TKey obj) => throw new NotImplementedException();
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        /// Optimizing as we need not to compute duplicate indexes.
        protected override List<int> FindDuplicates(TKey? key) => [];

        /// <inheritdoc/>
        /// Optimizing as we need not to validate if items are the same.
        protected override bool SameItem(IItem source, IItem target) => false;

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual IHost CreateInstance()
            => Count == 0 ? new THost(Engine) : new THost(Engine, this);

        /// <summary>
        /// The engine this instance is associated with.
        /// </summary>
        public IEngine Engine
        {
            get;
            set
            {
                if (ReferenceEquals(field, value.ThrowWhenNull())) return;
                var range = ToList();
                Clear();
                _Value = null;
                _Changed = true;

                field = value; AddRange(range);
            }
        }

        /// <summary><
        /// <inheritdoc/>
        /// </summary>
        public string? Value
        {
            get
            {
                if (_Changed)
                {
                    _Changed = false;
                    _Value = Count == 0 || this.All(static x => x.Value == null)
                        ? null
                        : string.Join('.', this.Select(static x => x.Value));
                }
                return _Value;
            }
        }
        string? _Value = null;
        bool _Changed = true;

        // ------------------------------------------------

        /// <summary>
        /// Reduces this instance to a simpler form by removing the heading null parts.
        /// </summary>
        public void Reduce()
        {
            while (Count > 0)
            {
                if (this[0].Value == null) base.RemoveAt(0);
                else break;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public override int Replace(int index, IItem item) => Replace(index, item, true);
        int Replace(int index, IItem item, bool reduce)
        {
            item.ThrowWhenNull();

            var r = base.Replace(index, item);
            if (r > 0 && reduce) Reduce();
            return r;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override int Add(IItem item) => Add(item, true);
        int Add(IItem item, bool reduce)
        {
            item.ThrowWhenNull();

            if (Count == 0 && reduce && item.Value == null) return 0;

            var r = base.Add(item);
            if (r > 0 && reduce) Reduce();
            return r;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public override int AddRange(IEnumerable<IItem> range) => AddRange(range, true);
        int AddRange(IEnumerable<IItem> range, bool reduce)
        {
            range.ThrowWhenNull();

            if (Count == 0 && reduce &&
                range.All(static x => x is not null && x.Value is null)) return 0;

            var r = base.AddRange(range);
            if (r > 0 && reduce) Reduce();
            return r;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public override int Insert(int index, IItem item) => Insert(index, item, true);
        int Insert(int index, IItem item, bool reduce)
        {
            item.ThrowWhenNull();

            if (Count == 0 && reduce && item.Value == null) return 0;
            if (index == 0 && reduce && item.Value == null) return 0;

            var r = base.Insert(index, item);
            if (r > 0 && reduce) Reduce();
            return r;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public override int InsertRange(int index, IEnumerable<IItem> range) => InsertRange(index, range, true);
        int InsertRange(int index, IEnumerable<IItem> range, bool reduce)
        {
            range.ThrowWhenNull();

            if (Count == 0 && reduce &&
                range.All(static x => x is not null && x.Value is null)) return 0;

            if (index == 0 && reduce &&
                range.All(static x => x is not null && x.Value is null)) return 0;

            var r = base.InsertRange(index, range);
            if (r > 0 && reduce) Reduce();
            return r;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override int RemoveAt(int index) => RemoveAt(index, true);
        int RemoveAt(int index, bool reduce)
        {
            var r = base.RemoveAt(index);
            if (r > 0 && reduce) Reduce();
            return r;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int RemoveRange(int index, int count) => RemoveRange(index, count, true);
        int RemoveRange(int index, int count, bool reduce)
        {
            var r = base.RemoveRange(index, count);
            if (r > 0 && reduce) Reduce();
            return r;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override int Remove(TKey? key) => Remove(key, true);
        int Remove(TKey? key, bool reduce)
        {
            var r = base.Remove(key);
            if (r > 0 && reduce) Reduce();
            return r;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override int RemoveLast(TKey? key) => RemoveLast(key, true);
        int RemoveLast(TKey? key, bool reduce)
        {
            var r = base.RemoveLast(key);
            if (r > 0 && reduce) Reduce();
            return r;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override int RemoveAll(TKey? key) => RemoveAll(key, true);
        int RemoveAll(TKey? key, bool reduce)
        {
            var r = base.RemoveAll(key);
            if (r > 0 && reduce) Reduce();
            return r;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public override int Remove(Predicate<IItem> predicate) => Remove(predicate, true);
        int Remove(Predicate<IItem> predicate, bool reduce)
        {
            var r = base.Remove(predicate);
            if (r > 0 && reduce) Reduce();
            return r;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public override int RemoveLast(Predicate<IItem> predicate) => RemoveLast(predicate, true);
        int RemoveLast(Predicate<IItem> predicate, bool reduce)
        {
            var r = base.RemoveLast(predicate);
            if (r > 0 && reduce) Reduce();
            return r;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public override int RemoveAll(Predicate<IItem> predicate) => RemoveAll(predicate, true);
        int RemoveAll(Predicate<IItem> predicate, bool reduce)
        {
            var r = base.RemoveAll(predicate);
            if (r > 0 && reduce) Reduce();
            return r;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override int Clear() => Clear(true);
        int Clear(bool reduce)
        {
            var r = base.Clear();
            if (r > 0 && reduce) Reduce();
            return r;
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns a list with the parts found in the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        List<IItem> GetParts(string? value, bool reduce)
        {
            var items = Identifier.GetParts(Engine, value, reduce);
            if (items.Count == 0) return [];

            // It may happen that some parts have embedded dots if they were protected by the
            // engine terminators. If so, to prevent single-part exceptions, we need to re-wrap.

            return [.. items.Select(x => x is null
                ? new IdentifierUnit(Engine)
                : new IdentifierUnit(Engine, x.Contains('.')
                    ? x.Wrap(Engine.LeftTerminator, Engine.RightTerminator, false)
                    : x))];
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual int Replace(int index, string? value) => Replace(index, value, true);
        int Replace(int index, string? value, bool reduce)
        {
            var parts = GetParts(value, false);
            if (parts.Count == 0) parts.Add(new IdentifierUnit(Engine));

            var source = this[index];
            var r = base.RemoveAt(index);
            if (r > 0)
            {
                r = base.InsertRange(index, parts);
                if (r == 0)
                {
                    // Restoring if needed...
                    if (base.Insert(index, source) == 0) throw new InvalidOperationException(
                        "Cannot restore removed source element after failing replacement.")
                        .WithData(index)
                        .WithData(source)
                        .WithData(this);
                }
            }
            if (r > 0 && reduce) Reduce();
            return r;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual int Add(string? value) => Add(value, true);
        int Add(string? value, bool reduce)
        {
            var parts = value is null ? [new IdentifierUnit(Engine)] : GetParts(value, false);
            return AddRange(parts, reduce);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual int AddRange(IEnumerable<string?> range) => AddRange(range, true);
        int AddRange(IEnumerable<string?> range, bool reduce)
        {
            range.ThrowWhenNull();

            var parts = range.SelectMany(x => GetParts(x, false));
            return AddRange(parts, reduce);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual int Insert(int index, string? value) => Insert(index, value, true);
        int Insert(int index, string? value, bool reduce)
        {
            var parts = value is null ? [new IdentifierUnit(Engine)] : GetParts(value, false);
            return InsertRange(index, parts, reduce);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual int InsertRange(int index, IEnumerable<string?> range) => InsertRange(index, range, true);
        int InsertRange(int index, IEnumerable<string?> range, bool reduce)
        {
            range.ThrowWhenNull();

            var parts = range.SelectMany(x => GetParts(x, false));
            return InsertRange(index, parts, reduce);
        }
    }
}
