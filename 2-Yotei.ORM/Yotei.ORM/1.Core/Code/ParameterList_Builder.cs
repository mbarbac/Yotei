using IHost = Yotei.ORM.IParameterList;
using IItem = Yotei.ORM.IParameter;
using TKey = string;

namespace Yotei.ORM.Code;

partial class ParameterList
{
    // ====================================================
    /// <inheritdoc cref="IHost.IBuilder"/>
    [DebuggerDisplay("{ToDebugString(5)}")]
    [Cloneable]
    public partial class Builder : CoreList<TKey, IItem>, IHost.IBuilder
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => Engine = engine;

        /// <summary>
        /// Initializes a new empty instance with the given initial capacity.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="capacity"></param>
        public Builder(IEngine engine, int capacity) : this(engine) => Capacity = capacity;

        /// <summary>
        /// Initializes a new instance with the elements of the given range.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(IEngine engine, IEnumerable<IItem> range) : this(engine) => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) : this(source.Engine) => AddRange(source);

        /// <inheritdoc/>
        public override string ToString() => base.ToString();

        // ----------------------------------------------------

        /// <inheritdoc/>
        public override IItem ValidateItem(IItem item) => item.ThrowWhenNull();

        /// <inheritdoc/>
        public override TKey GetKey(IItem item) => item.Name;

        /// <inheritdoc/>
        public override TKey ValidateKey(TKey key) => key.NotNullNotEmpty();

        /// <inheritdoc/>
        public override bool ExpandItems => false;

        /// <inheritdoc/>
        public override bool IsValidDuplicate(IItem source, IItem item)
            => ReferenceEquals(source, item)
            ? true
            : throw new DuplicateException("Duplicated Element.").WithData(item);

        /// <inheritdoc/>
        public override IEqualityComparer<TKey> Comparer => _Comparer ??= new ItemComparer(this);
        ItemComparer? _Comparer = null;

        readonly struct ItemComparer(Builder Master) : IEqualityComparer<TKey>
        {
            public bool Equals(TKey? x, TKey? y)
                => string.Compare(x, y, !Master.Engine.CaseSensitiveNames) == 0;

            public int GetHashCode([DisallowNull] TKey obj) => throw new NotImplementedException();
        }

        // ------------------------------------------------

        /// <inheritdoc cref="IHost.IBuilder.CreateInstance"/>
        public virtual ParameterList CreateInstance() => new(Engine, this);
        IHost IHost.IBuilder.CreateInstance() => CreateInstance();

        /// <inheritdoc/>
        public IEngine Engine
        {
            get => _Engine;
            set
            {
                if (!ReferenceEquals(_Engine, value))
                {
                    var range = ToList();

                    Clear(); _Engine = value;
                    AddRange(range);
                }
            }
        }
        IEngine _Engine = default!;

        // ------------------------------------------------

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public virtual int AddNew(object? value, out IItem item)
        {
            item = new Parameter(NextName(), value);
            return Add(item);
        }

        /// <inheritdoc/>
        public virtual int InsertNew(int index, object? value, out IItem item)
        {
            item = new Parameter(NextName(), value);
            return Insert(index, item);
        }
    }
}