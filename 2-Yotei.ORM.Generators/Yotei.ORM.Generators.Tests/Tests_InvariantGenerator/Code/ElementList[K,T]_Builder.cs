using TKey = string;
using IItem = Yotei.ORM.InvariantGenerator.Tests.IElement;
using IHost = Yotei.ORM.InvariantGenerator.Tests.IElementList_KT;
using THost = Yotei.ORM.InvariantGenerator.Tests.ElementList_KT;

namespace Yotei.ORM.InvariantGenerator.Tests;

partial class ElementList_KT
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="IHost.IBuilder"/>
    /// </summary>
    [DebuggerDisplay("{ToDebugString(3)}")]
    [Cloneable]
    public partial class Builder : CoreList<TKey, IItem>, IHost.IBuilder
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => Engine = engine.ThrowWhenNull();

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

        /// <summary>
        /// <inheritdoc cref="IHost.IBuilder.ToInstance"/>
        /// </summary>
        /// <returns></returns>
        public THost ToInstance()
        {
            var host = new THost(Engine) { AcceptDuplicates = AcceptDuplicates };
            if (Count > 0) host.AddRange(this);
            return host;
        }
        IHost IHost.IBuilder.ToInstance() => ToInstance();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEngine Engine { get; }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override IItem ValidateElement(IItem value) => value.ThrowWhenNull();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override TKey GetKey(IItem item) => item is NamedElement named
            ? named.Name
            : throw new ArgumentException("Element is not a named one.").WithData(item);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override TKey ValidateKey(TKey key) => key.NotNullNotEmpty(true);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool CompareKeys(TKey source, TKey target)
        {
            var comparer = new MyComparer(Engine.IgnoreCase);
            return comparer.Equals(source, target);
        }

        readonly struct MyComparer(bool IgnoreCase) : IEqualityComparer<TKey>
        {
            public bool Equals(TKey? x, TKey? y) => string.Compare(x, y, IgnoreCase) == 0;
            public int GetHashCode(TKey _) => throw new UnExpectedException();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override IEnumerable<IItem> FindDuplicates(TKey key) => base.FindDuplicates(key);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="_"></param>
        /// <returns></returns>
        public override bool AllowDuplicate(IItem value)
        {
            if (AcceptDuplicates) return true;
            throw new DuplicateException("Duplicated value").WithData(value);
        }

        // For debug purposes...
        public bool AcceptDuplicates
        {
            get;
            set
            {
                if (field == value) return;
                if (Count == 0) { field = value; return; }

                var range = ToList(); Clear();
                field = value; AddRange(range);
            }
        }
    }
}