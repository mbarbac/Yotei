using System.Xml.Schema;

namespace Yotei.ORM.Code;

partial class Identifier
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="IIdentifier.IBuilder"/>
    /// </summary>
    [Cloneable]
    public partial class Builder : IIdentifier.IBuilder
    {
        readonly List<string?> Items;

        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine)
        {
            Engine = engine.ThrowWhenNull();
            Items = [];
        }

        /// <summary>
        /// Initializes a new instance with the parts obtained from the given range of values.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        /// <param name="reduce"></param>
        public Builder(
            IEngine engine, IEnumerable<string?> range, bool reduce = true) : this(engine)
        {
            AddRange(range, reduce);
            if (reduce) Reduce();
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected Builder(Builder other)
        {
            other.ThrowWhenNull();

            Engine = other.Engine;
            Items = [.. other.Items];
        }

        /// <summary>
        /// <inheritdoc cref="Identifier.ToString"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToStringEx();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="reduce"></param>
        /// <param name="wrap"></param>
        /// <returns></returns>
        public string ToStringEx(bool reduce = true, bool wrap = true)
        {
            var items = reduce && Items.Any(static x => x == null)
                ? Items.SkipWhile(x => x == null)
                : Items;

            return items.Any()
                ? string.Join('.', items.Select(x => Wrap(x, wrap)))
                : string.Empty;
        }

        // ----------------------------------------------------

        /// <summary>
        /// Tries to wrap the given raw part with the engine terminators, if possible
        /// </summary>
        string? Wrap(string? str, bool useTerminators)
            => str != null && useTerminators && Engine.UseTerminators
            ? $"{Engine.LeftTerminator}{str}{Engine.RightTerminator}"
            : str;

        /// <summary>
        /// Tries to unwrap the given part from the engine terminators, if used.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        string? UnWrap(string? str) => Engine.UseTerminators
            ? str.Unwrap(Engine.LeftTerminator, Engine.RightTerminator, trim: true).NullWhenEmpty(trim: true)
            : str;

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEngine Engine { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string? Value
        {
            get
            {
                var str = ToStringEx();
                return string.IsNullOrEmpty(str) ? null : str;
            }
            set
            {
                Items.Clear();
                Add(value);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int Count => Items.Count;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string? this[int index] => Items[index];

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="useTerminators"></param>
        /// <returns></returns>
        public string? this[int index, bool useTerminators] => Wrap(Items[index], useTerminators);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="useTerminators"></param>
        /// <returns></returns>
        public IEnumerable<string?> Enumerate(bool useTerminators = false)
        {
            foreach (var item in Items) yield return Wrap(item, useTerminators);
        }

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public bool Contains(string? part) => IndexOf(part) >= 0;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public int IndexOf(string? part)
            => IndexOf(x => string.Compare(x, part, Engine.IgnoreCase) == 0);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public int LastIndexOf(string? part)
            => LastIndexOf(x => string.Compare(x, part, Engine.IgnoreCase) == 0);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public List<int> IndexesOf(string? part)
            => IndexesOf(x => string.Compare(x, part, Engine.IgnoreCase) == 0);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Contains(Predicate<string?> predicate) => IndexOf(predicate) >= 0;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int IndexOf(Predicate<string?> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            return Items.FindIndex(predicate);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int LastIndexOf(Predicate<string?> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            return Items.FindLastIndex(predicate);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<int> IndexesOf(Predicate<string?> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            List<int> values = []; for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                if (predicate(item)) values.Add(i);
            }
            return values;
        }

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool Reduce()
        {
            var done = false;
            while (Items.Count > 0)
            {
                if (Items[0] == null) { Items.RemoveAt(0); done = true; }
                else break;
            }
            return done;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual IIdentifier ToInstance()
        {
            Reduce();
            return Count == 0 ? new Identifier(Engine) : new Identifier(Engine, Items);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        public virtual int Replace(int index, string? value, bool reduce = true)
        {
            var parts = Split(Engine, value, reduce);
            if (parts.Count <= 1)
            {
                var part = parts.Count == 0 ? null : parts[0];

                Items[index] = part;
                if (reduce) Reduce();
                return 1;
            }
            else
            {
                var num = 0;
                Items.RemoveAt(index); foreach (var part in parts)
                {
                    Items.Insert(index, part);
                    num++;
                    index++;
                }

                if (reduce) Reduce();
                return num;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        public virtual int Add(string? value, bool reduce = true)
        {
            var parts = Split(Engine, value, reduce);
            if (parts.Count <= 1)
            {
                var part = parts.Count == 0 ? null : parts[0];
                return AddOne(part, reduce);
            }
            else
            {
                var num = 0; foreach (var part in parts)
                {
                    // We can use the given 'reduce' as we don't care about the underlying index value...
                    var r = AddOne(part, reduce);
                    num += r;
                }
                // if (reduce) Reduce(); - redundant!
                return num;
            }
        }
        int AddOne(string? part, bool reduce)
        {
            if (Count == 0 && reduce && part == null) return 0;

            Items.Add(part);
            if (reduce) Reduce();
            return 1;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        public virtual int AddRange(IEnumerable<string?> range, bool reduce = true)
        {
            ArgumentNullException.ThrowIfNull(range);

            var num = 0; foreach (var value in range)
            {
                var parts = Split(Engine, value, reduce: false);
                foreach (var part in parts)
                {
                    if (Count == 0 && reduce && parts.All(static x => x == null)) continue;

                    // We can use the given 'reduce' as we don't care about the underlying index value...
                    var r = AddOne(part, reduce);
                    num += r;
                }
            }
            // if (reduce) Reduce(); - redundant!
            return num;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        public virtual int Insert(int index, string? value, bool reduce = true)
        {
            var parts = Split(Engine, value, reduce);
            if (parts.Count <= 1)
            {
                var part = parts.Count == 0 ? null : parts[0];
                return InsertOne(index, part, reduce);
            }
            else
            {
                var num = 0; foreach (var part in parts)
                {
                    // We must use 'reduce:false' because otherwise the underlying index may change!
                    var r = InsertOne(index, part, reduce: false);
                    num += r;
                    index += r;
                }
                if (reduce) Reduce();
                return num;
            }
        }
        int InsertOne(int index, string? part, bool reduce)
        {
            if (Count == 0 && reduce && part == null) return 0;
            if (index == 0 && reduce && part == null) return 0;

            Items.Insert(index, part);
            if (reduce) Reduce();
            return 1;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        public virtual int InsertRange(int index, IEnumerable<string?> range, bool reduce = true)
        {
            ArgumentNullException.ThrowIfNull(range);

            var num = 0; foreach (var value in range)
            {
                var parts = Split(Engine, value, reduce: false);
                foreach (var part in parts)
                {
                    if (Count == 0 && reduce && parts.All(static x => x == null)) continue;
                    if (index == 0 && reduce && parts.All(static x => x == null)) continue;

                    // We must use 'reduce:false' because otherwise the underlying index may change!
                    var r = InsertOne(index, part, reduce: false);
                    num += r;
                    index += r;
                }
            }
            if (reduce) Reduce();
            return num;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        public virtual int RemoveAt(int index, bool reduce = true)
        {
            Items.RemoveAt(index);
            if (reduce) Reduce();
            return 1;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        public virtual int RemoveRange(int index, int count, bool reduce = true)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfNegative(count);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(count, Count - index);

            var num = 0; while (count > 0)
            {
                Items.RemoveAt(index);
                num++;
                count--;
            }
            if (reduce) Reduce();
            return num;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="part"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        public virtual int Remove(string? part, bool reduce = true)
        {
            part = UnWrap(part);
            return Remove(x => string.Compare(x, part, Engine.IgnoreCase) == 0);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="part"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        public virtual int RemoveLast(string? part, bool reduce = true)
        {
            part = UnWrap(part);
            return RemoveLast(x => string.Compare(x, part, Engine.IgnoreCase) == 0);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="part"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        public virtual int RemoveAll(string? part, bool reduce = true)
        {
            part = UnWrap(part);
            return RemoveAll(x => string.Compare(x, part, Engine.IgnoreCase) == 0);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        public virtual int Remove(Predicate<string?> predicate, bool reduce = true)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            var index = Items.FindIndex(predicate);
            return index < 0 ? 0 : RemoveAt(index, reduce);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        public virtual int RemoveLast(Predicate<string?> predicate, bool reduce = true)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            var index = Items.FindLastIndex(predicate);
            return index< 0 ? 0 : RemoveAt(index, reduce);
    }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        public virtual int RemoveAll(Predicate<string?> predicate, bool reduce = true)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            var num = 0; while (true)
            {
                var index = Items.FindIndex(predicate); if (index < 0) break;
                var r = RemoveAt(index, reduce: false);
                num += r;
            }
            if (reduce) Reduce();
            return num;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual int Clear()
        {
            var num = Items.Count; if (num > 0) Items.Clear();
            return num;
        }
    }
}