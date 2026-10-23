using TKey = Yotei.ORM.IIdentifier;
using IItem = Yotei.ORM.Records.ISchemaElement;
using IHost = Yotei.ORM.Records.ISchemaDescriptor;
using THost = Yotei.ORM.Records.Code.SchemaDescriptor;

namespace Yotei.ORM.Records.Code;

partial class SchemaDescriptor
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="IHost.IBuilder"/>
    /// </summary>
    [Cloneable]
    [DebuggerDisplay("{ToDebugString(3)}")]
    public partial class Builder : CoreList<TKey, IItem>, IHost.IBuilder
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => Engine = engine.ThrowWhenNull();

        /// <summary>
        /// Initializes a new instance with the elements from the given range.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(
            IEngine engine, IEnumerable<IItem> range) : this(engine) => AddRange(range);

        /// <summary>
        /// Initializes a new instance of the Builder class by copying values from an existing instance.
        /// </summary>
        /// <param name="other">The Builder instance to copy values from.</param>
        protected Builder(Builder other)
        {
            ArgumentNullException.ThrowIfNull(other);

            OnCreating(other);
            Engine = other.Engine;
            AddRange(other);
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override IItem ValidateElement(IItem value)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (!Engine.Equals(value.Engine)) throw new ArgumentException(
                "Element's engine is not the one of this instance.")
                .WithData(value)
                .WithData(this);

            ValidateKey(value.Identifier!);
            return value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override TKey GetKey(IItem value)
        {
            ArgumentNullException.ThrowIfNull(value);
            ArgumentNullException.ThrowIfNull(value.Identifier);
            return value.Identifier;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override TKey ValidateKey(TKey key)
        {
            ArgumentNullException.ThrowIfNull(key);

            if (!Engine.Equals(key.Engine)) throw new ArgumentException(
                "Identifier's engine is not the one of this instance.")
                .WithData(key)
                .WithData(this);

            if (key.Value is null || key.Count == 0) throw new EmptyException(
                "Idntifier value cannot be null or empty.")
                .WithData(key);

            var part = key.Count == 1 ? key[0] : key[^1];
            if (string.IsNullOrWhiteSpace(part)) throw new ArgumentException(
                "Identifier's last part cannot be null or empty.")
                .WithData(key);

            return key;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool CompareKeys(TKey source, TKey target)
        {
            if (source is null && target is null) return true;
            if (source is null || target is null) return false;

            return string.Compare(source.Value, target.Value, Engine.IgnoreCase) == 0;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override IEnumerable<IItem> FindDuplicates(TKey key)
        {
            ArgumentNullException.ThrowIfNull(key);

            var nums = IndexesOf(x => x.Identifier!.Match(key.Value));
            var temp = IndexesOf(x => key.Match(x.Identifier!.Value));

            foreach (var num in temp) if (!nums.Contains(num)) nums.Add(num);
            nums.Sort();
            return nums.Select(x => this[x]);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public override bool AllowDuplicate(IItem value, IEnumerable<IItem> range)
        {
            ArgumentNullException.ThrowIfNull(value);
            ArgumentNullException.ThrowIfNull(range);

            // Duplicates allowed only if are the same item (carrying the same metadata)...
            if (range.Any(x => ReferenceEquals(x, value))) return true;

            throw new DuplicateException(
                "This instance already carries other elements with the same identifier.")
                .WithData(value)
                .WithData(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool SameElement(
            IItem source, IItem target) => ReferenceEquals(source, target);

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual IHost ToInstance() => Count == 0
            ? new THost(Engine)
            : new THost(Engine, this);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEngine Engine { get; }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public bool Contains(string identifier) => IndexOf(identifier) >= 0;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public int IndexOf(string identifier)
        {
            identifier = identifier.NotNullNotEmpty(trim: true);

            var key = new Identifier(Engine, identifier);
            return IndexOf(key);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public int LastIndexOf(string identifier)
        {
            identifier = identifier.NotNullNotEmpty(trim: true);

            var key = new Identifier(Engine, identifier);
            return LastIndexOf(key);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public List<int> IndexesOf(string identifier)
        {
            identifier = identifier.NotNullNotEmpty(trim: true);

            var key = new Identifier(Engine, identifier);
            return IndexesOf(key);
        }

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="specs"></param>
        /// <returns></returns>
        public List<int> Match(string? specs) => Match(specs, out _);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="specs"></param>
        /// <param name="unique"></param>
        /// <returns></returns>
        public List<int> Match(string? specs, out IItem? unique)
        {
            List<int> list = []; for (int i = 0; i < Count; i++)
            {
                var item = this[i];
                var match = item.Identifier!.Match(specs);
                if (match) list.Add(i);
            }

            unique = list.Count == 1 ? this[list[0]] : null;
            return list;
        }

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns><inheritdoc/></returns>
        public virtual bool Remove(string identifier)
        {
            var index = IndexOf(identifier);
            return index >= 0 && (RemoveAt(index) > 0);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns><inheritdoc/></returns>
        public virtual bool RemoveLast(string identifier)
        {
            var index = LastIndexOf(identifier);
            return index >= 0 && (RemoveAt(index) > 0);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns><inheritdoc/></returns>
        public virtual bool RemoveAll(string identifier)
        {
            var done = false; while (true)
            {
                var temp = Remove(identifier);

                if (temp) done = true;
                else break;
            }
            return done;
        }
    }
}