namespace Yotei.ORM.Code;

partial class Identifier
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="IIdentifier.IBuilder"/>
    /// </summary>
    [Cloneable]
    public sealed partial class Builder : IIdentifier.IBuilder
    {
        // The collection of raw (not-terminated) parts.
        readonly List<string?> Items = [];

        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => Engine = engine.ThrowWhenNull();

        /// <summary>
        /// Initializes a new instance with the given collection of values, including multi-part
        /// ones.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="values"></param>
        public Builder(
            IEngine engine, params string?[]? values) : this(engine) => AddRange(values);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        Builder(Builder other)
        {
            Engine = other.ThrowWhenNull().Engine;
            Items = [.. other.Items];
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToString(false, true);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="reduce"></param>
        /// <param name="useTerminators"></param>
        /// <returns></returns>
        public string ToString(
            bool reduce, bool useTerminators) => string.Join('.', Enumerate(reduce, useTerminators));

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string?> GetEnumerator()
        {
            foreach (var item in Enumerate(false, false)) yield return item;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="reduce"></param>
        /// <param name="useTerminators"></param>
        /// <returns></returns>
        public IEnumerable<string?> Enumerate(bool reduce, bool useTerminators)
        {
            var items = reduce ? Items.SkipWhile(x => x is null) : Items;

            return useTerminators
                ? items.Select(x => Wrap(x, true))
                : items;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IIdentifier ToInstance() => Count == 0
            ? new Identifier(Engine)
            : new Identifier(Engine, [.. Items]);

        // ----------------------------------------------------

        /// <summary>
        /// Tries to wrap the given part with the engine's terminators if it is nor a null one,
        /// if they are used, and if such is explicitly requested.
        /// </summary>
        string? Wrap(string? part, bool useTerminators)
        {
            if (part is not null)
            {
                if (string.IsNullOrWhiteSpace(part)) part = null;
                else
                {
                    if (part.StartsWith(' ') || part.EndsWith(' ')) part = part.Trim();
                    
                    if (useTerminators &&
                        Engine.UseTerminators)
                        part = $"{Engine.LeftTerminator}{part}{Engine.RightTerminator}";
                }
            }
            return part;
        }

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
                var parts = Enumerate(false, true);
                return
                    parts.All(x => x == null) ? null :
                    string.Join('.', parts);
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
        public string? this[int index] => this[index, Engine.UseTerminators];

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
        /// <param name="part"></param>
        /// <returns></returns>
        public bool Contains(string? part) => IndexOf(part) >= 0;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public int IndexOf(string? part)
        {
            var parts = GetParts(Engine, part, reduce: false);

            if (parts.Count == 0) return -1;
            if (parts.Count > 1) throw new ArgumentException(
                "Part cannot contain first-level dots.")
                .WithData(part);

            return IndexOf(x => string.Compare(x, parts[0], Engine.IgnoreCase) == 0);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public int LastIndexOf(string? part)
        {
            var parts = GetParts(Engine, part, reduce: false);

            if (parts.Count == 0) return -1;
            if (parts.Count > 1) throw new ArgumentException(
                "Part cannot contain first-level dots.")
                .WithData(part);

            return LastIndexOf(x => string.Compare(x, parts[0], Engine.IgnoreCase) == 0);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public List<int> IndexesOf(string? part)
        {
            var parts = GetParts(Engine, part, reduce: false);

            if (parts.Count == 0) return [];
            if (parts.Count > 1) throw new ArgumentException(
                "Part cannot contain first-level dots.")
                .WithData(part);

            return IndexesOf(x => string.Compare(x, parts[0], Engine.IgnoreCase) == 0);
        }

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

            List<int> nums = []; for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                if (predicate(item)) nums.Add(i);
            }
            return nums;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Trim() => Items.TrimExcess();

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public int Reduce()
        {
            var num = 0; while (Items.Count > 0)
            {
                if (Items[0] is null) { Items.RemoveAt(0); num++; }
                else break;
            }
            return num;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><inheritdoc/></returns>
        public int Replace(int index, params string?[]? values)
        {
            values ??= [null];

            var old = Items[index];
            if (values.Length == 1 && old == values[0]) return 0;

            Items.RemoveAt(index);
            var num = InsertRange(index, values);

            if (num == 0) Items.Insert(index, old);
            return num;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns><inheritdoc/></returns>
        public int Add(string? value)
        {
            var parts = GetParts(Engine, value, reduce: false);

            if (parts.Count == 0) return 0;
            if (parts.Count > 1) return AddRange([.. parts]);

            Items.Add(parts[0]);
            return 1;

        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="values"></param>
        /// <returns><inheritdoc/></returns>
        public int AddRange(params string?[]? values)
        {
            values ??= [null];

            var num = 0; foreach (var value in values) num += Add(value);
            return num;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><inheritdoc/></returns>
        public int Insert(int index, string? value)
        {
            var parts = GetParts(Engine, value, reduce: false);

            if (parts.Count == 0) return 0;
            if (parts.Count > 1) return InsertRange(index, [.. parts]);

            Items.Insert(index, parts[0]);
            return 1;

        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="values"></param>
        /// <returns><inheritdoc/></returns>
        public int InsertRange(int index, params string?[]? values)
        {
            values ??= [null];

            var num = 0; foreach (var value in values)
            {
                var r = Insert(index, value);
                num += r;
                index += r;
            }
            return num;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns><inheritdoc/></returns>
        public int RemoveAt(int index)
        {
            Items.RemoveAt(index);
            return 1;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns><inheritdoc/></returns>
        public int RemoveRange(int index, int count)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfNegative(count);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(count, Count - index);

            var num = 0; while (count > 0)
            {
                if (RemoveAt(index) == 0) break;
                num++;
                count--;
            }
            return num;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns><inheritdoc/></returns>
        public int Remove(string? part)
        {
            var parts = GetParts(Engine, part, reduce: false);

            if (parts.Count == 0) return 0;
            if (parts.Count > 1) throw new ArgumentException(
                "Part cannot contain first-level dots.")
                .WithData(part);

            return Remove(x => string.Compare(x, part, Engine.IgnoreCase) == 0);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns><inheritdoc/></returns>
        public int RemoveLast(string? part)
        {
            var parts = GetParts(Engine, part, reduce: false);

            if (parts.Count == 0) return 0;
            if (parts.Count > 1) throw new ArgumentException(
                "Part cannot contain first-level dots.")
                .WithData(part);

            return RemoveLast(x => string.Compare(x, part, Engine.IgnoreCase) == 0);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns><inheritdoc/></returns>
        public int RemoveAll(string? part)
        {
            var parts = GetParts(Engine, part, reduce: false);

            if (parts.Count == 0) return 0;
            if (parts.Count > 1) throw new ArgumentException(
                "Part cannot contain first-level dots.")
                .WithData(part);

            return RemoveAll(x => string.Compare(x, part, Engine.IgnoreCase) == 0);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns><inheritdoc/></returns>
        public int Remove(Predicate<string?> predicate)
        {
            var index = IndexOf(predicate);
            return index < 0 ? 0 : RemoveAt(index);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns><inheritdoc/></returns>
        public int RemoveLast(Predicate<string?> predicate)
        {
            var index = LastIndexOf(predicate);
            return index < 0 ? 0 : RemoveAt(index);
        }

        /// <summary>
        /// <inheritdoc/>
        /// if any.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns><inheritdoc/></returns>
        public int RemoveAll(Predicate<string?> predicate)
        {
            var num = 0; while (true)
            {
                var index = IndexOf(predicate); if (index < 0) break;
                var r = RemoveAt(index); if (r == 0) break;
                num += r;
            }
            return num;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public int Clear()
        {
            var num = Items.Count; if (num > 0) Items.Clear();
            return num;
        }
    }
}