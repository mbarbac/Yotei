﻿using IHost = Yotei.ORM.IIdentifierChain;
using THost = Yotei.ORM.Code.IdentifierChain;
using IItem = Yotei.ORM.IIdentifierPart;
using TKey = string;

namespace Yotei.ORM.Code;

public partial class IdentifierChain
{
    // ====================================================
    /// <inheritdoc cref="IHost.IBuilder"/>
    [Cloneable]
    [DebuggerDisplay("{ToDebugString(5)}")]
    public partial class Builder : CoreList<TKey?, IItem>, IHost.IBuilder
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => Engine = engine.ThrowWhenNull();

        /// <summary>
        /// Initializes a new empty instance with the given initial capacity.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="capacity"></param>
        public Builder(IEngine engine, int capacity) : this(engine) => Capacity = capacity;

        /// <summary>
        /// Initializes a new instance with the given element.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="item"></param>
        public Builder(IEngine engine, IItem item) : this(engine) => Add(item);

        /// <summary>
        /// Initializes a new instance with the elements of the given range.
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

        /// <inheritdoc/>
        public override string ToString() => Value ?? string.Empty;

        // ------------------------------------------------

        /// <summary>
        /// Returns a new instance with the elements obtained from the given value.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="value"></param>
        public Builder(IEngine engine, string? value) : this(engine) => Add(value);

