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
        public Builder(
            IEngine engine, IEnumerable<string?> range) : this(engine) => AddRange(range);

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
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Value ?? string.Empty;

        /// <summary>
        /// Tries to wrap the given raw part with the engine terminators, if possible
        /// </summary>
        string? Wrap(string? str, bool useTerminators)
            => str != null && useTerminators && Engine.UseTerminators
            ? $"{Engine.LeftTerminator}{str}{Engine.RightTerminator}"
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
                return Items.Count == 0 || Items.All(static x => x == null)
                    ? null
                    : string.Join('.', Items.Select(x => Wrap(x, true)));
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
        public IEnumerable<string?> Enumerate(bool useTerminators)
        {
            foreach (var item in Items) yield return Wrap(item, useTerminators);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Reduce()
        {
            while (Items.Count > 0)
            {
                if (Items[0] == null) Items.RemoveAt(0);
                else break;
            }
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
        public virtual int Replace(int index, string? value, bool reduce = true) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        public virtual int Add(string? value, bool reduce = true) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        public virtual int AddRange(IEnumerable<string?> range, bool reduce = true) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        public virtual int Insert(int index, string? value, bool reduce = true) => throw null;
        /*{
        if (value is IEnumerable<T> range) return InsertRange(index, range);

        value = ValidateElement(value);
        var dups = FindDuplicates(value);
        if (dups.Any() && !AllowDuplicate(value, dups)) return 0;

        Items.Insert(index, value);
        return 1;
    }*/

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        public virtual int InsertRange(int index, IEnumerable<string?> range, bool reduce = true) => throw null;
        /*{
        ArgumentNullException.ThrowIfNull(range);

        var num = 0; foreach (var value in range)
        {
            var r = Insert(index, value);
            index += r;
            num += r;
        }
        return num;
    }*/

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual int RemoveAt(int index) => throw null;
        /*{
        Items.RemoveAt(index);
        return 1;
    }*/

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual int RemoveRange(int index, int count) => throw null;
        /* {
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
    }*/

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public virtual int Remove(string? part) => throw null;
        /*{
        if (value is IEnumerable<T> range) // Nested case...
        {
            var num = 0; foreach (var item in range) num += Remove(item);
            return num;
        }
        else // Standard case...
        {
            var index = IndexOf(value);
            var num = index < 0 ? 0 : RemoveAt(index);
            return num;
        }
    }*/

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public virtual int RemoveLast(string? part) => throw null;
        /*{
        if (value is IEnumerable<T> range) // Nested case...
        {
            var num = 0; foreach (var item in range) num += RemoveLast(item);
            return num;
        }
        else // Standard case...
        {
            var index = LastIndexOf(value);
            var num = index < 0 ? 0 : RemoveAt(index);
            return num;
        }
    }*/

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public virtual int RemoveAll(string? part) => throw null;
        /*{
        if (value is IEnumerable<T> range) // Nested case...
        {
            var num = 0; foreach (var item in range) num += RemoveAll(item);
            return num;
        }
        else // Standard case...
        {
            var num = 0; while (true)
            {
                var index = IndexOf(value); if (index < 0) break;
                var r = RemoveAt(index); if (r == 0) break;
                num += r;
            }
            return num;
        }
    }*/

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int Remove(Predicate<string?> predicate) => throw null;
        /*{
        var index = IndexOf(predicate);
        return index < 0 ? 0 : RemoveAt(index);
    }*/

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int RemoveLast(Predicate<string?> predicate) => throw null;
        /*{
        var index = LastIndexOf(predicate);
        return index < 0 ? 0 : RemoveAt(index);
    }*/

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int RemoveAll(Predicate<string?> predicate) => throw null;
        /*{
        var num = 0; while (true)
        {
            var index = IndexOf(predicate); if (index < 0) break;
            var r = RemoveAt(index); if (r == 0) break;
            num += r;
        }
        return num;
    }*/

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual int Clear() => throw null;
        /*{
        var num = Items.Count; if (num > 0) Items.Clear();
        return num;
    }*/
    }
}