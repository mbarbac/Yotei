using TKey = string;
using IItem = Yotei.ORM.IParameter;
using IHost = Yotei.ORM.IParameterList;
using THost = Yotei.ORM.Code.ParameterList;
using System.Runtime.InteropServices.Marshalling;

namespace Yotei.ORM.Code;

partial class ParameterList
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="IHost.IBuilder"/>
    /// </summary>
    [Cloneable]
    public partial class Builder : CoreList<TKey, IItem>, IHost.IBuilder
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => Engine = engine.ThrowWhenNull();

        /// <summary>
        /// Initializes a new intance with the elements of the given range.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(IEngine engine, IEnumerable<IItem> range)
            : this(engine)
            => AddRange(range.ThrowWhenNull());

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected Builder(Builder other)
        {
            ArgumentNullException.ThrowIfNull(other);

            OnCreating(other);
            Engine = other.Engine;
            AddRange(other);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual IHost ToInstance()
        {
            var host = new THost(Engine);
            host.Items.FlattenElements = FlattenElements;

            if (Count != 0) host.Items.AddRange(this);
            return host;
        }

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
        public override TKey GetKey(IItem value) => value.ThrowWhenNull().Name;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override TKey ValidateKey(TKey key) => key.NotNullNotEmpty(trim: true);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool FlattenElements { get => true; set { } }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool CompareKeys(
            TKey source, TKey target)
            => string.Compare(source, target, Engine.IgnoreCase) == 0;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public override bool AllowDuplicate(IItem value, IEnumerable<IItem> range)
        {
            foreach (var item in range)
            {
                if (!ReferenceEquals(value, item)) throw new DuplicateException(
                    "Value is not the same instance as an existing one.")
                    .WithData(value)
                    .WithData(item);
            }
            return true;
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
        public IEngine Engine { get; }

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
            throw new UnExpectedException("Range of integers exhausted.");
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="item"></param>
        /// <returns><inheritdoc/></returns>
        public virtual int AddNew(object? value, out IItem item)
        {
            item = new Parameter(NextName(), value);
            return Add(item);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="item"></param>
        /// <returns><inheritdoc/></returns>
        public virtual int InsertNew(int index, object? value, out IItem item)
        {
            item = new Parameter(NextName(), value);
            return Insert(index, item);
        }
    }
}