        /// <summary>
        /// Returns a new instance with the elements obtained from the given range of values.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(
            IEngine engine, IEnumerable<string?> range) : this(engine) => AddRange(range);

        // ------------------------------------------------

        /// <inheritdoc/>
        public THost ToInstance() => Count == 0 ? new(Engine) : new(Engine, this);
        IHost IHost.IBuilder.ToInstance() => ToInstance();

        /// <inheritdoc/>
        public IEngine Engine { get; }

        /// <inheritdoc/>
        public string? Value
        {
            get
            {
                if (_Changed)
                {
                    _Value = Count == 0 || this.All(x => x.Value == null)
                        ? null
                        : string.Join('.', this.Select(x => x.Value));

                    _Changed = false;
                }

                return _Value;
            }
        }
        string? _Value = null;
        bool _Changed = true;

        // ----------------------------------------------------

        /// <inheritdoc/>
        public override IItem ValidateItem(IItem item)
        {
            item.ThrowWhenNull();

            if (!Engine.Equals(item.Engine)) throw new ArgumentException(
                "Element's engine is not the same as the one of this instance.")
                .WithData(item)
                .WithData(this);

            return item;
        }

        /// <inheritdoc/>
        public override TKey? GetKey(IItem item) => item.ThrowWhenNull().Value;

        /// <inheritdoc/>
        public override TKey? ValidateKey(TKey? key) => key is null
        ? null
        : new IdentifierPart(Engine, key).Value;

        /// <inheritdoc/>
        public override IEqualityComparer<TKey?> Comparer => Engine.CaseSensitiveNames
            ? StringComparer.CurrentCulture
            : StringComparer.CurrentCultureIgnoreCase;

        /// <inheritdoc/>
        public override bool ExpandItems => true;

        /// <inheritdoc/>
        public override bool IncludeDuplicated(IItem item, IItem source) => true;

        // ----------------------------------------------------

        /// <summary>
        /// Conditionally reduces this instance to a simpler form, if such is requested and if the
        /// given number of changes is greater than cero, and returns that number of changes.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        int Reduce(int r, bool reduce)
        {
            if (r > 0)
            {
                if (reduce)
                {
                    while (Count > 0)
                    {
                        if (this[0].Value == null) base.RemoveAt(0);
                        else break;
                    }
                }

                _Changed = true; // Always needed because 'r > 0'...
            }

            return r;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override int Replace(int index, IItem item) => Replace(index, item, true);
        public int Replace(int index, IItem item, bool reduce)
        {
            item.ThrowWhenNull();

            var r = base.Replace(index, item);
            return Reduce(r, reduce);
        }

        /// <inheritdoc/>
        public override int Add(IItem item) => Add(item, true);
        public int Add(IItem item, bool reduce)
        {
            item.ThrowWhenNull();
            if (Count == 0 && item.Value == null && reduce) return 0;

            var r = base.Add(item);
            return Reduce(r, reduce);
        }

        /// <inheritdoc/>
        public override int AddRange(IEnumerable<IItem> range) => AddRange(range, true);
        public int AddRange(IEnumerable<IItem> range, bool reduce)
        {
            range.ThrowWhenNull();
            if (range.Any(x => x == null)) throw new ArgumentException("Range contains null elements.").WithData(range);

            if (Count == 0 && range.All(x => x.Value == null) && reduce) return 0;

            var r = base.AddRange(range);
            return Reduce(r, reduce);
        }

        /// <inheritdoc/>
        public override int Insert(int index, IItem item) => Insert(index, item, true);
        public int Insert(int index, IItem item, bool reduce)
        {
            item.ThrowWhenNull();
            if (Count == 0 && item.Value == null) return 0;
            if (index == 0 && item.Value == null && reduce) return 0;

            var r = base.Insert(index, item);
            return Reduce(r, reduce);
        }

        /// <inheritdoc/>
        public override int InsertRange(int index, IEnumerable<IItem> range) => InsertRange(index, range, true);
        public int InsertRange(int index, IEnumerable<IItem> range, bool reduce)
        {
            range.ThrowWhenNull();
            if (range.Any(x => x == null)) throw new ArgumentException("Range contains null elements.").WithData(range);

            if (Count == 0 && range.All(x => x.Value == null) && reduce) return 0;
            if (index == 0 && range.All(x => x.Value == null) && reduce) return 0;

            var r = base.InsertRange(index, range);
            return Reduce(r, reduce);
        }

        /// <inheritdoc/>
        public override int RemoveAt(int index) => RemoveAt(index, true);
        public int RemoveAt(int index, bool reduce)
        {
            var r = base.RemoveAt(index);
            return Reduce(r, reduce);
        }

        /// <inheritdoc/>
        public override int RemoveRange(int index, int count) => RemoveRange(index, count, true);
        public int RemoveRange(int index, int count, bool reduce)
        {
            var r = base.RemoveRange(index, count);
            return Reduce(r, reduce);
        }

        /// <inheritdoc/>
        public override int Remove(string? key) => Remove(key, true);
        public int Remove(string? key, bool reduce)
        {
            var r = base.Remove(key);
            return Reduce(r, reduce);
        }

        /// <inheritdoc/>
        public override int RemoveLast(string? key) => RemoveLast(key, true);
        public int RemoveLast(string? key, bool reduce)
        {
            var r = base.RemoveLast(key);
            return Reduce(r, reduce);
        }

        /// <inheritdoc/>
        public override int RemoveAll(string? key) => RemoveAll(key, true);
        public int RemoveAll(string? key, bool reduce)
        {
            var r = base.RemoveAll(key);
            return Reduce(r, reduce);
        }

        /// <inheritdoc/>
        public override int Remove(Predicate<IItem> predicate) => Remove(predicate, true);
        public int Remove(Predicate<IItem> predicate, bool reduce)
        {
            var r = base.Remove(predicate);
            return Reduce(r, reduce);
        }

        /// <inheritdoc/>
        public override int RemoveLast(Predicate<IItem> predicate) => RemoveLast(predicate, true);
        public int RemoveLast(Predicate<IItem> predicate, bool reduce)
        {
            var r = base.RemoveLast(predicate);
            return Reduce(r, reduce);
        }

        /// <inheritdoc/>
        public override int RemoveAll(Predicate<IItem> predicate) => RemoveAll(predicate, true);
        public int RemoveAll(Predicate<IItem> predicate, bool reduce)
        {
            var r = base.RemoveAll(predicate);
            return Reduce(r, reduce);
        }

        /// <inheritdoc/>
        public override int Clear() => Clear(true);
        public int Clear(bool reduce)
        {
            var r = base.Clear();
            return Reduce(r, reduce);
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns a list with the dot-separated parts obtained from the given value.
        /// </summary>
        List<IdentifierPart> GetParts(string? value, bool reduce)
        {

            var items = Identifier.GetParts(Engine, value, reduce);

            if (items.Count == 0) return [];
            else
            {
                var list = new List<IdentifierPart>(items.Count);
                foreach (var item in items)
                {
                    if (item is null) list.Add(new(Engine));
                    else
                    {
                        var str = !Engine.UseTerminators
                            ? item
                            : item.Wrap(Engine.LeftTerminator, Engine.RightTerminator, trim: true);

                        list.Add(new(Engine, str));
                    }
                }

                return list;
            }
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public virtual int Replace(int index, string? value) => Replace(index, value, true);
        int Replace(int index, string? value, bool reduce)
        {
            var parts = GetParts(value, reduce: false);
            if (parts.Count == 0) return 0;

            var temp = this[index];
            var removed = base.RemoveAt(index);
            if (removed == 0) { base.Insert(index, temp); return 0; }

            var r = 0; foreach (var part in parts)
            {
                var num = Insert(index, part, reduce: false);
                r += num;
                index += num;
            }

            return Reduce(r, reduce);
        }

        /// <inheritdoc/>
        public virtual int Add(string? value) => Add(value, true);
        int Add(string? value, bool reduce)
        {
            var parts = GetParts(value, reduce: false);

            var r = 0; foreach (var part in parts)
            {
                var temp = Add(part, reduce: false);
                r += temp;
            }

            return Reduce(r, reduce);
        }

        /// <inheritdoc/>
        public virtual int AddRange(IEnumerable<string?> range) => AddRange(range, true);
        int AddRange(IEnumerable<string?> range, bool reduce)
        {
            range.ThrowWhenNull();

            var r = 0; foreach (var item in range)
            {
                var temp = Add(item, reduce: false);
                r += temp;
            }

            return Reduce(r, reduce);
        }

        /// <inheritdoc/>
        public virtual int Insert(int index, string? value) => Insert(index, value, true);
        int Insert(int index, string? value, bool reduce)
        {
            var parts = GetParts(value, reduce: false);

            var r = 0; foreach (var part in parts)
            {
                var temp = Insert(index, part, reduce: false);
                r += temp;
                index += temp;
            }

            return Reduce(r, reduce);
        }

        /// <inheritdoc/>
        public virtual int InsertRange(int index, IEnumerable<string?> range) => InsertRange(index, range, true);
        int InsertRange(int index, IEnumerable<string?> range, bool reduce)
        {
            range.ThrowWhenNull();

            var r = 0; foreach (var item in range)
            {
                var temp = Insert(index, item, reduce: false);
                r += temp;
                index += temp;
            }

            return Reduce(r, reduce);
        }
    }
}