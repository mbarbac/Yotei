using THost = Yotei.ORM.Code.ParameterList;
using IHost = Yotei.ORM.IParameterList;
using IItem = Yotei.ORM.IParameter;
using TKey = string;

namespace Yotei.ORM.Code;

partial class ParameterList
{
    // ====================================================
    /// <inheritdoc cref="IHost.IBuilder"/>
    [Cloneable<IHost.IBuilder>]
    [DebuggerDisplay("{ToDebugString(5)}")]
    public partial class Builder : CoreList<TKey, IItem>, IHost.IBuilder
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) : base() => Engine = engine.ThrowWhenNull();

        /// <summary>
        /// Initializes a new instance with the elements from the given range.
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

        // ------------------------------------------------

        /// <inheritdoc/>
        public override IItem ValidateItem(IItem item) => item.ThrowWhenNull();

        /// <inheritdoc/>
        public override string GetKey(IItem item) => item.Name;

        /// <inheritdoc/>
        public override string ValidateKey(string key) => key.NotNullNotEmpty();

        /// <inheritdoc/>
        public override bool ExpandItems => false;

        /// <inheritdoc/>
        public override bool IsValidDuplicate(IItem source, IItem item)
            => ReferenceEquals(source, item)
            ? true
            : throw new DuplicateException("Duplicated element.").WithData(item);

        /// <inheritdoc/>
        public override IEqualityComparer<TKey> Comparer => _Comparer ??= new(Engine.CaseSensitiveNames);
        MyComparer? _Comparer;
        readonly struct MyComparer(bool Sensitive) : IEqualityComparer<TKey>
        {
            public bool Equals(TKey? x, TKey? y)
                => string.Compare(x, y, !Sensitive) == 0;

            public int GetHashCode(TKey obj) => throw new NotImplementedException();
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public virtual IHost CreateInstance()
            => Count == 0 ? new THost(Engine) : new THost(Engine, this);

        /// <inheritdoc/>
        public IEngine Engine
        {
            get => _Engine;
            set
            {
                value.ThrowWhenNull();

                if (_Engine?.Equals(value) ?? false) return;
                _Engine = value;

                if (Count == 0) return;
                var range = ToList();
                Clear();
                AddRange(range);
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
