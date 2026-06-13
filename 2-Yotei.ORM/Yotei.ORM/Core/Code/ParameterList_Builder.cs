using IHost = Yotei.ORM.IParameterList;
using THost = Yotei.ORM.Code.ParameterList;
using IItem = Yotei.ORM.IParameter;
using TKey = string;

namespace Yotei.ORM.Code;

partial class ParameterList
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
        public THost ToInstance() => Count == 0 ? new(Engine) : new(Engine, this);
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
        public override TKey GetKey(IItem item) => item.ThrowWhenNull().Name;

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
        /// <param name="_"></param>
        /// <param name="_"></param>
        /// <returns></returns>
        public override bool AllowDuplicate(IItem _) => true;

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public string NextName()
        {
            for (int i = Count; i < int.MaxValue; i++)
            {
                var name = $"{Engine.ParameterPrefix}{i}";
                var index = IndexOf(name);
                if (index < 0) return name;
            }

            throw new UnExpectedException("Range of integers exahusted.");
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual int AddNew(object? value, out IItem item)
        {
            item = new Parameter(NextName(), value);
            return Add(item);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual int InsertNew(int index, object? value, out IItem item)
        {
            item = new Parameter(NextName(), value);
            return Insert(index, item);
        }
    }
}