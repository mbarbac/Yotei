using TKey = Yotei.ORM.IIdentifier;
using IItem = Yotei.ORM.Records.ISchemaEntry;
using IHost = Yotei.ORM.Records.ISchema;
using THost = Yotei.ORM.Records.Code.Schema;
using System.Runtime.InteropServices.Marshalling;

namespace Yotei.ORM.Records.Code;

partial class Schema
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="IHost.IBuilder"/>
    /// </summary>
    [DebuggerDisplay("{ToDebugString(3)}")]
    [Cloneable]
    public partial class Builder : CoreList<TKey?, IItem>, IHost.IBuilder
    {
        /// <summary>
        /// Initializes an empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) : base() => Engine = engine.ThrowWhenNull();

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
        /// <param name="other"></param>
        protected Builder(Builder other)
        {
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
                "Element's engine is not the same as the one of this instance.")
                .WithData(value)
                .WithData(this);

            ValidateKey(value.Identifier);
            return value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override TKey? GetKey(IItem value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return value.Identifier;
        }

        /// <summary>
        /// <inheritdoc/>
        /// <br/> Valid identifiers match this instance's engine, and their values are not null
        /// and not empty, and do not end with a null or empty part.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override TKey ValidateKey(TKey? key)
        {
            ArgumentNullException.ThrowIfNull(key);

            if (!Engine.Equals(key.Engine)) throw new ArgumentException(
                "Identifier's engine is not the same as the one of this instance.")
                .WithData(key)
                .WithData(this);

            if (key.Value is null || key.Count == 0) throw new ArgumentException(
                "Identifier value cannot be null or empty.");

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
        public override bool CompareKeys(TKey? source, TKey? target)
        {
            if (source is null && target is null) return true;
            if (source is null || target is null) return false;

            return string.Compare(source.Value, target.Value, Engine.IgnoreCase) == 0;
        }

        /// <summary>
        /// <inheritdoc/>
        /// <br/> Schemas also intercept elements whose identifiers match any of the existing
        /// ones.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override IEnumerable<IItem> FindDuplicates(TKey? key)
        {
            ArgumentNullException.ThrowIfNull(key);
            var specs = key.ToStringEx(reduce: true, useTerminators: false);

            var nums = IndexesOf(x => x.Identifier!.Match(specs));
            var temp = IndexesOf(x => key.Match(x.Identifier!.Value));

            foreach (var num in temp) if (!nums.Contains(num)) nums.Add(num);
            nums.Sort();
            return nums.Select(x => this[x]);
        }

        /// <summary>
        /// <inheritdoc/>
        /// <br/> Duplicates are only allowed if they are strictly the same entry as an existing
        /// one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="existing"></param>
        /// <returns></returns>
        public override bool AllowDuplicate(IItem value, IEnumerable<IItem> existing)
        {
            ArgumentNullException.ThrowIfNull(value);
            ArgumentNullException.ThrowIfNull(existing);

            if (existing.Any(x => ReferenceEquals(x, value))) return true;

            throw new DuplicateException(
                "This instance already carries an entry with equivalent identifier.")
                .WithData(value)
                .WithData(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool SameElements(
            IItem source, IItem target) => ReferenceEquals(source, target);

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual THost ToInstance() => Count == 0
            ? new THost(Engine)
            : new THost(Engine, this);

        IHost IHost.IBuilder.ToInstance() => ToInstance();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEngine Engine { get; }

        // ----------------------------------------------------

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
        /// <param name="specs"></param>
        /// <param name="unique"></param>
        /// <returns></returns>
        public List<int> Match(string? specs, out IItem? unique)
        {
            List<int> list = [];

            for (int i = 0; i < Count; i++)
            {
                var item = this[i];
                var match = item.Identifier!.Match(specs);
                if (match) list.Add(i);
            }

            unique = list.Count == 1 ? this[list[0]] : null;
            return list;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public virtual bool Remove(string identifier)
        {
            var index = IndexOf(identifier);

            if (index < 0) return false;
            return RemoveAt(index) > 0;
        }
    }
}